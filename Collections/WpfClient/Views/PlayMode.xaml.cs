using System.Windows.Controls;
using WpfClient.ViewModels;

namespace WpfClient.Views
{
    public partial class PlayMode : UserControl
    {
        public PlayMode()
        {
            InitializeComponent();
            PlayModeViewModel vm = ViewModelLocator.PlayModeMode;
            DataContext = vm;
        }
    }
}