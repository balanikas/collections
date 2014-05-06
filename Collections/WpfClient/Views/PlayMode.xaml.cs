using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Collections;

namespace WpfClient.Views
{
    /// <summary>
    ///     Interaction logic for PlayMode.xaml
    /// </summary>
    public partial class PlayMode : UserControl
    {
        private string source;
        private string result;
        private BackgroundCompilerService _service;
        private BackgroundCompiler _backgroundCompiler;
        private BroadcastBlock<CompiledResultsMessage> _compilerBroadcasts;
        private BackgroundRunner _backgroundRunner;
        private BroadcastBlock<RunnerOutput> _runnerBroadcasts; 
        public PlayMode()
        {
            InitializeComponent();
        }

        private void avalonEdit_TextChanged(object sender, EventArgs e)
        {
            source = AvalonEdit.Text;
            _compilerBroadcasts.Post(new CompiledResultsMessage(source));
        }

        private void btnStart_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _compilerBroadcasts = new BroadcastBlock<CompiledResultsMessage>(null);

            _backgroundCompiler = new BackgroundCompiler(_compilerBroadcasts);
            _backgroundCompiler.Start();

            _runnerBroadcasts = new BroadcastBlock<RunnerOutput>(null);
            _backgroundRunner = new BackgroundRunner(_runnerBroadcasts,_compilerBroadcasts);
            _backgroundRunner.Start();


            source = AvalonEdit.Text;
            _compilerBroadcasts.Post(new CompiledResultsMessage(source));

            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            BtnStart.IsEnabled = false;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var results = _compilerBroadcasts.Receive();
            if (results.HasCompiled)
            {

                result = "Successfully compiled";
                
                TxtLog.Document.Blocks.Clear();
                var paragraph = new Paragraph(new Run(result));
                TxtLog.Document.Blocks.Add(paragraph);
                TxtLog.ScrollToEnd();
                
            }
            else
            {
                result = String.Empty;
                foreach (string error in results.CompilerErrors)
                {
                    result += error + Environment.NewLine;
                }
                TxtLog.Document.Blocks.Clear();
                var paragraph = new Paragraph(new Run(result));
                paragraph.Foreground = new SolidColorBrush(Colors.Red);
                TxtLog.Document.Blocks.Add(paragraph);
                TxtLog.ScrollToEnd();
                
            }

            var runnerResults = _runnerBroadcasts.Receive();
            if (!runnerResults.Success)
            {
                lbl.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                lbl.Foreground = new SolidColorBrush(Colors.Wheat);
            }
            lbl.Content = runnerResults.AvgExecutionTime;

        }

        private void btnStop_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _service.Stop();
            BtnStart.IsEnabled = true;
        }
    }
}