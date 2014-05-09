using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
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
        private CompilerService _backgroundCompiler;
        private RunnerService _runnerService;
        private BroadcastBlock<CompilerServiceMessage> _compilerBroadcasts;
        private BroadcastBlock<RunnerServiceMessage> _runnerBroadcasts;
        private TextDocument _codeDocument;
        private SolidColorBrush _canvasColor;
        private string _executionTime;
        private readonly IRuntime _runtime;


        public PlayModeViewModel(IRuntime runtime, TypesProvider typesProvider)
        {
            _runtime = runtime;
            _runtime.Reset();

            _canvasColor = new SolidColorBrush(Colors.Orange);
            _codeDocument = new TextDocument();
            CodeDocument.Text = @"class X { public void Calc(){}  }";

            _compilerBroadcasts = new BroadcastBlock<CompilerServiceMessage>(null);

            typesProvider.SetActiveCompilerService(CompilerType.Default);


            _backgroundCompiler = new CompilerService(_runtime, typesProvider, _compilerBroadcasts);
            _runnerBroadcasts = new BroadcastBlock<RunnerServiceMessage>(null);
            _runnerService = new RunnerService(_runtime, _runnerBroadcasts, _compilerBroadcasts);

            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);


            CmdStart = new RelayCommand(() =>
            {
               
                _backgroundCompiler.Start();
                _runnerService.Start();

                _compilerBroadcasts.Post(new CompilerServiceMessage(CodeDocument.Text));

                
                dispatcherTimer.Start();
                
            });

            CmdStop = new RelayCommand(() =>
            {
                dispatcherTimer.Stop();
                _backgroundCompiler.Stop();
                _runnerService.Stop();
                
                OnClearLog();

            });

            CmdCodeDocumentTextChanged = new RelayCommand<EventArgs>((args) =>
            {
                _compilerBroadcasts.Post(new CompilerServiceMessage(CodeDocument.Text));
            });

            CmdClearLog = new RelayCommand(OnClearLog);
        }


        public RelayCommand CmdStart { get; private set; }
        public RelayCommand CmdStop { get; private set; }

        public RelayCommand CmdClearLog { get; private set; }
        public RelayCommand<EventArgs> CmdCodeDocumentTextChanged { get; private set; }

        public SolidColorBrush CanvasColor
        {
            get { return _canvasColor; }
            set
            {
                _canvasColor = value;
                RaisePropertyChanged("CanvasColor");
            }
        }

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

        public string ExecutionTime
        {
            get { return _executionTime; }
            set
            {
                _executionTime = value;
                RaisePropertyChanged("ExecutionTime");
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            CompilerServiceMessage results = _compilerBroadcasts.Receive();
            //todo: output to log
            //if (results.CompilerErrors.Any())
            //{
            //    CanvasColor = new SolidColorBrush(Colors.Red);
            //}
            //else
            //{
            //    CanvasColor = new SolidColorBrush(Colors.Green);
            //}
            RunnerServiceMessage runnerResults = _runnerBroadcasts.Receive();

            ExecutionTime = runnerResults.AvgExecutionTime.ToString();
            //todo: output to log
            //ViewModelLocator.LogViewer.LogEntries.Clear();
            //foreach (var s in results.CompilerErrors)
            //{
            //    ViewModelLocator.Logger.ErrorNow(s);
            //}
          
        }
    }
}
