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
        private string _cachedSource = "";
        private readonly TypesProvider _typesProvider;
        private readonly IRuntime _runtime;
        public CompilerService(IRuntime runtime, TypesProvider typesProvider, BroadcastBlock<CompilerServiceMessage> broadcastToOutput)
        {
            _runtime = runtime;
            _broadcastToOutput = broadcastToOutput;
            _typesProvider = typesProvider;
            _executionFrequency = TimeSpan.FromMilliseconds(1000);
        }

        public void Start(TimeSpan? executionFrequency = null)
        {
            _executionFrequency = executionFrequency ?? _executionFrequency;

            var action = new Action<CompilerServiceMessage>(Execute);
            _cancellationTokenSource = new CancellationTokenSource();


            _targetBlock = StartService(action, _broadcastToOutput, _cancellationTokenSource.Token);
            _targetBlock.Post(new CompilerServiceMessage(""));
          
          
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
            if (action == null) 
                throw new ArgumentNullException("action");

            ActionBlock<CompilerServiceMessage> block = null;

            block = new ActionBlock<CompilerServiceMessage>(async now =>
            {
               

                await Task.Delay(_executionFrequency.Value, cancellationToken).
                    ConfigureAwait(false);

                CompilerServiceMessage data = broadcastBlock.Receive();
               
                action(data);
                block.Post(data);
                if (data.CompilerErrors.Count == 0)
                {
                    broadcastBlock.Post(data);
                }
                
               
            }, new ExecutionDataflowBlockOptions
            {
                CancellationToken = cancellationToken
            });

            return block;
        }

        

        private void Execute(CompilerServiceMessage msg)
        {
            if (_cachedSource == msg.Source)
            {
                return;
            }
            
            List<string> errors;
            var types = _typesProvider.TryCompileFromText(msg.Source, out errors);
           
            msg.CompilerErrors = errors;
            msg.Types = types;

            _cachedSource = msg.Source;

            _runtime.Logger.InfoNow(string.Format("compilation errors:  {0}", string.Join("\n", errors.ToArray())));
        }

    
    }
}
