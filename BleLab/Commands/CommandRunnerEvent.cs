using System;

namespace BleLab.Commands
{
    public class CommandRunnerEvent : EventArgs
    {
        public CommandRunnerEvent(CommandBase command)
        {
            Command = command;
        }

        public CommandBase Command { get; private set; }
    }
}