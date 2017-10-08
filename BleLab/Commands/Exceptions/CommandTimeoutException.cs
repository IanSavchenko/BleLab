using System;

namespace BleLab.Commands.Exceptions
{
    public class CommandTimeoutException : Exception
    {
        public CommandTimeoutException() : base("Command timeout.")
        {
        }
    }
}