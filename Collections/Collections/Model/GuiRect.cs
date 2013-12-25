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
using Brush = System.Drawing.Brush;

namespace Collections
{
    class GuiRect : IGuiObject
    {
        private Rectangle m_rect;

        public GuiRect()
        {
            m_rect = new Rectangle();
        }
        public void AddParent(UIElementCollection parent)
        {
            parent.Add(m_rect);
        }

        public void SetHeight(int value)
        {
            //m_rect.SetValue(Canvas.HeightProperty, value);
            m_rect.Height = value;
        }

        public void SetWidth(int value)
        {
            //m_rect.SetValue(Canvas.WidthProperty, value);
            m_rect.Width = value;
        }

        public void Init()
        {
            m_rect.Width = 20;
            m_rect.Height = 20;
            
        }

        

        public void Destroy()
        {
            if (m_rect.Parent != null)
            {
                ((Canvas)m_rect.Parent).Children.Remove(m_rect);
            }
            
        }

        public void SetColor(Color color)
        {
           m_rect.Fill = new SolidColorBrush(color);
        }

        public void SetPosition(double x, double y)
        {
           m_rect.SetValue(Canvas.LeftProperty,x);
           m_rect.SetValue(Canvas.BottomProperty, y);
        }

        public void Move(double x, double y)
        {
            m_rect.SetValue(Canvas.LeftProperty, (double)m_rect.GetValue(Canvas.LeftProperty) +  x);
            m_rect.SetValue(Canvas.BottomProperty, (double)m_rect.GetValue(Canvas.BottomProperty) + y);
        }

        public Rect GetBounds()
        {
            Point location = new Point((double)m_rect.GetValue(Canvas.LeftProperty), (double)m_rect.GetValue(Canvas.BottomProperty));
            Size size = new Size((double) m_rect.GetValue(Canvas.WidthProperty),
                (double) m_rect.GetValue(Canvas.HeightProperty));


            Rect rectangleBounds = new Rect(location, size);
            //rectangleBounds = m_rect.RenderTransform.TransformBounds(new Rect(m_rect.RenderSize));
            return rectangleBounds;
        }

        public void AddAnimation()
        {
            var height = new DoubleAnimation()
            {
                From = 50,
                To = 150,
               RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = true,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            var width = new DoubleAnimation()
            {
                From = 50,
                To = 150,
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = true,
                Duration = TimeSpan.FromMilliseconds(300)
            };



            Storyboard.SetTarget(height, m_rect);
            Storyboard.SetTarget(width, m_rect);
            Storyboard.SetTargetProperty(height, new PropertyPath(Button.WidthProperty));
            Storyboard.SetTargetProperty(width, new PropertyPath(Button.HeightProperty));

            var sb = new Storyboard();
            sb.Children.Add(width);
            sb.Children.Add(height);

            sb.Begin();
        }

       
     
    }
}
