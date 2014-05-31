using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfClient.Shapes;

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
                shape = new CustomEllipse(canvas.Children, location);
            }
            else if (drawType == DrawTypes.Rectangle)
            {
                shape = new CustomRect(canvas.Children, location);
            }

          
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

            if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
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

        //public static void AddPixelation(FrameworkElement el, double pixelation)
        //{
        //    var pixelateEffect = el.Effect as PixelateEffect;
        //    if (pixelateEffect == null)
        //    {
        //        pixelateEffect = new PixelateEffect { Pixelation = 0 };
        //        el.Effect = pixelateEffect;
        //    }
        //    if (pixelateEffect.Pixelation <= 0.8)
        //    {
        //        pixelateEffect.Pixelation += pixelation;
        //    }
        //}

        public static Control FindChild(Control parent, string controlName)
        {
             var control = parent.FindName(controlName) as DependencyObject;
            if (control == null)
            {
                control = LogicalTreeHelper.FindLogicalNode(parent, controlName);
            }

            return (Control)control;
        }

        public static T FindChild<T>(DependencyObject parent, string childName)
             where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                T childType = child as T;
                if (childType == null)
                {

                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;

                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
    }
}
