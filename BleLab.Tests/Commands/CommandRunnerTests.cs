using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BleLab.Commands;
using Xunit;

namespace BleLab.Tests.Commands
{
    public class CommandRunnerTests
    {
        private readonly CommandRunner _instance = new CommandRunner();
        
        [Fact]
        public async void Enqueue_WhenCalled_ShouldExecuteCommand()
        {
            var invoked = false;

            // Prepare
            var command = new TestCommand
            {
                ExecuteAction = () => invoked = true
            };

            // Execute
            _instance.Enqueue(command);

            // Wait
            await command.AsTask().ConfigureAwait(true);

            // Verify
            Assert.True(invoked);
        }

        [Fact]
        public async void Enqueue_WhenCalledWithUiThreadCommand_ShouldExecuteOnUiThread()
        {
            var invoked = false;
            var customUiThreadInstance = new CommandRunner(action =>
            {
                invoked = true;
                action();
                return Task.FromResult(0);
            });

            // Prepare
            var command = new TestCommand
            {
                RunOnUiThread = true
            };

            // Execute
            customUiThreadInstance.Enqueue(command);

            // Wait
            await command.AsTask().ConfigureAwait(true);

            // Verify
            Assert.True(invoked);
        }

        [Fact]
        public async void Enqueue_WhenInvokedForMultiple_ShouldExecuteSequentially()
        {
            var resultSequence = new List<int>();

            // Prepare
            var cmd1 = new TestCommand
            {
                ExecuteAction = () =>
                {
                    resultSequence.Add(0);
                    Task.Delay(1000).Wait();
                    resultSequence.Add(1);
                }
            };

            var cmd2 = new TestCommand
            {
                ExecuteAction = () =>
                {
                    resultSequence.Add(2);
                    Task.Delay(1000).Wait();
                    resultSequence.Add(3);
                }
            };

            // Execute
            _instance.Enqueue(cmd1);
            _instance.Enqueue(cmd2);

            // Wait
            await Task.WhenAll(
                cmd1.AsTask(), 
                cmd2.AsTask())
            .ConfigureAwait(true);
            
            // Verify
            Assert.True(resultSequence.Count == 4);
            resultSequence.Aggregate((a, b) =>
            {
                Assert.True(b == a + 1);
                return b;
            });
        }

        [Fact]
        public async void Events_OnEnequeue_ShouldBeInvokedAll()
        {
            var resultSequence = new List<string>();

            // Prepare
            _instance.Enqueued += (sender, e) =>
            {
                resultSequence.Add(nameof(CommandRunner.Enqueued));
            };

            _instance.Dispatched += (sender, e) =>
            {
                resultSequence.Add(nameof(CommandRunner.Dispatched));
            };

            _instance.Executed += (sender, e) =>
            {
                resultSequence.Add(nameof(CommandRunner.Executed));
            };

            // Execute
            var command = new TestCommand();
            _instance.Enqueue(command);

            // Wait
            await command.AsTask().ConfigureAwait(true);

            await Task.Delay(100).ConfigureAwait(true); // extra wait to ensure command cycle completed

            // Verify
            Assert.True(resultSequence.Count == 3);
            Assert.Contains(nameof(CommandRunner.Enqueued), resultSequence);
            Assert.Contains(nameof(CommandRunner.Dispatched), resultSequence);
            Assert.Contains(nameof(CommandRunner.Executed), resultSequence);
        }

        [Fact]
        public async void Events_OnEnequeue_ShouldHaveCorrectArgs()
        {
            var command = new TestCommand();
            
            _instance.Enqueued += (sender, e) =>
            {
                Assert.True(sender == _instance);
                Assert.True(e.Command == command);
            };

            _instance.Dispatched += (sender, e) =>
            {
                Assert.True(sender == _instance);
                Assert.True(e.Command == command);
            };

            _instance.Executed += (sender, e) =>
            {
                Assert.True(sender == _instance);
                Assert.True(e.Command == command);
            };

            // Execute
            _instance.Enqueue(command);

            // Wait
            await command.AsTask().ConfigureAwait(true);
            await Task.Delay(100).ConfigureAwait(true); // extra wait to ensure command cycle completed
        }
        
        [Fact]
        public async void Events_OnEnequeue_ShouldBeInvokedInCorrectOrder()
        {
            var resultSequence = new List<string>();
            var tcs = new TaskCompletionSource<bool>();

            // Prepare
            var command = new TestCommand
            {
                ExecuteAction = () =>
                {
                    tcs.Task.Wait();
                }
            };

            var enqueued = 1;
            _instance.Enqueued += (sender, e) =>
            {
                resultSequence.Add(nameof(CommandRunner.Enqueued) + enqueued++);
            };

            var dispatched = 1;
            _instance.Dispatched += (sender, e) =>
            {
                resultSequence.Add(nameof(CommandRunner.Dispatched) + dispatched++);
            };

            var executed = 1;
            _instance.Executed += (sender, e) =>
            {
                resultSequence.Add(nameof(CommandRunner.Executed) + executed++);
            };

            // Execute
            _instance.Enqueue(command);
            _instance.Enqueue(command);

            // Let run
            tcs.SetResult(true);

            // Wait
            await command.AsTask().ConfigureAwait(true);

            await Task.Delay(100).ConfigureAwait(true); // extra wait to ensure command cycle completed

            // Verify
            Assert.True(resultSequence.Count == 6);
            Assert.True(resultSequence[0] == nameof(CommandRunner.Enqueued) + "1");
            Assert.True(resultSequence[1] == nameof(CommandRunner.Dispatched) + "1");
            Assert.True(resultSequence[2] == nameof(CommandRunner.Enqueued) + "2");
            Assert.True(resultSequence[3] == nameof(CommandRunner.Executed) + "1");
            Assert.True(resultSequence[4] == nameof(CommandRunner.Dispatched) + "2");
            Assert.True(resultSequence[5] == nameof(CommandRunner.Executed) + "2");
        }
    }
}
