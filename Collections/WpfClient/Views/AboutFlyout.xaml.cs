using System.ComponentModel;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace WpfClient
{
    public partial class AboutFlyout : Flyout, INotifyPropertyChanged
    {
        private bool canCloseFlyout;

        private ICommand closeCmd;

        public AboutFlyout()
        {
            InitializeComponent();
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
    }
}