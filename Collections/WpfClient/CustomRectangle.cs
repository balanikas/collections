using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Expression.Media.Effects;

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
            
            
            _grid.MouseEnter += _grid_MouseEnter;
            _grid.MouseLeave += _grid_MouseLeave;

            _grid.Children.Add(_rect);
            _grid.Children.Add(_label);
        }



        void _grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _rect.StrokeThickness = 0;
            _rect.Opacity = 0.7;
        }

        void _grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _rect.StrokeThickness = 1;
            _rect.Stroke = Brushes.Black;
            _rect.Opacity = 1.0;
        }


        public override void Draw()
        {
            base.Draw();
            
            _animationsHelper.AddGrowthAnimation(_grid, TimeSpan.FromSeconds(10), 1, 3);
            _animationsHelper.AddColorAnimation(_rect, TimeSpan.FromSeconds(10), Colors.Green, Colors.Red);
        }

        private void Freeze()
        {
            _animationsHelper.Pause(_grid);
            _animationsHelper.Pause(_rect);
            //_rect.Fill = new SolidColorBrush(Colors.Gray);
            _rect.Opacity = 0.7;
        }
        public override void Update(CollectionsSOLID.UIMessage i)
        {
            _rect.Dispatcher.BeginInvoke((new Action(delegate()
            {
                _label.Content = i.ToString();
                if(i.Progress >= 100)
                {
                    Freeze();
                }
                if (!i.MethodExecution.Success)
                {
                    _animationsHelper.AddPixelation(_rect,0.01);
                }
              
            })));
                
        }

    }
}
