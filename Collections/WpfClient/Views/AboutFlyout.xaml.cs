using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using WpfClient.ViewModels;

namespace WpfClient
{
    public partial class AboutFlyout : Flyout, INotifyPropertyChanged
    {
        private bool canCloseFlyout;

        private ICommand closeCmd;

        public AboutFlyout()
        {
            InitializeComponent();

            TxtVersion.Text = "0.1.0";
        }

        public bool CanCloseFlyout
        {
            get { return canCloseFlyout; }
            set
            {
                if (Equals(value, canCloseFlyout))
                {
                    return;
                }
                canCloseFlyout = value;
                RaisePropertyChanged("CanCloseFlyout");
            }
        }

        public ICommand CloseCmd
        {
            get
            {
                return closeCmd ?? (closeCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => CanCloseFlyout,
                    ExecuteDelegate = x => IsOpen = false
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}