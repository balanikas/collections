using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using Autofac;
using Collections;
using WpfClient.ViewModels;

namespace WpfClient.Views
{
    public partial class ExploreMode : UserControl
    {
        public ExploreMode()
        {
            InitializeComponent();

            var vm = ViewModelLocator.ExploreMode;
            DataContext = vm;

#if DEBUG
            vm.Types.FilesPath = @"C:\dev\collections\Collections\Samples\sourcefiles";
#endif
            TypesView.DataContext = vm.Types;
            
        }     
    }
}