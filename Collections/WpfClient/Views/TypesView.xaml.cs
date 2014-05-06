using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Collections;

namespace WpfClient.Views
{
    /// <summary>
    ///     Interaction logic for UCTypes.xaml
    /// </summary>
    public partial class TypesView : UserControl
    {
      

        public TypesView()
        {
            InitializeComponent();

    

           
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

                  
                    //LoadTypes();
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
                           
                          //  LoadTypes();
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