using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Collections.Messages;

namespace Collections.Compiler
{
    public class CompilerService : IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly BroadcastBlock<CompilerServiceMessage> _compilerServiceMessage;
        private TimeSpan? _executionInterval;
        private readonly BroadcastBlock<CompilerServiceOutputMessage> _compilerServiceOutputMsgBuf;

        public bool IsRunning
        {
            get;
            private set;
        }

        public CompilerService(BroadcastBlock<CompilerServiceMessage> compilerServiceMessage,
            BroadcastBlock<CompilerServiceOutputMessage> compilerServiceOutputMsgBuf)
        {
            _compilerServiceOutputMsgBuf = compilerServiceOutputMsgBuf;
            _compilerServiceMessage = compilerServiceMessage;
            _executionInterval = TimeSpan.FromMilliseconds(1000);
        }

        public void Start(Func<CompilerServiceMessage, CompilerServiceOutputMessage> func = null, 
            TimeSpan? executionInterval = null)
        {

            if (IsRunning)
            {
                return;
            }

            IsRunning = true;

            _executionInterval = executionInterval ?? _executionInterval;

            func = func ?? DefaultCompilerExecution;
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

        public TimeSpan ExecutionInterval {
            set
            {
                _executionInterval = value;
            }
        }

        private ITargetBlock<string> StartService(
            Func<CompilerServiceMessage,CompilerServiceOutputMessage> action,
            CancellationToken cancellationToken)
        {
            string previousSource = null;


            var block = new ActionBlock<string>(async message =>
            {

                while (await _compilerServiceMessage.OutputAvailableAsync(cancellationToken))
                {

                    CompilerServiceMessage receivedMessage = _compilerServiceMessage.Receive();
                    if (previousSource != receivedMessage.Source)
                    {
                        previousSource = receivedMessage.Source;
                        var result = action(receivedMessage);
                        
                        // if (result.CompilerErrors.Count == 0)
                        {
                            _compilerServiceOutputMsgBuf.Post(result);
                        }
                    }

                    try
                    {
                        _compilerServiceMessage.Completion.Wait((int)_executionInterval.Value.TotalMilliseconds, cancellationToken);
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

        private CompilerServiceOutputMessage DefaultCompilerExecution(CompilerServiceMessage msg)
        {
            return new CompilerServiceOutputMessage(new List<string>(), new List<LoadedType>());
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
