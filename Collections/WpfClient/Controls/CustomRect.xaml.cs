using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfClient.Controls
{
    /// <summary>
    /// Interaction logic for CustomCircle.xaml
    /// </summary>
    public partial class CustomRect : UserControl
    {
        public CustomRect()
        {
            InitializeComponent();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            Shape.StrokeThickness = 1;
            Shape.Stroke = Brushes.Black;
            Shape.Opacity = 1.0;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Shape.StrokeThickness = 0;
            Shape.Opacity = 0.7;
            
        }


        public static readonly DependencyProperty FreezeProperty =
            DependencyProperty.Register("Freeze", typeof(bool), typeof(CustomRect), new PropertyMetadata(true));
        public bool Freeze
        {
            get { return (bool)this.GetValue(FreezeProperty); }
            set
            {
                this.SetValue(FreezeProperty, value);
                if (value)
                {
                    var sb = Resources["SBGrowth"] as Storyboard;
                    sb.Pause();
                    Shape.Opacity = 0.7;
                }
                else
                {
                    //var sb = Resources["SBColor"] as Storyboard;
                    //sb.Resume();
                    //Shape.Opacity = 1.0;
                }
               
            }
        }

        public static readonly DependencyProperty UpdateTextProperty =
            DependencyProperty.Register("UpdateText", typeof(string), typeof(CustomRect), new PropertyMetadata(""));
        public string UpdateText
        {
            get { return (string)this.GetValue(UpdateTextProperty); }
            set
            {
                this.SetValue(UpdateTextProperty, value);
                Text.Content = value;
            }
        }

       
    }
}
