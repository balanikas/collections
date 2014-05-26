using System;
using System.Linq;
using Collections;
using Collections.Messages;
using Collections.Runtime;
using GalaSoft.MvvmLight;
using ICSharpCode.AvalonEdit.Document;

namespace WpfClient.ViewModels
{
    public class MethodExecutionView :  ViewModelBase, IGui
    {
        public class InfoView
        {
          

            public void Update(MethodExecutionResultAggregation msg)
            {
                AvgMethodExecutionTime = Math.Round(msg.AvgMethodExecutionTime, 3);
                MinMethodExecutionTime = Math.Round(msg.MinMethodExecutionTime, 3);
                MaxMethodExecutionTime = Math.Round(msg.MaxMethodExecutionTime, 3);
                ExecutionsCount = msg.ExecutionsCount;
                FailedExecutionsCount = msg.FailedExecutionsCount;
                
            }

            public void Update(MethodExecutionMessage msg)
            {
                TypeName = msg.ObjectType != null ? msg.ObjectType.ToString() : null;
                MethodName = msg.Method != null ? msg.Method.ToString() : null;
                
                Progress = msg.Progress;

               
                TotalExecutionTime = Math.Round(msg.TotalExecutionTime.TotalMilliseconds,3);

                if (msg.Aggregation != null)
                {
                    AvgMethodExecutionTime = Math.Round(msg.Aggregation.AvgMethodExecutionTime, 3);
                    MinMethodExecutionTime = Math.Round(msg.Aggregation.MinMethodExecutionTime, 3);
                    MaxMethodExecutionTime = Math.Round(msg.Aggregation.MaxMethodExecutionTime, 3);
                    ExecutionsCount = msg.Aggregation.ExecutionsCount;
                    FailedExecutionsCount = msg.Aggregation.FailedExecutionsCount;
                }
                else
                {
                    AvgMethodExecutionTime = default(double);
                    MinMethodExecutionTime = default(double);
                    MaxMethodExecutionTime = default(double);
                    ExecutionsCount = default(int);
                    FailedExecutionsCount = default(int);
                }

                if (msg.MethodExecutionResult != null)
                {
                    if (msg.MethodExecutionResult.ArgsValues != null)
                    {
                        MethodArgs = string.Join(",", msg.MethodExecutionResult.ArgsValues.Select(x => x.ToString()));
                    }
                    else
                    {
                        MethodArgs = null;
                    }

                    if (msg.MethodExecutionResult.ReturnValue != null)
                    {
                        MethodReturnValue = msg.MethodExecutionResult.ReturnValue.ToString();
                    }
                    else
                    {
                        MethodReturnValue = null;
                    }
                }
               
                
            }
            public string TypeName { get; set; }
            public string MethodName { get; set; }
            public int Progress { get; set; }
            public double TotalExecutionTime { get; set; }
            public double AvgMethodExecutionTime { get; set; }
            public double MinMethodExecutionTime { get; set; }
            public double MaxMethodExecutionTime { get; set; }
            public int ExecutionsCount { get; set; }
            public int FailedExecutionsCount { get; set; }

            public string MethodArgs { get; set; }
            public string MethodReturnValue { get; set; }
        }

        private MethodExecutionMessage _message;
        private IRunner _runner;
        private bool _isExpanded;
        private InfoView _infoView;
        public string Id { get; set; }

        public MethodExecutionView()
        {
            _infoView = new InfoView();
        }
        public void Initialize()
        {
           
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                RaisePropertyChanged("IsExpanded");
            }
        }

        public void Register(IRunner runner)
        {
            if (_runner != null)
            {
                _runner.RemoveUiListener(this);
            }
          
           
            _runner = runner;
            
            _infoView.Update(_runner.GetCurrentState());
            RaisePropertyChanged("Info");
            _runner.AddUiListener(this);
        }

        public void Update(MethodExecutionMessage message)
        {
            _infoView.Update(message);
            RaisePropertyChanged("Info");
        }


        public void Destroy()
        {
            _infoView = new InfoView();
        }

        public InfoView Info
        {
            get { return _infoView; }
            set
            {
                _infoView = value;
                RaisePropertyChanged("Info");
            }
        }


    }

    
}