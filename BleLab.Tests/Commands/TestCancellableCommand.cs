using System;
using System.Threading;
using System.Threading.Tasks;
using BleLab.Commands;

namespace BleLab.Tests.Commands
{
    public class TestCancellableCommand : CancellableCommandBase
    {
        public Action ExecuteAction { get; set; }

        public CancellationToken GetToken()
        {
            return this.CancellationToken;
        }

        protected override Task DoExecuteAsync()
        {
            return Task.Run(() => ExecuteAction?.Invoke());
        }
    }
}
