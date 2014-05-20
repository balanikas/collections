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

        public static void ToggleFlyout(int index, IRunner userState = null, bool keepOpenIfOpened = false)
        {
            var flyout = _self.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            if (flyout is RunnerInfoFlyout && userState != null)
            {
                ((RunnerInfoFlyout) flyout).AddContent(userState.GetCurrentState());
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

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tuple<Theme, Accent> theme = ThemeManager.DetectTheme(Application.Current);

            string accentName;
            switch (TabControl.SelectedIndex)
            {
                case 0:
                    accentName = "Blue";
                    break;
                case 1:
                    accentName = "Green";
                    break;
                case 2:
                    accentName = "Purple";
                    break;
                default:
                    accentName = "Blue";
                    break;
            }
            Accent accent = ThemeManager.DefaultAccents.First(x => x.Name == accentName);
            ThemeManager.ChangeTheme(Application.Current, accent, theme.Item1);
        }
    }
}