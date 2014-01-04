using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

            _grid.MouseEnter += _grid_MouseEnter;
            _grid.MouseLeave += _grid_MouseLeave;

            _grid.Children.Add(_ellipse);
            _grid.Children.Add(_label);
            
        }



        void _grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _ellipse.StrokeThickness = 0;
            _ellipse.Opacity = 0.7;
        }

        void _grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _ellipse.StrokeThickness = 1;
            _ellipse.Stroke = Brushes.Black;
            _ellipse.Opacity = 1.0;
        }

        public override void Draw() 
        {
            base.Draw();
            
           
            _animationsHelper.AddGrowthAnimation(_grid, TimeSpan.FromSeconds(10), 1, 3);
            _animationsHelper.AddColorAnimation(_ellipse, TimeSpan.FromSeconds(10), Colors.Green, Colors.Red);
        }

        private void Freeze()
        {
            _animationsHelper.Pause(_grid);
            _animationsHelper.Pause(_ellipse);
            _ellipse.Opacity = 0.7;
        }

        public override void Update(CollectionsSOLID.Message msg)
        {
            _ellipse.Dispatcher.BeginInvoke((new Action(delegate()
            {
                _label.Content = msg.ToString();
                if (msg.Progress >= 100)
                {

                    Freeze();
                }

            })));

        }

      
    }
}
