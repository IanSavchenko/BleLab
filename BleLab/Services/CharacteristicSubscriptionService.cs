using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using BleLab.Model;

namespace BleLab.Services
{
    public class CharacteristicSubscriptionService
    {
        private readonly DeviceController _deviceController;
        private readonly Dictionary<Guid, CharacteristicInfo> _subscribedCharacteristics = new Dictionary<Guid, CharacteristicInfo>();

        public CharacteristicSubscriptionService(DeviceController deviceController)
        {
            _deviceController = deviceController;
            _deviceController.DeviceDisconnecting += DeviceControllerOnDeviceDisconnecting;
        }

        public event TypedEventHandler<CharacteristicInfo, GattValueChangedEventArgs> ValueChanged; 

        public bool Subscribe(CharacteristicInfo characteristicInfo)
        {
            if (_subscribedCharacteristics.ContainsKey(characteristicInfo.Uuid))
                return false;

            var characteristic = GetCharacteristic(characteristicInfo);
            characteristic.ValueChanged += CharacteristicOnValueChanged;
            _subscribedCharacteristics.Add(characteristicInfo.Uuid, characteristicInfo);
            return true;
        }

        public bool Unsubscribe(CharacteristicInfo characteristicInfo)
        {
            if (!_subscribedCharacteristics.TryGetValue(characteristicInfo.Uuid, out characteristicInfo))
                return false;

            var characteristic = GetCharacteristic(characteristicInfo);
            characteristic.ValueChanged -= CharacteristicOnValueChanged;
            _subscribedCharacteristics.Remove(characteristic.Uuid);
            return true;
        }

        public bool IsSubscribed(GattCharacteristic characteristic)
        {
            return _subscribedCharacteristics.ContainsKey(characteristic.Uuid);
        }
        
        private void DeviceControllerOnDeviceDisconnecting(object sender, EventArgs eventArgs)
        {
            var deviceId = _deviceController.ConnectedDevice.DeviceId;
            var characteristics = _subscribedCharacteristics.Values.Where(t => t.Service.Device.DeviceId == deviceId).ToList();

            foreach (var characteristic in characteristics)
            {
                try
                {
                    Unsubscribe(characteristic);
                }
                catch
                {
                }
            }
        }

        private void CharacteristicOnValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            if (_subscribedCharacteristics.TryGetValue(sender.Uuid, out var info))
            {
                ValueChanged?.Invoke(info, args);
            }
        }

        private GattCharacteristic GetCharacteristic(CharacteristicInfo characteristicInfo)
        {
            return _deviceController.ConnectedDevice.GetGattService(characteristicInfo.Service.Uuid).GetCharacteristics(characteristicInfo.Uuid)[0];
        }
    }
}
