using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace WpfClient
{
    internal class NeverEndingDataFlowTplService
    {
        private ITargetBlock<DateTimeOffset> _task;
        private CancellationTokenSource _wtoken;

        public void Start(Action<DateTimeOffset> action)
        {
            // Create the token source.
            _wtoken = new CancellationTokenSource();

            // Set the task.
            _task = CreateNeverEndingTask(action, _wtoken.Token);

            // Start the task.  Post the time.
            _task.Post(DateTimeOffset.Now);
        }

        public void Stop()
        {
            // CancellationTokenSource implements IDisposable.
            using (_wtoken)
            {
                // Cancel.  This will cancel the task.
                _wtoken.Cancel();
            }

            // Set everything to null, since the references
            // are on the class level and keeping them around
            // is holding onto invalid state.
            _wtoken = null;
            _task = null;
        }

        private ITargetBlock<DateTimeOffset> CreateNeverEndingTask(
            Action<DateTimeOffset> action, CancellationToken cancellationToken)
        {
            // Validate parameters.
            if (action == null) throw new ArgumentNullException("action");

            // Declare the block variable, it needs to be captured.
            ActionBlock<DateTimeOffset> block = null;

            // Create the block, it will call itself, so
            // you need to separate the declaration and
            // the assignment.
            // Async so you can wait easily when the
            // delay comes.
            block = new ActionBlock<DateTimeOffset>(async now =>
            {
                // Perform the action.
                action(now);

                // Wait.
                await Task.Delay(TimeSpan.FromMilliseconds(5000), cancellationToken).
                    // Doing this here because synchronization context more than
                    // likely *doesn't* need to be captured for the continuation
                    // here.  As a matter of fact, that would be downright
                    // dangerous.
                    ConfigureAwait(false);

                // Post the action back to the block.
                block.Post(DateTimeOffset.Now);
            }, new ExecutionDataflowBlockOptions
            {
                CancellationToken = cancellationToken
            });

            // Return the block.
            return block;
        }
    }
}