using Windows.ApplicationModel;

namespace BleLab.Messages
{
    internal class SuspendStateMessage
    {
        public SuspendStateMessage(SuspendingOperation operation)
        {
            Operation = operation;
        }

        public SuspendingOperation Operation { get; }
    }
}
