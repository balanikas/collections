using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Collections.Runtime;

namespace Collections.Compiler
{
    public class CompilerService : IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource;
        private ITargetBlock<CompilerServiceMessage> _targetBlock;
        private readonly BroadcastBlock<CompilerServiceMessage> _broadcaster;
        private TimeSpan? _executionInterval;

        public bool IsRunning
        {
            get;
            private set;
        }

        public CompilerService(BroadcastBlock<CompilerServiceMessage> broadcaster)
        {
            _broadcaster = broadcaster;
            _executionInterval = TimeSpan.FromMilliseconds(1000);
        }

        public void Start(Action<CompilerServiceMessage> action = null, 
            TimeSpan? executionInterval = null)
        {

            if (IsRunning)
            {
                return;
            }

            IsRunning = true;

            _executionInterval = executionInterval ?? _executionInterval;

            action = action ?? CompilerAction;
            _cancellationTokenSource = new CancellationTokenSource();

            _targetBlock = StartService(action, _broadcaster, _cancellationTokenSource.Token);
            _targetBlock.Post(new CompilerServiceMessage());
        }



        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }
            IsRunning = false;

            _cancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
            
        }

        public TimeSpan ExecutionInterval {
            set
            {
                _executionInterval = value;
            }
        }

        ITargetBlock<CompilerServiceMessage> StartService(
            Action<CompilerServiceMessage> action, 
            BroadcastBlock<CompilerServiceMessage> broadcaster, 
            CancellationToken cancellationToken)
        {
           
            ActionBlock<CompilerServiceMessage> block = null;
            string previousSource = null;


            block = new ActionBlock<CompilerServiceMessage>(async message =>
            {

                while (await broadcaster.OutputAvailableAsync(cancellationToken))
                {
                  
                    CompilerServiceMessage receivedMessage = broadcaster.Receive();
                    if (previousSource != receivedMessage.Source)
                    {
                        action(receivedMessage);
                        previousSource = receivedMessage.Source;
                        block.Post(receivedMessage);
                        if (receivedMessage.CompilerErrors.Count == 0)
                        {
                            broadcaster.Post(receivedMessage);
                        }
                    }

                    try
                    {
                        broadcaster.Completion.Wait((int)_executionInterval.Value.TotalMilliseconds, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                    }

                }

            }, new ExecutionDataflowBlockOptions
            {
                CancellationToken = cancellationToken
            });

            return block;
        }

        private void CompilerAction(CompilerServiceMessage msg)
        {
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }
            }
        }
    }
}
