using System;
using System.IO;
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

                vm.Types.FilesPath = Path.Combine(Environment.CurrentDirectory, "samples");

                TypesView.DataContext = vm.Types;
            };
           
        }

        
    }
}