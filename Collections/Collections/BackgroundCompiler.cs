using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Collections
{
    public class BackgroundCompiler
    {
        private CancellationTokenSource _cancellationTokenSource;
        private ITargetBlock<CompiledResultsMessage> _targetBlock;
        private readonly BroadcastBlock<CompiledResultsMessage> _broadcastToOutput;
        private TimeSpan? _scanFrequancy;

        public BackgroundCompiler(BroadcastBlock<CompiledResultsMessage> broadcastToOutput)
        {
            _broadcastToOutput = broadcastToOutput;
        }

        public void Start(TimeSpan? scanFrequency = null)
        {
            _scanFrequancy = scanFrequency ?? TimeSpan.FromMilliseconds(100);

            var action = new Action<CompiledResultsMessage>(Compile);
            _cancellationTokenSource = new CancellationTokenSource();


            _targetBlock = StartService(action, _broadcastToOutput, _cancellationTokenSource.Token);
            _targetBlock.Post(new CompiledResultsMessage(""));
          
          
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

        ITargetBlock<CompiledResultsMessage> StartService(
            Action<CompiledResultsMessage> action, 
            BroadcastBlock<CompiledResultsMessage> broadcastBlock, 
            CancellationToken cancellationToken)
        {
            if (action == null) 
                throw new ArgumentNullException("action");

            ActionBlock<CompiledResultsMessage> block = null;

            block = new ActionBlock<CompiledResultsMessage>(async now =>
            {
               

                await Task.Delay(_scanFrequancy.Value, cancellationToken).
                    ConfigureAwait(false);

                CompiledResultsMessage data = broadcastBlock.Receive();
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

        private string _cachedSource = "";
        private void Compile(CompiledResultsMessage msg)
        {
            if (_cachedSource == msg.Source)
            {
                return;
            }


            var typesLoader = new TypesProvider(new DefaultCompilerService());
            List<string> errors;
            var compiled = typesLoader.TryCompileFromSource(msg.Source, out errors);
            msg.HasCompiled = compiled;
            msg.CompilerErrors = errors;

            Trace.WriteLine("ahhhh...just compiled " + msg.Source);

            _cachedSource = msg.Source;
        }

    
    }
}
