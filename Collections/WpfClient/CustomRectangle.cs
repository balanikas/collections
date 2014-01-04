using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            _grid.ContextMenu = Application.Current.Resources["ctxMenu"] as ContextMenu;
            _grid.MouseEnter += _grid_MouseEnter;
            _grid.MouseLeave += _grid_MouseLeave;
        }



        void _grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _rect.StrokeThickness = 0;
        }

        void _grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _rect.StrokeThickness = 1;
            _rect.Stroke = Brushes.Black;
        }



        public override void Draw()
        {
            base.Draw();
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
                if(i.Progress >= 100)
                {
                    _animationsHelper.Pause(_grid);
                    _rect.Fill = new SolidColorBrush(Colors.Gray);
                    return;
                }
              
            })));
                
        }

    }
}
