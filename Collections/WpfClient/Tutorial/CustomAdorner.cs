using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WpfClient.Tutorial
{
    public class CustomAdorner : Adorner
    {
        public CustomAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            Loaded += CustomAdorner_Loaded;
        }

        private void CustomAdorner_Loaded(object sender, RoutedEventArgs e)
        {
            var myDoubleAnimation = new DoubleAnimation
            {
                From = 0.6,
                To = 0.3,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(1)
            };

            var myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            Storyboard.SetTarget(myStoryboard, this);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(OpacityProperty));

            myStoryboard.Begin(this);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var adornedElementRect = new Rect(AdornedElement.RenderSize);
            var renderBrush = new SolidColorBrush(Colors.Green);

            var renderPen = new Pen(new SolidColorBrush(Colors.LawnGreen), 1.5);
            double renderRadius = 5.0;

            // Draw a circle at each corner.
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius,
                renderRadius);


            // Connect with lines
            drawingContext.DrawLine(renderPen, adornedElementRect.TopLeft, adornedElementRect.TopRight);
            drawingContext.DrawLine(renderPen, adornedElementRect.TopRight, adornedElementRect.BottomRight);
            drawingContext.DrawLine(renderPen, adornedElementRect.BottomRight, adornedElementRect.BottomLeft);
            drawingContext.DrawLine(renderPen, adornedElementRect.BottomLeft, adornedElementRect.TopLeft);
        }
    }
}
