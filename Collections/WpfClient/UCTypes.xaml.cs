using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CollectionsSOLID;


namespace WpfClient
{
    /// <summary>
    /// Interaction logic for UCTypes.xaml
    /// </summary>
    public partial class UCTypes : UserControl
    {
        public UCTypes()
        {
            InitializeComponent();
            
            
        }

        public LoadedType SelectedType
        {
            get
            {
                return lstTypes.SelectedItem != null ? (LoadedType)lstTypes.SelectedItem : null;
            }
        }

        public List<MethodInfo> SelectedMethods
        {
            get
            {
                var result = new List<MethodInfo>();
                foreach (var item in lstMethods.SelectedItems)
	            {
                    result.Add(((KeyValuePair<string, MethodInfo>)item).Value);
	            }
                return result;
            }
        }

        public void Refresh()
        {
            
        }

        private void chkBclTypes_Checked(object sender, RoutedEventArgs e)
        {
            LoadTypes();
        }

        private void chkFromAssembly_Checked(object sender, RoutedEventArgs e)
        {
            LoadTypes();
        }

        private void chkFromFiles_Checked(object sender, RoutedEventArgs e)
        {
            LoadTypes();
        }

        private async void LoadTypes()
        {
            MainWindow.ShowProgressBar();

            var loadBclTypes = chkBclTypes.IsChecked == true;
            var loadAssemblyTypes = chkFromAssembly.IsChecked == true;
            var loadFileTypes = chkFromFiles.IsChecked == true;

            var allTypes = new List<LoadedType>();
            if (loadBclTypes)
            {
                var types = await TypesLoader.FromBCLAsync();
                allTypes.AddRange(types);
            }
            if (loadAssemblyTypes)
            {
                var types = await TypesLoader.FromAssemblyAsync(txtAssemblyLocation.Text);
                allTypes.AddRange(types);
            }
            if (loadFileTypes)
            {
                var types = await TypesLoader.FromDiscAsync(txtFolderLocation.Text);
                allTypes.AddRange(types);
            }


            lstTypes.ItemsSource = allTypes;
            if (allTypes.Count > 0)
            {
                lstTypes.SelectedIndex = 0;
            }
            MainWindow.HideProgressBar();

        }

        private void chkFromAssembly_Unchecked(object sender, RoutedEventArgs e)
        {
            LoadTypes();
        }

        private void chkFromFiles_Unchecked(object sender, RoutedEventArgs e)
        {
            LoadTypes();
        }

        private void chkBclTypes_Unchecked(object sender, RoutedEventArgs e)
        {
            LoadTypes();
        }

        private void lstObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstTypes.SelectedItem == null)
            {
                lstMethods.ItemsSource = null;
                SetCodeText(String.Empty);
                return;
            }
            var selectedType = (LoadedType)lstTypes.SelectedItem;

            SetCodeText(selectedType.Source);


            var actions = new Dictionary<string,MethodInfo>();
            foreach (MethodInfo method in selectedType.TypeInfo.GetMethods())
            {
                actions.Add(method.ToString(), method);
              
            }

            lstMethods.ItemsSource = actions; // selectedType.Key.GetMethods().Select(x => x.Name);
            if (actions.Count > 0)
            {
                lstMethods.SelectedIndex = 0;
            }

            btnCompile.IsEnabled = selectedType.IsCompilable;
            avalonEdit.IsEnabled = selectedType.IsCompilable;
        }

        private void btnCompile_Click(object sender, RoutedEventArgs e)
        {
            if (lstTypes.SelectedItem == null)
            {
                return;
            }
            var type = (LoadedType)lstTypes.SelectedItem;

            if (CollectionsSOLID.TypesLoader.TryCompileFromSource(avalonEdit.Text))
            {
                CollectionsSOLID.TypesLoader.SaveType(new LoadedType { FilePath = type.FilePath, Source = avalonEdit.Text, TypeInfo = type.TypeInfo });

                LoadTypes();
            }
            else
            {
                MessageBox.Show("Compilation error");
            }
           
        }

        private void SetCodeText(string source)
        {
            avalonEdit.Text = source;
        }
    }
}
