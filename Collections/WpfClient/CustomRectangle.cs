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
    class CustomRectangle : CustomShape
    {
       
        Rectangle _rect;

        public CustomRectangle(UIElementCollection parent, Point position,string title = "")
            : base(parent, position)
        {
            _rect = new Rectangle();
            _rect.Fill = new SolidColorBrush(Colors.Yellow);

        }


        public override void Init()
        {
            base.Init();
            _grid.Children.Add(_rect);
            _grid.Children.Add(_label);
           
            _animationsHelper.AddGrowthAnimation(_grid, TimeSpan.FromSeconds(10), 1, 3);
            //_animationsHelper.AddColorAnimation(_rect, TimeSpan.FromSeconds(3), Colors.Green, Colors.Red);
        }

        public override void Update(CollectionsSOLID.Message i)
        {
            _rect.Dispatcher.BeginInvoke((new Action(delegate()
            {
                _label.Content = i.ToString();
                if(i.Progress >= 100 || i.ObjectState == CollectionsSOLID.ObjectState.Finished)
                {
                    _animationsHelper.Pause(_grid);
                    _rect.Fill = new SolidColorBrush(Colors.Gray);
                    return;
                }
              
            })));
                
        }

    }
}
