using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Expression.Media.Effects;

namespace WpfClient
{
    internal class AnimationsHelper
    {
        private readonly Storyboard _storyboard;


        public AnimationsHelper()
        {
            _storyboard = new Storyboard();
        }

        public void Pause(FrameworkElement el)
        {
            _storyboard.Pause(el);
        }


        public void AddGrowthAnimation(FrameworkElement el, TimeSpan duration, double from, double to)
        {
            var scale = new ScaleTransform(1.0, 1.0);
            el.RenderTransformOrigin = new Point(0.5, 0.5);
            el.RenderTransform = scale;


            var easing = new ExponentialEase();
            easing.EasingMode = EasingMode.EaseOut;
            easing.Exponent = 10;

            var growthAnimationX = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = duration,
                EasingFunction = easing
            };
            var growthAnimationY = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = duration,
                EasingFunction = easing
            };


            _storyboard.Children.Add(growthAnimationY);
            _storyboard.Children.Add(growthAnimationX);

            Storyboard.SetTargetProperty(growthAnimationX, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(growthAnimationY, new PropertyPath("RenderTransform.ScaleY"));
            Storyboard.SetTarget(growthAnimationX, el);
            Storyboard.SetTarget(growthAnimationY, el);

            _storyboard.Begin(el, true);
        }

        public void AddColorAnimation(FrameworkElement el, TimeSpan duration, Color from, Color to)
        {
            var colorAnimation = new ColorAnimation
            {
                From = from,
                To = to,
                Duration = duration
            };

            _storyboard.Children.Add(colorAnimation);
            Storyboard.SetTargetProperty(colorAnimation,
                new PropertyPath("(" + el.GetType().Name + ".Fill).(SolidColorBrush.Color)"));
            Storyboard.SetTarget(colorAnimation, el);

            _storyboard.Begin(el, true);
        }

        public void AddPixelation(FrameworkElement el, double pixelation)
        {
            var pixelateEffect = el.Effect as PixelateEffect;
            if (pixelateEffect == null)
            {
                pixelateEffect = new PixelateEffect {Pixelation = 0};
                el.Effect = pixelateEffect;
            }
            if (pixelateEffect.Pixelation <= 0.8)
            {
                pixelateEffect.Pixelation += pixelation;
            }
        }
    }
}