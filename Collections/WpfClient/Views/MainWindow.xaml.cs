using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Collections.Runtime;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace WpfClient
{
    public partial class MainWindow : MetroWindow
    {
        private static MainWindow _self;

        public MainWindow()
        {
            InitializeComponent();
            
            DataContext = ViewModelLocator.MainWindow;
            _self = this;
        }

        public static Task<ProgressDialogController> ShowProgress(string title, string message)
        {
            return _self.ShowProgressAsync(title, message);
            
        }

        public static void ToggleFlyout(int index,bool keepOpenIfOpened = false)
        {
            var flyout = _self.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }
            if (keepOpenIfOpened)
            {
                flyout.IsOpen = true;
            }
            else
            {
                flyout.IsOpen = !flyout.IsOpen;
            }
        }

    }
}