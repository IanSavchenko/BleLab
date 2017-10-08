using System;
using System.Threading.Tasks;
using BleLab.Commands;
using BleLab.Commands.Exceptions;
using Xunit;

namespace BleLab.Tests.Commands
{
    public class CancellableCommandBaseTests
    {
        [Fact]
        public async void Cancel_Always_ShouldChangeTokenState()
        {
            var cancelled = false;

            // Prepare
            var command = new TestCancellableCommand();
            command.ExecuteAction = () =>
            {
                var i = 0;
                while (i++ < 10)
                {
                    Task.Delay(100).Wait();
                    if (command.GetToken().IsCancellationRequested)
                    {
                        cancelled = true;
                        return;
                    }
                }
            };

            // Execute
            command.ExecuteAsync(); // command started

            await Task.Delay(200).ConfigureAwait(true); // running some time already

            command.Cancel(); // in the middle of Delay now

            await command.AsTask().ConfigureAwait(true);

            // Waiting for ExecuteAction to check token
            await Task.Delay(200).ConfigureAwait(true); 

            // Verify
            Assert.True(cancelled);
        }

        [Fact]
        public async void Timeout_WhenSet_ShouldCancelTaskWithException()
        {
            // Prepare
            var command = new TestCancellableCommand();
            command.Timeout = TimeSpan.FromMilliseconds(300);
            command.ExecuteAction = () =>
            {
                Task.Delay(1000).Wait();
            };

            // Execute
            await command.ExecuteAsync().ConfigureAwait(true);
            
            // Verify
            Assert.True(command.Status == CommandStatus.Exception);
            Assert.True(command.Exception is CommandTimeoutException);
        }

        [Fact]
        public async void Cancel_WhenCalled_ShouldCancelTaskWithException()
        {
            // Prepare
            var command = new TestCancellableCommand();
            command.ExecuteAction = () =>
            {
                Task.Delay(1000).Wait();
            };

            // Execute
            command.ExecuteAsync();
            command.Cancel();

            // Verify
            Assert.True(command.Status == CommandStatus.Exception);
            Assert.True(command.Exception is CommandCancelledException);
        }
    }
}
