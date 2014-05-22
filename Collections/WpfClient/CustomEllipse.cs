using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Collections.Messages;

namespace WpfClient
{
    internal class CustomEllipse : CustomShape
    {
        public Controls.CustomEllipse _control;
        private UIElementCollection _parent;
        public override void Destroy()
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                _parent.Remove(_control);
            }));
            
        }

        public override void AddContextMenu(ContextMenu ctxMenu)
        {
            _control.LayoutRoot.ContextMenu = ctxMenu;
        }

       
        public CustomEllipse(UIElementCollection parent, Point position)
        {
            _parent = parent;
            
            _control = new Controls.CustomEllipse();
            _parent.Add(_control);
            _control.SetValue(Canvas.LeftProperty, position.X - (_control.LayoutRoot.Width / 2));
            _control.SetValue(Canvas.TopProperty, position.Y - (_control.LayoutRoot.Height / 2));
            _control.LayoutRoot.MouseDown += OnMouseDown;
          
        }


        public override void Initialize()
        {
            _control.LayoutRoot.Visibility = Visibility.Visible;
            _control.Freeze = false;
            var sb = _control.Resources["SBGrowth"] as Storyboard;
            sb.Begin();
            
        }


        public override void Update(MethodExecutionMessage msg)
        {

            _control.UpdateText = msg.ToString();
                
            if (msg.Progress >= 100)
            {
                _control.Freeze = true;
            }
        }
    }
}