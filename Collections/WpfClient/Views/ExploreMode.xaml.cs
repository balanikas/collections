using System.Windows.Controls;
using WpfClient.ViewModels;

namespace WpfClient.Views
{
    public partial class ExploreMode : UserControl
    {
        public ExploreMode()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                ExploreModeViewModel vm = ViewModelLocator.ExploreMode;
                DataContext = vm;

#if DEBUG
                vm.Types.FilesPath = @"C:\dev\GitHub\collections\Collections\Samples\sourcefiles";
#endif
                TypesView.DataContext = vm.Types;
            };
           
        }

        
    }
}