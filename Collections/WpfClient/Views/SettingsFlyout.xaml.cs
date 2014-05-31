using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Collections.Compiler;
using Collections.Runtime;
using MahApps.Metro.Controls;
using WpfClient.Tutorial;

namespace WpfClient
{
    public partial class SettingsFlyout : Flyout, INotifyPropertyChanged
    {
        private bool canCloseFlyout;

        private ICommand closeCmd;

        public SettingsFlyout()
        {
            InitializeComponent();

            IEnumerable<DrawTypes> drawTypes = Enum.GetValues(typeof (DrawTypes)).Cast<DrawTypes>();
            cmbGraphics.ItemsSource = drawTypes;

            IEnumerable<CompilerType> compilerServiceTypes =
                Enum.GetValues(typeof (CompilerType)).Cast<CompilerType>();
            cmbCompilerService.ItemsSource = compilerServiceTypes;


            
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
                    ExecuteDelegate = x => { IsOpen = false; }
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


        private void cmbGraphics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.Instance.Set(Settings.Keys.DrawAs, (DrawTypes)cmbGraphics.SelectedItem);
        }

        private void radUseTPL_Checked(object sender, RoutedEventArgs e)
        {
            if (radUseBW.IsChecked == true)
            {
                Settings.Instance.Set(Settings.Keys.ThreadingType, RunnerType.BackgroundWorkerBased);
            }
            else
            {
                Settings.Instance.Set(Settings.Keys.ThreadingType, RunnerType.ParallelTaskBased);
            }
        }

        private void CmbCompilerService_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.Instance.Set(Settings.Keys.CompilerServiceType, (CompilerType)cmbCompilerService.SelectedItem);
        }

        private void SldCompilerInterval_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            Settings.Instance.Set(Settings.Keys.CompilerInterval,(int)e.NewValue);
        }

        private void SldRunnerInterval_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Instance.Set(Settings.Keys.RunnerInterval, (int)e.NewValue);
        }

        private void SldPlayModeIterationCount_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Instance.Set(Settings.Keys.PlayModeIterationCount,(int) e.NewValue);
        }

        private void SldExploreModeIterationCount_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Instance.Set(Settings.Keys.ExploreModeIterationCount, (int)e.NewValue);
        }

        private void OnTutorialClick(object sender, RoutedEventArgs e)
        {
            MainWindow.ToggleFlyout(0);
            MainWindow.ShowTutorial();
            
        }
    }
}