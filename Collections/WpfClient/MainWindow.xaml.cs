using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CollectionsSOLID;
using MahApps.Metro;
using MahApps.Metro.Controls;
using System.Globalization;
using System.Diagnostics;
using WpfClient.Views;
using System.Linq;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Pages _pages;
        class Pages
        {
            private ExploreMode _exploreMode;
            private PlayMode _playMode;


            public UserControl ExploreModeView
            {
                get
                {
                    if (_exploreMode == null)
                        _exploreMode = new ExploreMode();
                    return _exploreMode;
                }
            }
            public UserControl PlayModeView
            {
                get
                {
                    if (_playMode == null)
                        _playMode = new PlayMode();
                    return _playMode;
                }
            }
        }

        static MainWindow _self;

      

        public MainWindow()
        {
            InitializeComponent();

            _self = this;
           _pages = new Pages();
        }

        public static void ShowProgressBar()
        {
            _self.PrgBar.Visibility = Visibility.Visible;
        }
        public static void HideProgressBar()
        {
            _self.PrgBar.Visibility = Visibility.Hidden;
        }

        public static void ChangeView(UserControl view)
        {
            //_self.ModeContent.Content = view;
        }


      

      

      


        public static void ToggleFlyout(int index, IRunner userState = null, bool keepOpenIfOpened = false)
        {
            var flyout = _self.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            if(flyout is RunnerInfoFlyout && userState != null)
            {
                ((RunnerInfoFlyout)flyout).AddContent(userState.GetState());
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(0);
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(2);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var theme = ThemeManager.DetectTheme(Application.Current);

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
