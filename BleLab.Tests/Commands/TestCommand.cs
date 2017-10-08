using System;
using System.Threading.Tasks;
using BleLab.Commands;

namespace BleLab.Tests.Commands
{
    public class TestCommand : CommandBase
    {
        public Action ExecuteAction { get; set; }

        public void SetException(Exception ex)
        {
            Exception = ex;
        }

        public void SetStatus(CommandStatus status)
        {
            Status = status;
        }

        protected override async Task DoExecuteAsync()
        {
            ExecuteAction?.Invoke();
        }
    }
}
