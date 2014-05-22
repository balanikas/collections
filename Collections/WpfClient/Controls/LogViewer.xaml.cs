using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace WpfClient.Controls
{
    public partial class LogViewer : UserControl
    {
        public LogViewer()
        {
            InitializeComponent();
            this.Loaded += (sender, args) =>
            {
                DataContext = ViewModelLocator.LogViewer.LogEntries;

            };
            
        }

        

        public ObservableCollection<LogEntry> LogEntries { get; set; }
    }
}