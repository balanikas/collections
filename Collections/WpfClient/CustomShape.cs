using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Collections;
using Collections.Messages;

namespace WpfClient
{
    internal class CustomShape : IGui
    {
        protected AnimationsHelper _animationsHelper;
        protected Grid _grid;
        protected Label _label;
        private UIElementCollection _parent;

        public CustomShape(UIElementCollection parent, Point position, string title = "")
        {
            _label = new Label();
            _label.FontSize = 6;
            _grid = new Grid();
            _grid.Visibility = Visibility.Collapsed;
            _grid.Width = 100;
            _grid.Height = 100;
            _grid.SetValue(Canvas.LeftProperty, position.X - (_grid.Width/2));
            _grid.SetValue(Canvas.TopProperty, position.Y - (_grid.Height/2));
            _grid.MouseDown += _grid_MouseDown;
            _grid.MouseMove += _grid_MouseMove;
            _grid.KeyDown += _grid_KeyDown;


            _label.Content = title;
            _animationsHelper = new AnimationsHelper();

            _parent = parent;
            _parent.Add(_grid);
        }

        public string Id { get; set; }


        public virtual void Initialize()
        {
            _grid.Visibility = Visibility.Visible;
        }

        public virtual void Update(MethodExecutionMessage methodExecutionMessage)
        {
        }

     

        public void Destroy()
        {
            //if (_grid.Parent != null)
            {
                _grid.Dispatcher.Invoke(() =>
                {
                    if (_grid.Parent != null)
                    {
                        UIElementCollection children = ((Canvas)_grid.Parent).Children;
                        if (children != null)
                        {
                            children.Remove(_grid);
                        }
                    }
                    
                });
            }
        }

        public event RightClickEventHandler OnRightClick;
        public event LeftClickEventHandler OnLeftClick;
        public event MouseOverEventHandler OnMouseOver;
        public event KeyPressedEventHandler OnKeyPressed;

        public void AddContextMenu(ContextMenu ctxMenu)
        {
            _grid.ContextMenu = ctxMenu;
        }


        private void _grid_MouseDown(object sender, MouseButtonEventArgs e)
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

        private void _grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (OnMouseOver != null)
            {
                OnMouseOver(this, new MouseOverEventArgs(Id));
            }
        }


        private void _grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (OnKeyPressed != null)
            {
                OnKeyPressed(this, new KeyPressedEventArgs(Id, e.Key));
            }
        }
    }
}