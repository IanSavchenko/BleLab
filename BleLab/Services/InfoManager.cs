using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage;
using BleLab.GattInformation;
using BleLab.Model;
using LiteDB;

namespace BleLab.Services
{
    public class InfoManager
    {
        private readonly GattInformationProvider _gattInformationProvider;
        private readonly object _dbLock = new object();

        //public const string DevicesCollectionName = "devices";
        //public const string ServicesCollectionName = "services";

        private readonly string _dataBaseFilePath = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "Devices.db"));

        public InfoManager(GattInformationProvider gattInformationProvider)
        {
            _gattInformationProvider = gattInformationProvider;
        }

        public async Task<ICollection<DeviceInfo>> GetAllPairedDevicesInfo(DeviceInformationCollection pairedDevices)
        {
            var allSaved = GetAllSavedDevices();

            var result = new List<DeviceInfo>();
            foreach (var device in pairedDevices)
            {
                var glyph = await device.GetGlyphThumbnailAsync();
                var deviceInfo = allSaved.FirstOrDefault(t => t.DeviceId == device.Id);
                if (deviceInfo == null)
                    deviceInfo = new DeviceInfo { DeviceId = device.Id, IsNew = true };

                deviceInfo.Glyph = glyph;
                deviceInfo.Name = device.Name;
                deviceInfo.Initialize(this);

                result.Add(deviceInfo);
            }

            return result;
        }

        public async Task<ICollection<ServiceInfo>> GetAllServicesInfo(DeviceInfo deviceInfo, IEnumerable<GattDeviceService> services)
        {
            var result = new List<ServiceInfo>();
            var allSaved = GetAllSavedServices(deviceInfo);
            foreach (var gattDeviceService in services)
            {
                var serviceInfo = allSaved.FirstOrDefault(x => x.Uuid == gattDeviceService.Uuid);

                if (serviceInfo == null)
                {
                    serviceInfo = new ServiceInfo();
                    serviceInfo.Device = deviceInfo;
                    serviceInfo.Uuid = gattDeviceService.Uuid;

                    var gattServiceInfo = _gattInformationProvider.GetServiceInfo(serviceInfo.Uuid);
                    if (gattServiceInfo != null)
                    {
                        serviceInfo.Name = SelectName(gattServiceInfo.Name, gattServiceInfo.OriginalName);
                        serviceInfo.Description = SelectDescription(gattServiceInfo.Description, gattServiceInfo.Abstract, gattServiceInfo.Summary);
                        serviceInfo.WebLink = gattServiceInfo.Link;
                    }

                    if (serviceInfo.Name == null)
                        serviceInfo.Name = "Unknown service";

                    if (serviceInfo.Description == null)
                        serviceInfo.Description = "No description available.";

                    SaveService(serviceInfo);
                }
                
                serviceInfo.Initialize(this);

                result.Add(serviceInfo);
            }

            return result;
        }

        public async Task<ICollection<CharacteristicInfo>> GetAllCharacteristicsInfo(ServiceInfo serviceInfo, IEnumerable<GattCharacteristic> characteristics)
        {
            var result = new List<CharacteristicInfo>();
            var allSaved = GetAllSavedCharacteristics(serviceInfo);
            foreach (var gattCharacteristic in characteristics)
            {
                var characteristicInfo = allSaved.FirstOrDefault(x => x.Uuid == gattCharacteristic.Uuid);

                if (characteristicInfo == null)
                {
                    characteristicInfo = new CharacteristicInfo();
                    characteristicInfo.Service = serviceInfo;
                    characteristicInfo.Uuid = gattCharacteristic.Uuid;
                    characteristicInfo.ReadDisplayFormat = BytesDisplayFormat.Decimal;
                    characteristicInfo.WriteDisplayFormat = BytesDisplayFormat.Auto;

                    var gattInfo = _gattInformationProvider.GetCharacteristicInfo(serviceInfo.Uuid, characteristicInfo.Uuid);
                    if (gattInfo != null)
                    {
                        characteristicInfo.Name = SelectName(gattInfo.Name, gattInfo.OriginalName);
                        characteristicInfo.Description = SelectDescription(gattInfo.Description, gattInfo.Summary, gattInfo.Abstract);
                        characteristicInfo.WebLink = gattInfo.Link;
                    }

                    if (characteristicInfo.Name == null)
                        characteristicInfo.Name = "Unknown characteristic";

                    if (characteristicInfo.Description == null)
                        characteristicInfo.Description = "No description available.";

                    SaveCharacteristic(characteristicInfo);
                }
                
                // not a db property
                characteristicInfo.Properties = gattCharacteristic.CharacteristicProperties;
                characteristicInfo.AttributeHandle = gattCharacteristic.AttributeHandle;
                characteristicInfo.ProtectionLevel = gattCharacteristic.ProtectionLevel;

                // ToDo: add presentation formats

                characteristicInfo.Initialize(this);
                result.Add(characteristicInfo);
            }

            return result;
        }
        
        public void SaveDevice(DeviceInfo deviceInfo)
        {
            lock (_dbLock)
            {
                using (var db = new LiteDatabase(_dataBaseFilePath))
                {
                    var devices = db.GetCollection<DeviceInfo>();
                    devices.Upsert(deviceInfo);
                }
            }
        }

        public void SaveService(ServiceInfo serviceInfo)
        {
            lock (_dbLock)
            {
                using (var db = new LiteDatabase(_dataBaseFilePath))
                {
                    var devices = db.GetCollection<ServiceInfo>();
                    devices.Upsert(serviceInfo);
                }
            }
        }

        public void SaveCharacteristic(CharacteristicInfo characteristicInfo)
        {
            lock (_dbLock)
            {
                using (var db = new LiteDatabase(_dataBaseFilePath))
                {
                    var devices = db.GetCollection<CharacteristicInfo>();
                    devices.Upsert(characteristicInfo);
                }
            }
        }

        public void ForgetDevice(DeviceInfo deviceInfo)
        {
            // need to write a smarter query here
            var deleted = 0;
            lock (_dbLock)
            {
                var services = GetAllSavedServices(deviceInfo);
                foreach (var serviceInfo in services)
                {
                    var characteristics = GetAllSavedCharacteristics(serviceInfo);
                    using (var db = new LiteDatabase(_dataBaseFilePath))
                    {
                        var collection = db.GetCollection<CharacteristicInfo>();
                        foreach (var characteristic in characteristics)
                        {
                            if (collection.Delete(characteristic.Id))
                                deleted++;
                        }

                        if (db.GetCollection<ServiceInfo>().Delete(serviceInfo.Id))
                            deleted++;
                    }
                }

                using (var db = new LiteDatabase(_dataBaseFilePath))
                {
                    if (db.GetCollection<DeviceInfo>().Delete(deviceInfo.Id))
                        deleted++;
                }
            }
        }

        private ICollection<DeviceInfo> GetAllSavedDevices()
        {
            lock (_dbLock)
            {
                using (var db = new LiteDatabase(_dataBaseFilePath))
                {
                    var devices = db.GetCollection<DeviceInfo>();
                    return devices.FindAll().ToList();
                }
            }
        }
        
        private ICollection<ServiceInfo> GetAllSavedServices(DeviceInfo deviceInfo)
        {
            lock (_dbLock)
            {
                using (var db = new LiteDatabase(_dataBaseFilePath))
                {
                    var services = db.GetCollection<ServiceInfo>()
                        .Include(x => x.Device)
                        .Find(x => x.Device.Id == deviceInfo.Id);

                    return services.ToList();
                }
            }
        }

        private ICollection<CharacteristicInfo> GetAllSavedCharacteristics(ServiceInfo serviceInfo)
        {
            lock (_dbLock)
            {
                using (var db = new LiteDatabase(_dataBaseFilePath))
                {
                    var characteristics = db.GetCollection<CharacteristicInfo>()
                        .Include(x => x.Service)
                        .Include(x => x.Service.Device)
                        .Find(x => x.Service.Id == serviceInfo.Id);

                    return characteristics.ToList();
                }
            }
        }

        private string SelectName(params string[] options)
        {
            return options.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        }

        private string SelectDescription(params string[] options)
        {
            return options.Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x.Length).FirstOrDefault();
        }
    }
}