namespace BleLab.Commands.Exceptions
{
    public class DeviceUnreachableException : CommandException
    {
        public DeviceUnreachableException() : base("Device unreachable.")
        {
        }
    }
}
