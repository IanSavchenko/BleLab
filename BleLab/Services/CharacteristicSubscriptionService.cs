using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;

namespace BleLab.Services
{
    public class CharacteristicSubscriptionService
    {
        private readonly Dictionary<Guid, GattCharacteristic> _subscribedCharacteristics = new Dictionary<Guid, GattCharacteristic>();

        public event TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> ValueChanged; 

        public bool Subscribe(GattCharacteristic characteristic)
        {
            if (_subscribedCharacteristics.ContainsKey(characteristic.Uuid))
                return false;

            characteristic.ValueChanged += CharacteristicOnValueChanged;
            _subscribedCharacteristics.Add(characteristic.Uuid, characteristic);
            return true;
        }

        public bool Unsubscribe(GattCharacteristic characteristic)
        {
            if (!_subscribedCharacteristics.TryGetValue(characteristic.Uuid, out characteristic))
                return false;

            characteristic.ValueChanged -= CharacteristicOnValueChanged;
            _subscribedCharacteristics.Remove(characteristic.Uuid);
            return true;
        }

        public bool IsSubscribed(GattCharacteristic characteristic)
        {
            return _subscribedCharacteristics.ContainsKey(characteristic.Uuid);
        }

        public void DeviceDisconnected(BluetoothLEDevice device)
        {
            if (device == null)
                return;

            var deviceId = device.DeviceId;
            var characteristics = _subscribedCharacteristics.Values.Where(t => t.Service.DeviceId == deviceId).ToList();

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
            ValueChanged?.Invoke(sender, args);
        }
    }
}
