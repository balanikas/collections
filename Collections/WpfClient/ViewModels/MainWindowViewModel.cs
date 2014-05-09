using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace WpfClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            CmdAbout = new RelayCommand(() => { MainWindow.ToggleFlyout(2); });

            CmdSettings = new RelayCommand(() => { MainWindow.ToggleFlyout(0); });
        }


        public RelayCommand CmdAbout { get; private set; }

        public RelayCommand CmdSettings { get; private set; }
    }
}