using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Collections;
using MahApps.Metro.Controls;

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

        private void sldLoopCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Loops = (int) e.NewValue;
        }

        private void cmbGraphics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.DrawAs = (DrawTypes) cmbGraphics.SelectedItem;
        }

        private void radUseTPL_Checked(object sender, RoutedEventArgs e)
        {
            if (radUseBW.IsChecked == true)
            {
                Settings.ThreadingType = ObjectType.BackgroundWorkerBased;
            }
            else
            {
                Settings.ThreadingType = ObjectType.ParallelTaskBased;
            }
        }
    }
}