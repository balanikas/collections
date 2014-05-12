using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using System.Windows.Threading;
using Collections;
using Collections.Compiler;
using Collections.Runtime;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;

namespace WpfClient.ViewModels
{
    public class PlayModeViewModel :ViewModelBase
    {
        private readonly CompilerService _backgroundCompiler;
        private readonly RunnerService _runnerService;
        private BroadcastBlock<CompilerServiceMessage> _compilerBroadcasts;
        private BroadcastBlock<RunnerServiceMessage> _runnerBroadcasts;
        private TextDocument _codeDocument;
        private readonly TypesProvider _typesProvider;
        private readonly IRuntime _runtime;
        DispatcherTimer _timer;
        private List<IGui> _uiListeners;
        private bool _isStarted = false;

        public PlayModeViewModel(IRuntime runtime, TypesProvider typesProvider)
        {
            _runtime = runtime;
            _runtime.Reset();
            _typesProvider = typesProvider;
          
            _codeDocument = new TextDocument();
            CodeDocument.Text = "class X { public void Calc(){  \\n  for(int i = 0; i < 10000; i++) { var s = i; }    \\n}  }";

            _compilerBroadcasts = new BroadcastBlock<CompilerServiceMessage>(null);

            typesProvider.SetActiveCompilerService(CompilerType.Default);


            _backgroundCompiler = new CompilerService(_compilerBroadcasts);
            _runnerBroadcasts = new BroadcastBlock<RunnerServiceMessage>(null);
            _runnerService = new RunnerService(_runnerBroadcasts, _compilerBroadcasts);

            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 0,0,30);


            CmdStart = new RelayCommand(() =>
            {
                _isStarted = true;
                _backgroundCompiler.Start(CompilerAction);
                _runnerService.Start(RunnerAction);

                _compilerBroadcasts.Post(new CompilerServiceMessage(CodeDocument.Text));


                _timer.Start();


                
            });

            CmdStop = new RelayCommand(() =>
            {
                _isStarted = false;
                _timer.Stop();
                _backgroundCompiler.Stop();
                _runnerService.Stop();
                _uiListeners.Clear();
                OnClearLog();

            });

            CmdCodeDocumentTextChanged = new RelayCommand<EventArgs>((args) =>
            {
                if (!_isStarted)
                {
                    return;
                }
                _compilerBroadcasts.Post(new CompilerServiceMessage(CodeDocument.Text));
            });

            CmdClearLog = new RelayCommand(OnClearLog);


            _uiListeners = new List<IGui>();
            CanvasCtx = new CanvasViewModel();
           
            _uiListeners.Add(CanvasCtx);

        }

        private void RunnerAction(CompilerServiceMessage msg)
        {
            if (!_isStarted || msg.State == ServiceMessageState.NotHandled)
            {
                msg.State = ServiceMessageState.NotHandled;
                return;
            }
            if (msg.CompilerErrors.Count == 0 && msg.Types.Any())
            {
                var methods = msg.Types[0].MethodsInfos.Where(x => x.DeclaringType == msg.Types[0].TypeInfo.AsType()).ToList();

                _runtime.Logger.InfoNow(string.Format("executing method {0}.{1}", msg.Types[0].TypeInfo.Name, methods[0].Name));

                methods = Utils.GetSupportedMethods(methods);
                var runnable = new RunnableItem(msg.Types[0].TypeInfo, methods);
                var settings = new RunnerSettings()
                {
                    CompilerServiceType = Settings.CompilerServiceType,
                    Iterations = Settings.Loops,
                    RunnerType = Settings.ThreadingType
                };
                var runner = _runtime.CreateAndAddRunner(runnable, settings);
                foreach (var listener in _uiListeners)
                {
                    runner.AddUiListener(listener);
                }
                
                runner.Start();
                msg.State = ServiceMessageState.Succeeded;
            }
            else
            {
                msg.State = ServiceMessageState.Failed;
            }
            
        }

        private void CompilerAction(CompilerServiceMessage msg)
        {
            if (!_isStarted)
            {
                return;
            }
            List<string> errors;
            var types = _typesProvider.TryCompileFromText(msg.Source, out errors);

            if (errors.Any())
            {
                msg.State = ServiceMessageState.Failed;
            }
            else
            {
                msg.State = ServiceMessageState.Succeeded;
            }
            

            msg.CompilerErrors = errors;
            msg.Types = types;
            _runtime.Logger.InfoNow(string.Format("compilation errors:  {0}", string.Join("\n", errors.ToArray())));
        }
      

        public RelayCommand CmdStart { get; private set; }
        public RelayCommand CmdStop { get; private set; }

        public RelayCommand CmdClearLog { get; private set; }
        public RelayCommand<EventArgs> CmdCodeDocumentTextChanged { get; private set; }


        public CanvasViewModel CanvasCtx { get; private set; }


        private void OnClearLog()
        {
            ViewModelLocator.LogViewer.LogEntries.Clear();
        }

        public TextDocument CodeDocument
        {
            get { return _codeDocument; }
            set
            {
                if (_codeDocument == null)
                {
                    return;
                }
                _codeDocument = value;
                
                RaisePropertyChanged("CodeDocument");
            }
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            CompilerServiceMessage compilerResults = _compilerBroadcasts.Receive();

            switch (compilerResults.State)
            {
                case ServiceMessageState.NotHandled:
                    break;
                case ServiceMessageState.Succeeded:
                    _runtime.Logger.InfoNow("Code compiled successfully");
                    break;
                case ServiceMessageState.Failed:
                    _runtime.Logger.ErrorNow(string.Join("\n", compilerResults.CompilerErrors.ToArray()));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

         
            RunnerServiceMessage runnerResults = _runnerBroadcasts.Receive();

            switch (runnerResults.State)
            {
                case ServiceMessageState.NotHandled:
                    break;
                case ServiceMessageState.Succeeded:
                    _runtime.Logger.InfoNow("Runner completed successfully");
                    break;
                case ServiceMessageState.Failed:
                    _runtime.Logger.InfoNow("Runner failed to complete");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}
