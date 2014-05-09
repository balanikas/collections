using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Collections;
using Collections.Logging;
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

        public ObservableCollection<LogEntry> LogEntries { get; set; }

        public void Notify(LogMessage message)
        {
            Log(message);
        }

        public void Log(LogMessage message)
        {
            Application.Current.Dispatcher.BeginInvoke((Action) (() =>
            {

                var textBrush = new SolidColorBrush((message.IsError ? Colors.Red : Colors.White));
                LogEntries.Add(
                    new LogEntry
                    {
                        Message = message.Message,
                        Index = 0,
                        TextBrush = textBrush
                    });

            }));
        }
    }
}