using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace BleLab.Commands
{
    public class CommandRunner
    {
        private readonly ConcurrentQueue<CommandBase> _commands = new ConcurrentQueue<CommandBase>();
        
        public event EventHandler<CommandRunnerEvent> CommandEnqueued;
        public event EventHandler<CommandRunnerEvent> CommandDispatched;
        public event EventHandler<CommandRunnerEvent> CommandExecuted; 

        public T EnqueueCommand<T>(T command) where T : CommandBase
        {
            _commands.Enqueue(command);
            CommandEnqueued?.Invoke(this, new CommandRunnerEvent(command));
            DispatchNext();

            return command;
        }

        private void DispatchNext()
        {
            if (_commands.Count == 0)
                return;

            if (!Monitor.TryEnter(_commands))
                return;

            try
            {
                CommandBase command;
                if (_commands.TryDequeue(out command))
                    Dispatch(command);
            }
            finally
            {
                Monitor.Exit(_commands);
            }
        }

        private void Dispatch(CommandBase command)
        {
            if (command.RunOnUiThread)
                Execute.OnUIThreadAsync(async () => await ExecuteAndDispatchNext(command));
            else
                Task.Run(async () => await ExecuteAndDispatchNext(command));

            CommandDispatched?.Invoke(this, new CommandRunnerEvent(command));
        }

        private async Task ExecuteAndDispatchNext(CommandBase command)
        {
            await command.ExecuteAsync().ConfigureAwait(false);

            CommandExecuted?.Invoke(this, new CommandRunnerEvent(command));

            DispatchNext();
        }
    }
}
