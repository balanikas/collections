using System;
using System.Windows.Media;
using Collections;
using Collections.Messages;
using GalaSoft.MvvmLight;

namespace WpfClient.ViewModels
{
    public class CanvasViewModel :ViewModelBase, IGui
    {

        private SolidColorBrush _color;
        private TimeSpan _executionTime;
        private int _progress;

        public SolidColorBrush CanvasColor
        {
            get { return _color; }
            set
            {
                _color = value;
                RaisePropertyChanged("CanvasColor");
            }
        }


        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged("Progress");
            }
        }


        public TimeSpan ExecutionTime
        {
            get { return _executionTime; }
            set
            {
                _executionTime = value;
                RaisePropertyChanged("ExecutionTime");
            }
        }

        public string Id { get; set; }
        public void Initialize()
        {
            // throw new NotImplementedException();
        }

        public void Update(MethodExecutionMessage message)
        {
            var doUpdate = message.Progress % 10 == 0;
            if (doUpdate || message.Progress == 100)
            {
                ExecutionTime = message.MethodExecutionResult.ExecutionTime;
                Progress = message.Progress;
            }
            
        }

        public void Update(MethodExecutionSummaryMessage message)
        {
           
        }

        public void Destroy()
        {
           // throw new NotImplementedException();
        }


    }
}