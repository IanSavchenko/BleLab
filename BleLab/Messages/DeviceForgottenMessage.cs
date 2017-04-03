using BleLab.Model;

namespace BleLab.Messages
{
    public class DeviceForgottenMessage
    {
        public DeviceForgottenMessage(DeviceInfo device)
        {
            Device = device;
        }

        public DeviceInfo Device { get; }
    }
}
