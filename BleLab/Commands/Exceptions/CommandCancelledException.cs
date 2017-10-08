using System;

namespace BleLab.Commands.Exceptions
{
    public class CommandCancelledException : Exception
    {
        public CommandCancelledException() : base("Command cancelled.")
        {
        }
    }
}