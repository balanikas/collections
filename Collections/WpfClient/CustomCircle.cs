using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WpfClient
{

   

    class CustomCircle : CustomShape
    {

        Ellipse _ellipse;

        public CustomCircle(UIElementCollection parent, Point position, string title = "")
            :base(parent,position)
        {
            _ellipse = new Ellipse();
           
           
            _ellipse.Fill = new SolidColorBrush(Colors.Pink);
        }

       

        public override void Init() 
        {
            base.Init();
            _grid.Children.Add(_ellipse);
            _grid.Children.Add(_label);
           
            _animationsHelper.AddGrowthAnimation(_grid, TimeSpan.FromSeconds(10), 1, 3);
            _animationsHelper.AddColorAnimation(_ellipse, TimeSpan.FromSeconds(10), Colors.Green, Colors.Red);
        }

        public override void Update(CollectionsSOLID.Message msg)
        {
            _ellipse.Dispatcher.BeginInvoke((new Action(delegate()
            {
                _label.Content = msg.ToString();
                if (msg.Progress >= 100 || msg.ObjectState == CollectionsSOLID.ObjectState.Finished)
                {
                    _animationsHelper.Pause(_grid);
                    _animationsHelper.Pause(_ellipse);
                    _ellipse.Fill = new SolidColorBrush(Colors.Gray);
                    return;
                }

            })));

        }

      
    }
}
