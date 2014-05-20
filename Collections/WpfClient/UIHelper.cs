using System;
using System.IO;
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

        public static bool IsPathValid(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return false;
            }

            if (Directory.Exists(path))
            {
                return true;
            }
            
            if (File.Exists(path))
            {
                var  isCompilableFile = path.EndsWith(".cs") || path.EndsWith(".vb");
                if (isCompilableFile)
                {
                    return true;
                }
                var isAssembly = path.EndsWith(".dll") || path.EndsWith(".exe");
                if (isAssembly)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
