using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Collections;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro;
using MahApps.Metro.Controls;

namespace WpfClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Visibility _progressBarVisibility;
      
        public MainWindowViewModel()
        {
            CmdAbout = new RelayCommand(() =>
            {
                MainWindow.ToggleFlyout(2);
            });

            CmdSettings = new RelayCommand(() =>
            {
                MainWindow.ToggleFlyout(0);
            });

         

            ProgressBarVisibility = Visibility.Hidden;
        }

        public Visibility ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                _progressBarVisibility = value;
                RaisePropertyChanged("ProgressBarVisibility");
            }
        }

        public RelayCommand CmdAbout { get; private set; }

        public RelayCommand CmdSettings { get; private set; }

    

    }
}
