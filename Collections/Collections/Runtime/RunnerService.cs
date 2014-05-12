using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Collections.Compiler;

namespace Collections.Runtime
{
    public class RunnerService
    {
        private readonly BroadcastBlock<RunnerServiceMessage> _broadcastToOutput;
        private readonly BroadcastBlock<CompilerServiceMessage> _broadcastToConsume;
        private TimeSpan? _executionFrequency;
        private CancellationTokenSource _cancellationTokenSource;
        public RunnerService(BroadcastBlock<RunnerServiceMessage> broadcastToOutput,
            BroadcastBlock<CompilerServiceMessage> broadcastToConsume)
        {
            _broadcastToConsume = broadcastToConsume;
            _broadcastToOutput = broadcastToOutput;
            _executionFrequency = TimeSpan.FromMilliseconds(2000);
        }

        public void Start(
            Action<CompilerServiceMessage> action = null,
            TimeSpan? executionFrequency = null)
        {
            _executionFrequency = executionFrequency ?? _executionFrequency;
            action = action ?? RunnerAction;

            _cancellationTokenSource = new CancellationTokenSource();

            var targetBlock = StartService(action, _broadcastToConsume, _broadcastToOutput, _cancellationTokenSource.Token);
            targetBlock.Post(String.Empty);
        }


        public void Stop()
        {
            using (_cancellationTokenSource)
            {
                _cancellationTokenSource.Cancel();
            }

            _cancellationTokenSource = null;
            
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

                while (await broadcastBlock.OutputAvailableAsync(cancellationToken))
                {
                    Thread.Sleep(_executionFrequency.Value);
                    CompilerServiceMessage message = broadcastBlock.Receive();
                    message = new CompilerServiceMessage(message, message.State);
                  
                    action(message);

                    runnerBlock.Post(new RunnerServiceMessage
                    {
                        State = message.State
                    });

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
    }
}
