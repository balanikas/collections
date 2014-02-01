using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Collections;

namespace WpfClient
{
    /// <summary>
    ///     Interaction logic for UCTypes.xaml
    /// </summary>
    public partial class UCTypes : UserControl
    {
        private readonly TypesLoader _typesLoader;

        public UCTypes()
        {
            InitializeComponent();
            _typesLoader = new TypesLoader(Logger);

#if DEBUG
            txtFolderLocation.Text =
                @"C:\Users\grillo\Documents\GitHub\collections\Collections\Samples\sourcefiles";
            txtAssemblyLocation.Text = @"C:\dev\collections\Collections\Collections\bin\Debug\Collections.dll";
#endif
        }

        public ILogger Logger { get;set; }

        public LoadedType SelectedType
        {
            get { return lstTypes.SelectedItem != null ? (LoadedType) lstTypes.SelectedItem : null; }
        }

        public List<MethodInfo> SelectedMethods
        {
            get
            {
                var result = new List<MethodInfo>();
                foreach (object item in lstMethods.SelectedItems)
                {
                    result.Add(((KeyValuePair<string, MethodInfo>) item).Value);
                }
                return result;
            }
        }

        public void Refresh()
        {
        }


        private void ContentSelectionUpdated(object sender, RoutedEventArgs e)
        {
            LoadTypes();
        }

        private async void LoadTypes()
        {
            MainWindow.ShowProgressBar();

            bool loadBclTypes = chkBclTypes.IsChecked == true;
            bool loadAssemblyTypes = chkFromAssembly.IsChecked == true;
            bool loadFileTypes = chkFromFiles.IsChecked == true;

            var allTypes = new List<LoadedType>();
            if (loadBclTypes)
            {
                List<LoadedType> types = await _typesLoader.FromBCLAsync();
                allTypes.AddRange(types);
            }
            if (loadAssemblyTypes)
            {
                if (IsValidAssembly(txtAssemblyLocation.Text))
                {
                    List<LoadedType> types = await _typesLoader.FromAssemblyAsync(txtAssemblyLocation.Text);
                    allTypes.AddRange(types);
                }
            }
            if (loadFileTypes)
            {
                if (IsValidFolder(txtFolderLocation.Text))
                {
                    List<LoadedType> types = await _typesLoader.FromDiscAsync(txtFolderLocation.Text);
                    allTypes.AddRange(types);
                }
            }


            lstTypes.ItemsSource = allTypes;
            if (allTypes.Count > 0)
            {
                lstTypes.SelectedIndex = 0;
            }
            MainWindow.HideProgressBar();
        }

        private bool IsValidAssembly(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
            {
                return false;
            }
            string extension = Path.GetExtension(txtAssemblyLocation.Text);
            if (extension == ".dll" ||
                extension == ".exe")
            {
                return true;
            }
            return false;
        }

        private bool IsValidFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                return true;
            }
            return false;
        }

        private void lstObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstTypes.SelectedItem == null)
            {
                lstMethods.ItemsSource = null;
                SetCodeText(String.Empty);
                return;
            }
            var selectedType = (LoadedType) lstTypes.SelectedItem;

            SetCodeText(selectedType.Source);


            var actions = new Dictionary<string, MethodInfo>();
            foreach (MethodInfo method in selectedType.TypeInfo.GetMethods())
            {
                actions.Add(method.ToString(), method);
            }

            lstMethods.ItemsSource = actions; 
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
            var type = (LoadedType) lstTypes.SelectedItem;

            if (_typesLoader.TryCompileFromSource(avalonEdit.Text))
            {
                _typesLoader.SaveType(new LoadedType
                {
                    FilePath = type.FilePath,
                    Source = avalonEdit.Text,
                    TypeInfo = type.TypeInfo
                });

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


        private void txtFolderLocation_Drop(object sender, DragEventArgs e)
        {
            var target = sender as TextBox;
            if (target != null)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var folderList = (string[]) e.Data.GetData(DataFormats.FileDrop, false);
                    string folder = folderList[0];

                    FileAttributes attr = File.GetAttributes(folder);
                    if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                    {
                        folder = Path.GetDirectoryName(folder);
                    }

                    txtFolderLocation.Text = folder;
                    LoadTypes();
                }
            }
        }


        private void TxtFolderLocation_OnPreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }


        private void txtAssemblyLocation_Drop(object sender, DragEventArgs e)
        {
            var target = sender as TextBox;
            if (target != null)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var itemList = (string[]) e.Data.GetData(DataFormats.FileDrop, false);
                    string item = itemList[0];

                    FileAttributes attr = File.GetAttributes(item);
                    if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                    {
                        if (Path.GetExtension(item) == ".dll" || Path.GetExtension(item) == ".exe")
                        {
                            txtFolderLocation.Text = item;
                            LoadTypes();
                        }
                    }
                }
            }
        }

        private void TxtAssemblyLocation_OnPreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}