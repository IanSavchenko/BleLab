using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using BleLab.Model;
using BleLab.Services;
using Caliburn.Micro;

namespace BleLab.Commands.Characteristic
{
    public abstract class CharacteristicCommandBase : CommandBase
    {
        private DeviceController _deviceController;

        protected CharacteristicCommandBase(CharacteristicInfo characteristicInfo)
        {
            CharacteristicInfo = characteristicInfo;
            _deviceController = IoC.Get<DeviceController>();
        }

        public CharacteristicInfo CharacteristicInfo { get; private set; }

        protected GattCharacteristic Characteristic
        {
            get
            {
                if (_deviceController.ConnectedDevice == null)
                    throw new InvalidOperationException("Device not connected");

                if (_deviceController.ConnectedDevice.DeviceId != CharacteristicInfo.Service.Device.DeviceId)
                    throw new InvalidOperationException("Targeted device not connected");

                return _deviceController.ConnectedDevice.GetGattService(CharacteristicInfo.Service.Uuid).GetCharacteristics(CharacteristicInfo.Uuid)[0];
            }
        }
    }
}