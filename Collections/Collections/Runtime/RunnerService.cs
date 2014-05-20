using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Collections.Compiler;
using Collections.Messages;

namespace Collections.Runtime
{
    public class RunnerService : IDisposable
    {
        private readonly BroadcastBlock<RunnerServiceOutputMessage> _runnerServiceOutputMsgBuf;
        private readonly BroadcastBlock<CompilerServiceOutputMessage> _compilerServiceMsgBuf;
        private TimeSpan? _executionInterval;
        private CancellationTokenSource _cancellationTokenSource;

        public bool IsRunning
        {
            get;
            private set;
        }
        
        public RunnerService(BroadcastBlock<CompilerServiceOutputMessage> compilerServiceMsgBuf ,
            BroadcastBlock<RunnerServiceOutputMessage> runnerServiceOutputMsgBuf)
        {
            _compilerServiceMsgBuf = compilerServiceMsgBuf;
            _runnerServiceOutputMsgBuf = runnerServiceOutputMsgBuf;
            _executionInterval = TimeSpan.FromMilliseconds(1000);
        }

        public void Start(
            Func<CompilerServiceOutputMessage, RunnerServiceOutputMessage> func = null,
            TimeSpan? executionInterval = null)
        {
            if (IsRunning)
            {
                return;
            }

            IsRunning = true;
            _executionInterval = executionInterval ?? _executionInterval;
            func = func ?? DefaultRunnerExecution;

            _cancellationTokenSource = new CancellationTokenSource();

            StartService(func, _cancellationTokenSource.Token).Post("");
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
              Func<CompilerServiceOutputMessage,RunnerServiceOutputMessage> func,
              CancellationToken cancellationToken)
        {

            ActionBlock<string> block = null;
            block = new ActionBlock<string>(async now =>
            {

                while (await _compilerServiceMsgBuf.OutputAvailableAsync(cancellationToken))
                {
                    CompilerServiceOutputMessage message = _compilerServiceMsgBuf.Receive();
                   
                  
                    var result = func(message);

                    _runnerServiceOutputMsgBuf.Post(result);

                    try
                    {
                        _compilerServiceMsgBuf.Completion.Wait((int)_executionInterval.Value.TotalMilliseconds, cancellationToken);
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
   
        private RunnerServiceOutputMessage DefaultRunnerExecution(CompilerServiceOutputMessage msg)
        {
            return new RunnerServiceOutputMessage();
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
