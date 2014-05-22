using System.Windows.Controls;
using WpfClient.ViewModels;

namespace WpfClient.Views
{
    public partial class PlayMode : UserControl
    {
        public PlayMode()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                PlayModeViewModel vm = ViewModelLocator.PlayModeMode;
                DataContext = vm;
            };

        }
    }
}