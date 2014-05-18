using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Collections.Compiler;

namespace Collections.Runtime
{
    public class RunnerService : IDisposable
    {
        private readonly BroadcastBlock<RunnerServiceMessage> _broadcastToOutput;
        private readonly BroadcastBlock<CompilerServiceMessage> _broadcastToConsume;
        private TimeSpan? _executionInterval;
        private CancellationTokenSource _cancellationTokenSource;

        public bool IsRunning
        {
            get;
            private set;
        }
        
        public RunnerService(BroadcastBlock<RunnerServiceMessage> broadcastToOutput,
            BroadcastBlock<CompilerServiceMessage> broadcastToConsume)
        {
            _broadcastToConsume = broadcastToConsume;
            _broadcastToOutput = broadcastToOutput;
            _executionInterval = TimeSpan.FromMilliseconds(1000);
        }

        public void Start(
            Action<CompilerServiceMessage> action = null,
            TimeSpan? executionInterval = null)
        {
            if (IsRunning)
            {
                return;
            }

            IsRunning = true;
            _executionInterval = executionInterval ?? _executionInterval;
            action = action ?? RunnerAction;

            _cancellationTokenSource = new CancellationTokenSource();

            var targetBlock = StartService(action, _broadcastToConsume, _broadcastToOutput, _cancellationTokenSource.Token);
            targetBlock.Post(String.Empty);
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

      

        public TimeSpan ExecutionInterval
        {
            set { _executionInterval = value; }
        }

        ITargetBlock<string> StartService(
              Action<CompilerServiceMessage> action,
              BroadcastBlock<CompilerServiceMessage> broadcastBlock,
              BroadcastBlock<RunnerServiceMessage> runnerBlock,
              CancellationToken cancellationToken)
        {

            ActionBlock<string> block = null;
            block = new ActionBlock<string>(async now =>
            {
                
                while (  await broadcastBlock.OutputAvailableAsync(cancellationToken))
                {
                    CompilerServiceMessage message = broadcastBlock.Receive();
                    message = new CompilerServiceMessage(message, message.State);
                  
                    action(message);

                    runnerBlock.Post(new RunnerServiceMessage
                    {
                        State = message.State
                    });

                    try
                    {
                        broadcastBlock.Completion.Wait((int)_executionInterval.Value.TotalMilliseconds,cancellationToken);
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
   
        private void RunnerAction(CompilerServiceMessage msg)
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
