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
using ColorCode;


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

        public Type SelectedType
        {
            get
            {
                return ((KeyValuePair<TypeInfo, string>)lstObjects.SelectedItem).Key;
            }
        }

        public List<MethodInfo> SelectedMethods
        {
            get
            {
                var result = new List<MethodInfo>();
                foreach (var item in lstActions.SelectedItems)
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

        private void LoadTypes()
        {
            Dictionary<TypeInfo, string> types = new Dictionary<TypeInfo, string>();

            if (chkBclTypes.IsChecked == true)
            {
                var t = CollectionsSOLID.Utils.LoadBclTypes();
                types = types.Union(t).ToDictionary(k => k.Key, v => v.Value);
            }
            if (chkFromAssembly.IsChecked == true)
            {
                var t = CollectionsSOLID.Utils.LoadTypesFromAssembly(txtAssemblyLocation.Text);
                types = types.Union(t).ToDictionary(k => k.Key, v => v.Value);
            }
            if (chkFromFiles.IsChecked == true)
            {
                var t = CollectionsSOLID.Utils.LoadTypesFromDisc(txtFolderLocation.Text);
                types = types.Union(t).ToDictionary(k => k.Key, v => v.Value);
            }


            lstObjects.ItemsSource = types;
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
            if (lstObjects.SelectedItem == null)
            {
                return;
            }
            var selectedType = (KeyValuePair<TypeInfo, string>)lstObjects.SelectedItem;

            SetCodeText(selectedType.Value);


            var actions = new Dictionary<string,MethodInfo>();
            foreach (MethodInfo method in selectedType.Key.GetMethods())
            {
                actions.Add(method.ToString(), method);
              
            }

            lstActions.ItemsSource = actions; // selectedType.Key.GetMethods().Select(x => x.Name);
        }

        private void btnCompile_Click(object sender, RoutedEventArgs e)
        {
            if (lstObjects.SelectedItem == null)
            {
                return;
            }
            var x = (KeyValuePair<TypeInfo, string>)lstObjects.SelectedItem;

            CollectionsSOLID.Utils.SaveType("", x.Key.ToString());

            var tt = CollectionsSOLID.Utils.LoadTypesFromDisc();
            lstObjects.ItemsSource = tt;
        }

        private void SetCodeText(string source)
        {

            string colorizedSourceCode = new CodeColorizer().Colorize(source, Languages.CSharp);

            FlowDocument myFlowDoc = new FlowDocument();

            // Add paragraphs to the FlowDocument.
            myFlowDoc.Blocks.Add(new Paragraph(new Run(colorizedSourceCode)));
            myFlowDoc.Blocks.Add(new Paragraph(new Run("Paragraph 2")));
            myFlowDoc.Blocks.Add(new Paragraph(new Run("Paragraph 3")));
            RichTextBox myRichTextBox = new RichTextBox();

            // Add initial content to the RichTextBox.
            txtCode.Document = myFlowDoc;
        }

        private string GetCodeText()
        {
            var textRange = new TextRange(txtCode.Document.ContentStart, txtCode.Document.ContentEnd);
            return textRange.Text;
        }

        private void SetCodeTextAsHtml(string source)
        {

            string colorizedSourceCode = new CodeColorizer().Colorize(source, Languages.CSharp);

           // codeEditor.NavigateToString(source);
        
        }



    }
}
