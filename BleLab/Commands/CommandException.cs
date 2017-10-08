using System;

namespace BleLab.Commands
{
    public class CommandException : Exception
    {
        public CommandException()
        {
        }

        public CommandException(string message) : base(message)
        {
        }
    }
}
