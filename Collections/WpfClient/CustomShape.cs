﻿using System;
using System.Collections.Generic;
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

namespace WpfClient
{
    class CustomShape : CollectionsSOLID.IGui
    {
        public event RightClickEventHandler OnRightClick;
        public event MouseOverEventHandler OnMouseOver;

        protected AnimationsHelper _animationsHelper;
        protected Grid _grid;
        protected Label _label;
        UIElementCollection _parent;

        public string Id { get; set; }

        public CustomShape(UIElementCollection parent, Point position, string title = "")
        {
            _label = new Label();
            _label.FontSize = 4;
            _grid = new Grid();
            

            _grid.Width = 70;
            _grid.Height = 70;
            _grid.SetValue(Canvas.LeftProperty, position.X - (_grid.Width / 2));
            _grid.SetValue(Canvas.TopProperty, position.Y - (_grid.Height / 2));
            _grid.MouseDown += _grid_MouseDown;
            _grid.MouseMove += _grid_MouseMove;

            

            _label.Content = title;
            _animationsHelper = new AnimationsHelper();

            _parent = parent;
        }


       

        public virtual void Draw()
        {
            _parent.Add(_grid);
        }

        public virtual void Update(CollectionsSOLID.Message msg)
        {

        }

        public void Destroy()
        {
            if (_grid.Parent != null)
            {
                _grid.Dispatcher.Invoke(new Action(() =>
                {
                    var children = ((Canvas)_grid.Parent).Children;
                    if (children != null)
                    {
                        children.Remove(_grid);
                    }

                }));

            }

        }

        void _grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                if (OnRightClick != null)
                {
                    OnRightClick(this, new RightClickEventArgs(Id));
                }
            }
        }

        void _grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            

            if (OnMouseOver != null)
            {
                OnMouseOver(this, new MouseOverEventArgs(Id));
            }
        }

    }
}
