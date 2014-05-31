using System.Linq;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro;
using MahApps.Metro.Controls;

namespace WpfClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            CmdAbout = new RelayCommand(() => { MainWindow.ToggleFlyout(1); });

            CmdSettings = new RelayCommand(() => { MainWindow.ToggleFlyout(0); });
            
            CmdTabSelectionChanged = new RelayCommand< SelectionChangedEventArgs>(args =>
            {
                
                if (args == null || args.OriginalSource is TabControl == false)
                {
                    return;
                }

                var selectedItem = args.AddedItems[0] as MetroTabItem;
                if (selectedItem == null)
                {
                    return;
                }
                
                ChangeTheme(selectedItem.Name);
              
            });
        }

        private void ChangeTheme(string themeName)
        {

            var theme = ThemeManager.DetectAppStyle(Application.Current);

            string accentName;
            switch (themeName)
            {
                case "Blue":
                    ViewModelLocator.PlayModeMode.IsActivated = false;
                    ViewModelLocator.LogViewer.LogEntries.Clear();
                    accentName = "Blue";
                    break;
                case "Green":
                    ViewModelLocator.ExploreMode.IsActivated = false;
                    ViewModelLocator.LogViewer.LogEntries.Clear();
                    accentName = "Green";
                    break;
                default:
                    accentName = "Blue";
                    break;
            }
            Accent accent = ThemeManager.Accents.First(x => x.Name == accentName);
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
        }


        public RelayCommand CmdAbout { get; private set; }

        public RelayCommand CmdSettings { get; private set; }

        public RelayCommand<SelectionChangedEventArgs> CmdTabSelectionChanged { get; private set; }
    }
}