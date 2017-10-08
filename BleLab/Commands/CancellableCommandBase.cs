using System;
using System.Threading;
using System.Threading.Tasks;
using BleLab.Commands.Exceptions;

namespace BleLab.Commands
{
    public abstract class CancellableCommandBase : CommandBase
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        protected CancellableCommandBase()
        {
            CancellationToken = _cts.Token;
            CancellationToken.Register(OnCancellation);
        }

        public TimeSpan Timeout { get; set; }

        protected CancellationToken CancellationToken { get; }

        public void Cancel()
        {
            _cts.Cancel();
        }

        public override Task ExecuteAsync()
        {
            if (Timeout != default(TimeSpan))
            {
                Task.Delay(Timeout, CancellationToken).ContinueWith(_ => OnTimeout(), CancellationToken);
            }

            return base.ExecuteAsync();
        }

        protected virtual void OnCancellation()
        {
            Exception = new CommandCancelledException();
            Status = CommandStatus.Exception;
        }

        protected virtual void OnTimeout()
        {
            Exception = new CommandTimeoutException();
            Cancel();
        }
    }
}
