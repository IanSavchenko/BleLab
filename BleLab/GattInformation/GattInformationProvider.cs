using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Windows.ApplicationModel;
using BleLab.Utils;
using Newtonsoft.Json;

namespace BleLab.GattInformation
{
    public class GattInformationProvider
    {
        private readonly List<GattInfo> _infoObjects = new List<GattInfo>();

        private readonly ManualResetEvent _initializedEvent = new ManualResetEvent(false);
        private bool _initialized = false;

        public async void Initialize()
        {
            try
            {
                if (_initialized)
                    return;

                var folder = await Package.Current.InstalledLocation.GetFolderAsync("GattInformation");
                var file = await folder.GetFileAsync("bluetooth_gatt.json");
                var text = await Windows.Storage.FileIO.ReadTextAsync(file);

                var deserializedObjects = JsonConvert.DeserializeObject<IList<GattInfo>>(text);
                _infoObjects.AddRange(deserializedObjects);
            }
            catch (Exception e)
            {
                // shouldn't happen...
                Debug.WriteLine(e);
            }
            finally
            {
                _initialized = true;
                _initializedEvent.Set();
            }
        }

        public GattInfo GetServiceInfo(Guid uuid)
        {
            WaitInitialized();

            if (!uuid.IsStandartGattUuid())
                return null;

            var standartUuid = uuid.GetStandartGattAssignedNumber();
            return _infoObjects.FirstOrDefault(x => x.Type == GattInfoType.Service && x.Uuid.Equals(standartUuid, StringComparison.OrdinalIgnoreCase));
        }

        public GattInfo GetCharacteristicInfo(Guid serviceUuid, Guid gattCharacteristicUuid)
        {
            WaitInitialized();

            var service = GetServiceInfo(serviceUuid);
            if (service == null)
                return null;

            var standartUuid = gattCharacteristicUuid.GetStandartGattAssignedNumber();
            return service.Children.FirstOrDefault(x => x.Type == GattInfoType.Characteristic && x.Uuid.Equals(standartUuid, StringComparison.OrdinalIgnoreCase));
        }

        private void WaitInitialized()
        {
            if (_initialized)
                return;

            _initializedEvent.WaitOne();
        }
    }
}
