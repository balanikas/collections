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

#if DEBUG
                vm.Types.FilesPath =
                    @"C:\Users\grillo\Documents\GitHub\Espera\Espera\Espera.Core\bin\Debug\Espera.Core.dll";
                //vm.Types.FilesPath = Path.Combine(Environment.CurrentDirectory, "samples");
#endif
                TypesView.DataContext = vm.Types;
            };
           
        }

        
    }
}