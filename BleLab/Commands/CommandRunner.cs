using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Action = System.Action;

namespace BleLab.Commands
{
    public class CommandRunner
    {
        private readonly Func<Action, Task> _executeUiThreadFunc;
        private readonly ConcurrentQueue<CommandBase> _commands = new ConcurrentQueue<CommandBase>();

        private int _executing = 0;
        
        public event EventHandler<CommandRunnerEvent> Enqueued;
        public event EventHandler<CommandRunnerEvent> Dispatched;
        public event EventHandler<CommandRunnerEvent> Executed;


        public CommandRunner()
        {
            _executeUiThreadFunc = Execute.OnUIThreadAsync;
        }


        public CommandRunner(Func<Action, Task> executeUiThreadFunc = null) : this()
        {
            if (executeUiThreadFunc != null)
            {
                _executeUiThreadFunc = executeUiThreadFunc;
            }
        }


        public T Enqueue<T>(T command) where T : CommandBase
        {
            _commands.Enqueue(command);

            Enqueued?.Invoke(this, new CommandRunnerEvent(command));

            DispatchNext();

            return command;
        }


        private async void DispatchNext()
        {
            if (_commands.Count == 0)
                return;

            if (Interlocked.Exchange(ref _executing, 1) != 0)
                return;

            try
            {
                if (_commands.TryDequeue(out var command))
                    await Dispatch(command).ConfigureAwait(false);
            }
            finally
            {
                Interlocked.Exchange(ref _executing, 0);
            }

            DispatchNext();
        }


        private async Task Dispatch(CommandBase command)
        {
            Dispatched?.Invoke(this, new CommandRunnerEvent(command));

            if (command.RunOnUiThread)
                await _executeUiThreadFunc(async () => await ExecuteOne(command).ConfigureAwait(false)).ConfigureAwait(false);
            else
                await Task.Run(() => ExecuteOne(command)).ConfigureAwait(false);
        }


        private async Task ExecuteOne(CommandBase command)
        {
            await command.ExecuteAsync().ConfigureAwait(false);

            Executed?.Invoke(this, new CommandRunnerEvent(command));
        }
    }
}
