using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Collections;
using GalaSoft.MvvmLight;
using WpfClient.Controls;

namespace WpfClient.ViewModels
{
    public class LogViewerViewModel : ViewModelBase, ILogSubscriber
    {
       
        public LogViewerViewModel()
        {
            LogEntries = new ObservableCollection<LogEntry>();
        }

        public void Log(LogMessage message)
        {

            var textBrush = new SolidColorBrush((message.IsError ? Colors.MediumVioletRed : Colors.White));
            

            Application.Current.Dispatcher.BeginInvoke((Action)(() => LogEntries.Add(
                new LogEntry
                {
                    Message = message.Message,
                    Index = 0,
                    TextBrush = textBrush
                })));
           
        }
        public ObservableCollection<LogEntry> LogEntries { get; set; }
        public void Notify(LogMessage message)
        {
            Log(message);
        }
    }
}
