using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfClient
{
    public class UIHelper
    {
        internal static CustomShape CreateDrawingShape(Canvas canvas)
        {
            var location = Mouse.GetPosition(canvas);
            return CreateDrawingShape(canvas, location);
        }

        internal static CustomShape CreateDrawingShape(Canvas canvas, Point location)
        {
            
            CustomShape shape = null;

            var drawType = Settings.Instance.Get(Settings.Keys.DrawAs);
            if (drawType == DrawTypes.Circle)
            {
                shape = new CustomCircle(canvas.Children, location);
            }
            else if (drawType == DrawTypes.Rectangle)
            {
                shape = new CustomRectangle(canvas.Children, location);
            }

            shape.OnMouseOver += (source, args) =>
            {
                
            };
            //shape.OnKeyPressed += (source, args) =>
            //{
            //    if (args.Key == Key.D1)
            //    {
            //        runtime.Runners.RemoveById(args.EventInfo);
            //    }
            //    else if (args.Key == Key.D2)
            //    {
            //        MainWindow.ToggleFlyout(1, runtime.Runners.GetById(args.EventInfo));
            //    }
            //};

            return shape;
        }
    }
}
