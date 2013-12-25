using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Brush = System.Drawing.Brush;

namespace Collections
{
    class GuiMissile : IGuiObject
    {
        private ProgressBar m_progressBar;

        public GuiMissile()
        {
            m_progressBar = new ProgressBar();
        }
        public void AddParent(UIElementCollection parent)
        {
            parent.Add(m_progressBar);
        }

        public void SetHeight(int value)
        {
            m_progressBar.Value = value;
        }

        public void SetWidth(int value)
        {
            
        }

        public void Init()
        {
            m_progressBar.Height = 20;
            m_progressBar.Value = 0;
        }

        public void Destroy()
        {
            ((StackPanel)m_progressBar.Parent).Children.Remove(m_progressBar);
        }

        public void SetColor(Color color)
        {
            m_progressBar.Foreground = new SolidColorBrush(color);
        }

        public void SetPosition(double x, double y)
        {
            throw new NotImplementedException();
        }

        public void Move(double x, double y)
        {
            throw new NotImplementedException();
        }

        public Rect GetBounds()
        {

            Point location = new Point((double)m_progressBar.GetValue(Canvas.LeftProperty), (double)m_progressBar.GetValue(Canvas.BottomProperty));
            Size size = new Size((double)m_progressBar.GetValue(Canvas.WidthProperty),
                (double)m_progressBar.GetValue(Canvas.HeightProperty));


            Rect rectangleBounds = new Rect(location, size);
            //rectangleBounds = m_rect.RenderTransform.TransformBounds(new Rect(m_rect.RenderSize));
            return rectangleBounds;
        
        }

        public void AddAnimation()
        {
            throw new NotImplementedException();
        }
    }
}
