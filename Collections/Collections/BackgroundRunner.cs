using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Collections
{
    public class BackgroundRunner
    {
        private readonly BroadcastBlock<RunnerOutput> _broadcastToOutput;
        private readonly BroadcastBlock<CompiledResultsMessage> _broadcastToConsume;
        private TimeSpan? _scanFrequancy;
        private Action<CompiledResultsMessage> _action;
        private CancellationTokenSource _cancellationTokenSource;
        public BackgroundRunner(BroadcastBlock<RunnerOutput> broadcastToOutput,
            BroadcastBlock<CompiledResultsMessage> broadcastToConsume)
        {
            _broadcastToConsume = broadcastToConsume;
            _broadcastToOutput = broadcastToOutput;
        }

        public void Start(
            Action<CompiledResultsMessage> action = null,
            TimeSpan? scanFrequency = null)
        {
            
            _scanFrequancy = scanFrequency ?? TimeSpan.FromMilliseconds(100);
            _action = action ?? new Action<CompiledResultsMessage>(RunSimulation);

            _cancellationTokenSource = new CancellationTokenSource();

            var targetBlock = StartService(_action, _broadcastToConsume, _broadcastToOutput, _cancellationTokenSource.Token);
            targetBlock.Post("");
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
          Action<CompiledResultsMessage> action,
          BroadcastBlock<CompiledResultsMessage> broadcastBlock,
          BroadcastBlock<RunnerOutput> runnerBlock,
          CancellationToken cancellationToken)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            ActionBlock<string> block = null;

            block = new ActionBlock<string>(async now =>
            {

                while (await broadcastBlock.OutputAvailableAsync(cancellationToken))
                {
                    Thread.Sleep(_scanFrequancy.Value);
                    CompiledResultsMessage data = broadcastBlock.Receive();
                    action(data);
                    runnerBlock.Post(new RunnerOutput()
                    {
                        AvgExecutionTime = TimeSpan.FromMilliseconds(10023),
                       // Success = !data.CompilerErrors.HasErrors
                    });
                }

            }, new ExecutionDataflowBlockOptions
            {
                CancellationToken = cancellationToken
            });

            return block;
        }
        private void RunSimulation(CompiledResultsMessage msg)
        {
            if (msg.HasCompiled && msg.CompilerErrors.Count == 0)
            {
                Trace.WriteLine("running code from source " + msg.Source);
            }

        }
    }
}
