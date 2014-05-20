﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Collections.Messages;

namespace WpfClient
{
    internal class CustomRectangle : CustomShape
    {
        private readonly Rectangle _rect;

        public CustomRectangle(UIElementCollection parent, Point position)
            : base(parent, position)
        {
            _rect = new Rectangle();
            _rect.Fill = new SolidColorBrush(Colors.Yellow);


            _grid.MouseEnter += _grid_MouseEnter;
            _grid.MouseLeave += _grid_MouseLeave;

            _grid.Children.Add(_rect);
            _grid.Children.Add(_label);
            _label.HorizontalAlignment = HorizontalAlignment.Stretch;
            _label.VerticalAlignment = VerticalAlignment.Stretch;
        }


        private void _grid_MouseLeave(object sender, MouseEventArgs e)
        {
            _rect.StrokeThickness = 0;
            _rect.Opacity = 0.7;
        }

        private void _grid_MouseEnter(object sender, MouseEventArgs e)
        {
            _rect.StrokeThickness = 1;
            _rect.Stroke = Brushes.Black;
            _rect.Opacity = 1.0;
        }


        public override void Initialize()
        {
            base.Initialize();

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

        public override void Update(MethodExecutionMessage i)
        {
            _rect.Dispatcher.BeginInvoke((new Action(delegate
            {
                _label.Content = i.ToString();
                if (i.Progress >= 100)
                {
                    Freeze();
                }
                if (!i.MethodExecutionResult.Success)
                {
                    _animationsHelper.AddPixelation(_rect, 0.01);
                }
            })));
        }
    }
}