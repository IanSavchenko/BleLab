using System;
using System.Threading.Tasks;
using BleLab.Commands;

namespace BleLab.Tests.Commands
{
    public class TestCommand : CommandBase
    {
        public Action ExecuteAction { get; set; }

        protected override async Task DoExecuteAsync()
        {
            ExecuteAction?.Invoke();
        }
    }
}
