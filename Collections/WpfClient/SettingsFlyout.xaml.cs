using System.Collections.Generic;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Linq;
using CollectionsSOLID;

namespace WpfClient
{

    public partial class SettingsFlyout : Flyout, INotifyPropertyChanged
    {
      

        public SettingsFlyout()
        {

            InitializeComponent();
            
            var drawTypes = System.Enum.GetValues(typeof(DrawTypes)).Cast<DrawTypes>();
            cmbGraphics.ItemsSource = drawTypes;
        }

     

        private bool canCloseFlyout;

        public bool CanCloseFlyout
        {
            get { return this.canCloseFlyout; }
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

        private ICommand closeCmd;

        public ICommand CloseCmd
        {
            get
            {
                return this.closeCmd ?? (closeCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => this.CanCloseFlyout,
                    ExecuteDelegate = x => this.IsOpen = false
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

        private void sldLoopCount_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Loops = (int)e.NewValue;
        }

        private void cmbGraphics_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Settings.DrawAs = (DrawTypes)cmbGraphics.SelectedItem;
            
        }

        private void radUseTPL_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if(radUseBW.IsChecked == true)
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
