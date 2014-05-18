using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Collections;
using Collections.Compiler;
using Collections.Runtime;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;

namespace WpfClient.ViewModels
{
    public class PlayModeViewModel : ViewModelBase
    {
        private readonly CompilerService _compilerService;
        private readonly RunnerService _runnerService;
        private BroadcastBlock<CompilerServiceMessage> _compilerBroadcasts;
        private BroadcastBlock<RunnerServiceMessage> _runnerBroadcasts;
        private TextDocument _codeDocument;
        private readonly TypesProvider _typesProvider;
        private readonly IRuntime _runtime;
        private DispatcherTimer _timer;
        private List<IGui> _uiListeners;
        private bool _isStarted = false;
        private int _compilerInterval;
        private Canvas _canvas;
        private List<MethodInfo> _compiledMethods;
        private MethodInfo _selectedCompiledMethod;
        private int _runnerInterval;

        private void OnSettingsUpdated(object sender, SettingsUpdatedEventArgs e)
        {
            if (e.Type == Setting.CompilerInterval)
            {
                CompilerInterval = (int)e.Value;
            }
            if (e.Type == Setting.RunnerInterval)
            {
                RunnerInterval = (int)e.Value;
            }
        }

        public PlayModeViewModel(IRuntime runtime, TypesProvider typesProvider)
        {
            _runtime = runtime;
            _runtime.Reset();
            _typesProvider = typesProvider;

            Settings.Instance.OnSettingsUpdated += OnSettingsUpdated;
        
            _codeDocument = new TextDocument();
            CodeDocument.Text =
                "class X { \n public void Calc() { \n for(int i = 0; i < 10000; i++) { \n var s = i; } \n  } \n public void Calc2(){} \n }";


            _compilerBroadcasts = new BroadcastBlock<CompilerServiceMessage>(null);

            typesProvider.SetActiveCompilerService(CompilerType.Default);


            _compilerService = new CompilerService(_compilerBroadcasts);
            _runnerBroadcasts = new BroadcastBlock<RunnerServiceMessage>(null);
            _runnerService = new RunnerService(_runnerBroadcasts, _compilerBroadcasts);

            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 30);


            CmdStart = new RelayCommand(() =>
            {
                _isStarted = true;
                _compilerService.Start(CompilerAction, TimeSpan.FromMilliseconds(CompilerInterval));
                _runnerService.Start(RunnerAction);

                _compilerBroadcasts.Post(new CompilerServiceMessage(CodeDocument.Text));


                _timer.Start();
            });

            CmdStop = new RelayCommand(() =>
            {
                _isStarted = false;
                _timer.Stop();
                _compilerService.Stop();
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

            CmdCanvasLoaded = new RelayCommand<RoutedEventArgs>((args) => { _canvas = args.Source as Canvas; });

            CmdClearLog = new RelayCommand(OnClearLog);


            _uiListeners = new List<IGui>();
            CanvasCtx = new CanvasViewModel();

            _uiListeners.Add(CanvasCtx);

            CompilerInterval = Settings.Instance.Get(Settings.Keys.CompilerInterval);
            RunnerInterval = Settings.Instance.Get(Settings.Keys.RunnerInterval);
        }

       


        private void RunnerAction(CompilerServiceMessage msg)
        {
            if (!_isStarted || msg.State == (ServiceMessageState.NotHandled | ServiceMessageState.Failed))
            {
                msg.State = ServiceMessageState.NotHandled;
                return;
            }
            
            if (msg.CompilerErrors.Count == 0 && msg.Types.Any())
            {
                // var methods = msg.Types[0].MethodsInfos.Where(x => x.DeclaringType == msg.Types[0].TypeInfo.AsType()).ToList();
                // methods = Utils.GetSupportedMethods(methods);
                if (SelectedCompiledMethod != null)
                {

                    if (_runtime.Runners.GetActiveRunners().Any())
                    {
                        foreach (var r in _runtime.Runners.GetActiveRunners())
                        {
                            r.Destroy();
                        }
                        return;
                    }
                    
                    _runtime.Logger.InfoNow(string.Format("executing method {0}.{1}", msg.Types[0].TypeInfo.Name,
                        SelectedCompiledMethod.Name));


                    var runnable = new RunnableItem(msg.Types[0].TypeInfo,
                        new List<MethodInfo>() {SelectedCompiledMethod});
                    var settings = new RunnerSettings()
                    {
                        CompilerServiceType = Settings.Instance.Get(Settings.Keys.CompilerServiceType),
                        Iterations = Settings.Instance.Get(Settings.Keys.PlayModeIterationCount),
                        RunnerType = Settings.Instance.Get(Settings.Keys.ThreadingType)
                    };
                    var runner = _runtime.CreateAndAddRunner(runnable, settings);

                    
                    _canvas.Dispatcher.BeginInvoke((new Action(delegate
                    {
                        _canvas.Children.Clear();
                        var shape = UIHelper.CreateDrawingShape(_canvas,
                            new Point(_canvas.ActualWidth/2, _canvas.ActualHeight/2));
                        runner.AddUiListener(shape);

                        foreach (var listener in _uiListeners)
                        {
                            runner.AddUiListener(listener);
                        }

                        runner.Start();
                    })));
                }


                msg.State = ServiceMessageState.Succeeded;
            }
            else
            {
                msg.State = ServiceMessageState.Failed;
            }
        }

        private void CompilerAction(CompilerServiceMessage msg)
        {
            if (!_isStarted || String.IsNullOrEmpty(msg.Source))
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
            if (!msg.CompilerErrors.Any())
            {
                CompiledMethods = types[0].MethodsInfos;
                //SelectedCompiledMethod = types[0].MethodsInfos[0];
            }
            else
            {
                CompiledMethods = null;
            }

            _runtime.Logger.InfoNow(string.Format("compilation errors:  {0}", string.Join("\n", errors.ToArray())));
        }


        public RelayCommand CmdStart { get; private set; }
        public RelayCommand CmdStop { get; private set; }
        public RelayCommand<RoutedEventArgs> CmdCanvasLoaded { get; private set; }
        public RelayCommand CmdClearLog { get; private set; }
        public RelayCommand<EventArgs> CmdCodeDocumentTextChanged { get; private set; }


        public CanvasViewModel CanvasCtx { get; private set; }


        private void OnClearLog()
        {
            ViewModelLocator.LogViewer.LogEntries.Clear();
        }


        public List<MethodInfo> CompiledMethods
        {
            get { return _compiledMethods; }
            set
            {
                _compiledMethods = value;
                RaisePropertyChanged("CompiledMethods");
            }
        }

        public MethodInfo SelectedCompiledMethod
        {
            get { return _selectedCompiledMethod; }
            set
            {
                _selectedCompiledMethod = value;
                RaisePropertyChanged("SelectedCompiledMethod");
            }
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

        public int CompilerInterval
        {
            get { return _compilerInterval; }
            set
            {
                _compilerInterval = value;
                _compilerService.ExecutionInterval = TimeSpan.FromMilliseconds(_compilerInterval);
                RaisePropertyChanged("CompilerInterval");
            }
        }

        public int RunnerInterval
        {
            get { return _runnerInterval; }
            set
            {
                _runnerInterval = value;
                _runnerService.ExecutionInterval = TimeSpan.FromMilliseconds(_runnerInterval);
                RaisePropertyChanged("RunnerInterval");
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
                    _runtime.Logger.ErrorNow("Runner failed to complete");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}