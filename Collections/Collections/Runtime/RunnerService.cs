using System;
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
        private Action<CompilerServiceMessage> _action;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly IRuntime _runtime;
        public RunnerService(IRuntime runtime, BroadcastBlock<RunnerServiceMessage> broadcastToOutput,
            BroadcastBlock<CompilerServiceMessage> broadcastToConsume)
        {
            _runtime = runtime;
            _broadcastToConsume = broadcastToConsume;
            _broadcastToOutput = broadcastToOutput;
        }

        public void Start(
            Action<CompilerServiceMessage> action = null,
            TimeSpan? executionFrequency = null)
        {
            
            _runtime.Start();

            _executionFrequency = executionFrequency ?? TimeSpan.FromMilliseconds(1000);
            _action = action ?? new Action<CompilerServiceMessage>(Execute);

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
          Action<CompilerServiceMessage> action,
          BroadcastBlock<CompilerServiceMessage> broadcastBlock,
          BroadcastBlock<RunnerServiceMessage> runnerBlock,
          CancellationToken cancellationToken)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            ActionBlock<string> block = null;

            block = new ActionBlock<string>(async now =>
            {
                
                while (await broadcastBlock.OutputAvailableAsync(cancellationToken))
                {
                    Thread.Sleep(_executionFrequency.Value);
                    CompilerServiceMessage data = broadcastBlock.Receive();

                    var execTime = Utils.MeasureExecutionTime(action, data);
                  
                    runnerBlock.Post(new RunnerServiceMessage()
                    {
                        AvgExecutionTime = execTime,
                       // Success = !data.CompilerErrors.HasErrors
                    });
                }

            }, new ExecutionDataflowBlockOptions
            {
                CancellationToken = cancellationToken
            });

            return block;
        }
        private void Execute(CompilerServiceMessage msg)
        {
           
            if (msg.CompilerErrors.Count == 0 && msg.Types.Any())
            {
                var methods = msg.Types[0].MethodsInfos.Where(x => x.DeclaringType == msg.Types[0].TypeInfo.AsType()).ToList();

                _runtime.Logger.InfoNow(string.Format("executing method {0}.{1}",msg.Types[0].TypeInfo.Name, methods[0].Name));
                var runnable = new RunnableItem(msg.Types[0].TypeInfo, methods);
                var settings = new RunnerSettings()
                {
                    CompilerServiceType = CompilerType.Default,
                    Iterations = 100,
                    RunnerType = RunnerType.BackgroundWorkerBased
                };
                var runner = _runtime.CreateAndAddRunner(runnable, settings);
                runner.Start();

            }

        }
    }
}
