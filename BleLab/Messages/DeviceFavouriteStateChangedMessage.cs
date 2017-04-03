using BleLab.Model;

namespace BleLab.Messages
{
    internal class DeviceFavouriteStateChangedMessage
    {
        public DeviceFavouriteStateChangedMessage(DeviceInfo device)
        {
            Device = device;
        }

        public DeviceInfo Device { get; private set; }
    }
}

