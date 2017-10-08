using System;
using System.Threading.Tasks;

namespace BleLab.Commands
{
    public abstract class CommandBase
    {
        private readonly TaskCompletionSource<CommandStatus> _taskCompletionSource = new TaskCompletionSource<CommandStatus>();
        private CommandStatus _status = CommandStatus.None;
        private Exception _exception;

        public event EventHandler Completed;
        
        public bool RunOnUiThread { get; set; }

        public bool HideInConsole { get; set; }

        public Exception Exception
        {
            get => _exception;
            protected set
            {
                if (_exception != null)
                    return;

                _exception = value;
            }
        }

        public CommandStatus Status
        {
            get => _status;
            protected set
            {
                // if result already set, we should not change it
                if (_status != CommandStatus.None)
                    return;

                _status = value;
                Completed?.Invoke(this, EventArgs.Empty);

                _taskCompletionSource.TrySetResult(_status);
            }
        }
        
        public async Task ExecuteAsync()
        {
            try
            {
                if (Status != CommandStatus.None)
                    return;

                await DoExecuteAsync().ConfigureAwait(false);
                Status = CommandStatus.Succeeded;
            }
            catch (Exception ex)
            {
                if (Exception == null)
                    Exception = ex;

                Status = CommandStatus.Exception;
            }
        }

        public TaskCompletionSource<CommandStatus> GetTaskCompletionSource()
        {
            return _taskCompletionSource;
        }
        
        protected abstract Task DoExecuteAsync();
    }
}
