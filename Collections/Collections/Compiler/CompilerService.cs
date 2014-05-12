using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Collections.Runtime;

namespace Collections.Compiler
{
    public class CompilerService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private ITargetBlock<CompilerServiceMessage> _targetBlock;
        private readonly BroadcastBlock<CompilerServiceMessage> _broadcastToOutput;
        private TimeSpan? _executionFrequency;
        public CompilerService(BroadcastBlock<CompilerServiceMessage> broadcastToOutput)
        {
            _broadcastToOutput = broadcastToOutput;
            _executionFrequency = TimeSpan.FromMilliseconds(1000);
        }

        public void Start(Action<CompilerServiceMessage> action = null, 
            TimeSpan? executionFrequency = null)
        {
            _executionFrequency = executionFrequency ?? _executionFrequency;

            action = action ?? CompilerAction;
            _cancellationTokenSource = new CancellationTokenSource();

            _targetBlock = StartService(action, _broadcastToOutput, _cancellationTokenSource.Token);
            _targetBlock.Post(new CompilerServiceMessage());
        }

        public void Stop()
        {
            using (_cancellationTokenSource)
            {
                _cancellationTokenSource.Cancel();
            }

            _cancellationTokenSource = null;
            _targetBlock = null;
        }

        ITargetBlock<CompilerServiceMessage> StartService(
            Action<CompilerServiceMessage> action, 
            BroadcastBlock<CompilerServiceMessage> broadcastBlock, 
            CancellationToken cancellationToken)
        {
           
            ActionBlock<CompilerServiceMessage> block = null;
            string previousSource = null;


            block = new ActionBlock<CompilerServiceMessage>(async now =>
            {

                while (await broadcastBlock.OutputAvailableAsync(cancellationToken))
                {
                    Thread.Sleep(_executionFrequency.Value);
                    CompilerServiceMessage data = broadcastBlock.Receive();
                    if (previousSource != data.Source)
                    {
                        action(data);
                        previousSource = data.Source;
                        block.Post(data);
                        if (data.CompilerErrors.Count == 0)
                        {
                            broadcastBlock.Post(data);
                        }
                    }

                }

            }, new ExecutionDataflowBlockOptions
            {
                CancellationToken = cancellationToken
            });


            //block = new ActionBlock<CompilerServiceMessage>(async now =>
            //{
            //    await Task.Delay(_executionFrequency.Value, cancellationToken).ConfigureAwait(false);

            //    CompilerServiceMessage data = broadcastBlock.Receive();
            //    if (previousSource != data.Source)
            //    {
            //        action(data);
            //        previousSource = data.Source;
            //        block.Post(data);
            //        if (data.CompilerErrors.Count == 0)
            //        {
            //            broadcastBlock.Post(data);
            //        }
            //    }

            //}, new ExecutionDataflowBlockOptions
            //{
            //    CancellationToken = cancellationToken
            //});

            return block;
        }

        private void CompilerAction(CompilerServiceMessage msg)
        {
        }
    }
}
