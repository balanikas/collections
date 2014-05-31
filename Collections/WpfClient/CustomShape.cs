using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Collections;
using Collections.Messages;

namespace WpfClient
{
    public abstract class CustomShape : IGui
    {
        public event RightClickEventHandler OnRightClick;
        public event LeftClickEventHandler OnLeftClick;
        public string Id { get; set; }
        public abstract void Initialize();
        public abstract void Update(MethodExecutionMessage message);
        public abstract void Destroy();

        public abstract void AddContextMenu(ContextMenu ctxMenu);

        protected void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (OnRightClick != null)
                {
                    OnRightClick(this, new RightClickEventArgs(Id));
                }
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (OnLeftClick != null)
                {
                    OnLeftClick(this, new LeftClickEventArgs(Id));
                }
            }
        }


    }
}