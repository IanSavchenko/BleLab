using System;
using BleLab.Commands;
using Xunit;

namespace BleLab.Tests.Commands
{
    public class CommandBaseTests 
    {
        [Fact]
        public async void Completed_WheStatusChanges_ShouldBeInvoked()
        {
            var invoked = false;

            // Prepare
            var command = new TestCommand();
            command.Completed += (sender, args) => invoked = true;

            // Execute
            await command.ExecuteAsync().ConfigureAwait(true);

            // Verify
            Assert.True(invoked);
        }

        [Fact]
        public async void ExceptionProperty_WhenSet_ShouldNotBeChanged()
        {
            var ex1 = new Exception();
            var ex2 = new Exception();

            // Prepare
            var command = new TestCommand();
            command.ExecuteAction = () =>
            {
                command.SetException(ex1);
                throw ex2;
            };

            // Execute
            await command.ExecuteAsync().ConfigureAwait(true);

            // Verify
            Assert.True(command.Exception == ex1);
        }

        [Fact]
        public async void StatusProperty_WhenSet_ShouldNotBeChanged()
        {
            // Prepare
            var command = new TestCommand();
            command.ExecuteAction = () =>
            {
                command.SetStatus(CommandStatus.Exception);
            };

            // Execute
            await command.ExecuteAsync().ConfigureAwait(true);

            // Verify
            Assert.True(command.Status == CommandStatus.Exception);
        }

        [Fact]
        public async void ExecuteAsync_WhenCalled_ShouldInvokeDoExecuteAsync()
        {
            // Prepare
            var invoked = false;

            var command = new TestCommand();
            command.ExecuteAction = () =>
            {
                invoked = true;
            };

            // Execute
            await command.ExecuteAsync().ConfigureAwait(true);

            // Verify
            Assert.True(invoked);
        }

        [Fact]
        public async void ExecuteAsync_WhenCalledForCompletedCommand_ShouldNotInvokeDoExecuteAsync()
        {
            // Prepare
            var invoked = 0;

            var command = new TestCommand();
            command.ExecuteAction = () =>
            {
                invoked++;
            };

            // Execute
            await command.ExecuteAsync().ConfigureAwait(true);
            await command.ExecuteAsync().ConfigureAwait(true);

            // Verify
            Assert.True(invoked == 1);
        }

        [Fact]
        public async void ExecuteAsync_WhenDoExecuteThrows_ShouldSetExceptionAndStatus()
        {
            // Prepare
            var exception = new Exception();

            var command = new TestCommand();
            command.ExecuteAction = () => throw exception;

            // Execute
            await command.ExecuteAsync().ConfigureAwait(true);

            // Verify
            Assert.True(command.Status == CommandStatus.Exception);
            Assert.True(command.Exception == exception);
        }

        [Fact]
        public async void GetStatusAsync_Always_ShouldReturnTaskWithStatus()
        {
            // Prepare
            var command = new TestCommand();
            var task = command.GetStatusAsync();
            
            // Execute
            await command.ExecuteAsync().ConfigureAwait(true);

            var status = await task.ConfigureAwait(true);

            // Verify
            Assert.True(status == CommandStatus.Succeeded);
        }
    }
}
