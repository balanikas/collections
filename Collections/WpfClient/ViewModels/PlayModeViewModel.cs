using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Collections;
using Collections.Compiler;
using Collections.Messages;
using Collections.Runtime;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using WpfClient.Properties;

namespace WpfClient.ViewModels
{
    public class PlayModeViewModel : ViewModelBase
    {
        private readonly CompilerService _compilerService;
        private readonly RunnerService _runnerService;
        private readonly IRuntime _runtime;
        private readonly DispatcherTimer _timer;
        private readonly TypesProvider _typesProvider;
        private readonly List<IGui> _uiListeners;
        private Canvas _canvas;
        private TextDocument _codeDocument;
        private ObservableCollection<MethodInfo> _compiledMethods;
        private int _compilerInterval;
        private BroadcastBlock<CompilerServiceMessage> _compilerServiceMsgBuf;
        private BroadcastBlock<CompilerServiceOutputMessage> _compilerServiceOutputMsgBuf;
        private bool _isActivated = false;
        private int _runnerInterval;
        private BroadcastBlock<RunnerServiceOutputMessage> _runnerServiceOutputMsgBuf;
        private MethodInfo _selectedCompiledMethod;
        private MethodInfo _selectedMethod;

        public PlayModeViewModel(IRuntime runtime, TypesProvider typesProvider)
        {
            _runtime = runtime;
            _runtime.Reset();
            _typesProvider = typesProvider;
            _codeDocument = new TextDocument();
            _uiListeners = new List<IGui>();
            CompiledMethods = new ObservableCollection<MethodInfo>();

            CodeDocument.Text = Resources.Sample;

            SetupServices();
            _compilerService = new CompilerService(_compilerServiceMsgBuf, _compilerServiceOutputMsgBuf);

            _runnerService = new RunnerService(_compilerServiceOutputMsgBuf, _runnerServiceOutputMsgBuf);

            typesProvider.SetActiveCompilerService(CompilerType.Default);

            _timer = new DispatcherTimer();
            _timer.Tick += LogServices;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 30);
            
            CmdCodeDocumentTextChanged = new RelayCommand<EventArgs>(args =>
            {
                if (!IsActivated)
                {
                    return;
                }
                _compilerServiceMsgBuf.Post(new CompilerServiceMessage(CodeDocument.Text));
            });

            CmdCanvasLoaded = new RelayCommand<RoutedEventArgs>(args => { _canvas = args.Source as Canvas; });

            CmdClearLog = new RelayCommand(OnClearLog);

            CmdSelectedMethodChanged = new RelayCommand<SelectionChangedEventArgs>(args =>
            {
                args.Handled = true;
                if (args.AddedItems.Count == 1)
                {
                    _selectedMethod = args.AddedItems[0] as MethodInfo;
                }
                else
                {
                    _selectedMethod = null;
                }
               
            });

            Settings.Instance.OnSettingsUpdated += OnSettingsUpdated;
            CompilerInterval = Settings.Instance.Get(Settings.Keys.CompilerInterval);
            RunnerInterval = Settings.Instance.Get(Settings.Keys.RunnerInterval);
        }

        public RelayCommand<SelectionChangedEventArgs> CmdSelectedMethodChanged { get; private set; }
        public RelayCommand<RoutedEventArgs> CmdCanvasLoaded { get; private set; }
        public RelayCommand CmdClearLog { get; private set; }
        public RelayCommand<EventArgs> CmdCodeDocumentTextChanged { get; private set; }


        public bool IsActivated
        {
            get
            {
                return _isActivated;
            }
            set
            {
                if (value)
                {
                   OnActivated();
                }
                else
                {
                   OnDeactivated();
                }
                _isActivated = value;
                RaisePropertyChanged("IsActivated");
            }
        }

        public ObservableCollection<MethodInfo> CompiledMethods
        {
            get
            {
                return _compiledMethods;
            }
            set
            {
                _compiledMethods = value;
                RaisePropertyChanged("CompiledMethods");
            }
        }

        public MethodInfo SelectedCompiledMethod
        {
            get
            {
                return _selectedCompiledMethod;
            }
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

        private void SetupServices()
        {
            _compilerServiceMsgBuf = new BroadcastBlock<CompilerServiceMessage>(message =>
            {
                return new CompilerServiceMessage(message.Source);
            });
            
            _compilerServiceOutputMsgBuf = new BroadcastBlock<CompilerServiceOutputMessage>(message =>
            {
                var clone = new CompilerServiceOutputMessage(message.CompilerErrors, message.Types, message.State);
                return clone;
            });
            _runnerServiceOutputMsgBuf = new BroadcastBlock<RunnerServiceOutputMessage>(message =>
            {
                var clone = new RunnerServiceOutputMessage(message.State, message.ErrorMessage);
                
                return clone;
            });
        }

        private RunnerServiceOutputMessage RunnerAction(CompilerServiceOutputMessage message)
        {

            if (!IsActivated || message.State == (ServiceMessageState.Failed | ServiceMessageState.NotHandled))
            {
                return new RunnerServiceOutputMessage();
            }

            if (message.CompilerErrors.Count == 0 && message.Types.Any())
            {

                if (_selectedMethod != null)
                {

                    if (_runtime.Runners.GetActiveRunners().Any())
                    {
                        foreach (var r in _runtime.Runners.GetActiveRunners())
                        {
                            r.Destroy();
                        }
                    }

                    var settings = new RunnerSettings(
                         Settings.Instance.Get(Settings.Keys.ExploreModeIterationCount),
                         Settings.Instance.Get(Settings.Keys.ThreadingType),
                         Settings.Instance.Get(Settings.Keys.CompilerServiceType)
                         );

                    try
                    {
                        var runnable = new RunnableItem(message.Types[0].TypeInfo,
                            new List<MethodInfo>() { _selectedMethod });

                        var runner = _runtime.CreateAndAddRunner(runnable, settings);

                        _canvas.Dispatcher.BeginInvoke((new Action(delegate
                        {
                            _canvas.Children.Clear();
                            var shape = UIHelper.CreateDrawingShape(_canvas,
                                new Point(_canvas.ActualWidth / 2, _canvas.ActualHeight / 2));
                            runner.AddUiListener(shape);

                            foreach (var listener in _uiListeners)
                            {
                                runner.AddUiListener(listener);
                            }

                            runner.Start();
                        })));
                    }
                    catch (Exception e)
                    {
                        return new RunnerServiceOutputMessage(ServiceMessageState.Failed, e.Message);
                    }

                }
                return new RunnerServiceOutputMessage(ServiceMessageState.Succeeded);
            }

            return new RunnerServiceOutputMessage(ServiceMessageState.Failed);
        }

        private CompilerServiceOutputMessage CompilerAction(CompilerServiceMessage message)
        {

            if (!IsActivated || String.IsNullOrEmpty(message.Source))
            {
                return new CompilerServiceOutputMessage(new List<string>(),new List<LoadedType>());
            }
            List<string> errors;
            var types = _typesProvider.TryCompileFromText(message.Source, out errors);

            if (!errors.Any())
            {
                if (_selectedMethod != null)
                {
                    CompiledMethods = new ObservableCollection<MethodInfo>(types[0].MethodsInfos);
                    SelectedCompiledMethod = CompiledMethods.FirstOrDefault(x => x.Name == _selectedMethod.Name);
                }
                else
                {
                    CompiledMethods = new ObservableCollection<MethodInfo>(types[0].MethodsInfos);
                }
                
                return new CompilerServiceOutputMessage(errors, types, ServiceMessageState.Succeeded);
            }
            else
            {
                CompiledMethods = null;
                return new CompilerServiceOutputMessage(errors, types, ServiceMessageState.Failed);
            }
            
        }

        private void OnClearLog()
        {
            ViewModelLocator.LogViewer.LogEntries.Clear();
        }

        private void OnActivated()
        {
            _compilerService.Start(CompilerAction, TimeSpan.FromMilliseconds(CompilerInterval));
            _runnerService.Start(RunnerAction);
            _compilerServiceMsgBuf.Post(new CompilerServiceMessage(CodeDocument.Text));
            _timer.Start();
            OnClearLog();
            _runtime.Logger.InfoNow("Services started, running live code");
        }

        private void OnDeactivated()
        {
            _timer.Stop();
            _compilerService.Stop();
            _runnerService.Stop();
            _runtime.Runners.RemoveAll();
            _uiListeners.Clear();
            CompiledMethods = null;
            OnClearLog();
   

        }

        
        private void LogServices(object sender, EventArgs e)
        {
  
            CompilerServiceOutputMessage compilerResults = _compilerServiceOutputMsgBuf.Receive();

            switch (compilerResults.State)
            {
                case ServiceMessageState.NotHandled:
                    break;
                case ServiceMessageState.Succeeded:
                    break;
                case ServiceMessageState.Failed:
                    OnClearLog();
                    _runtime.Logger.ErrorNow(string.Join("\n", compilerResults.CompilerErrors.ToArray()));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            
            RunnerServiceOutputMessage runnerResults = _runnerServiceOutputMsgBuf.Receive();

            switch (runnerResults.State)
            {
                case ServiceMessageState.NotHandled:
                    break;
                case ServiceMessageState.Succeeded:
                    break;
                case ServiceMessageState.Failed:
                    OnClearLog();
                    _runtime.Logger.ErrorNow("Runner failed to complete");
                    _runtime.Logger.ErrorNow(runnerResults.ErrorMessage);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}