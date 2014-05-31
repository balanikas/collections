using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfClient.Tutorial
{
    public class Tutorial
    {
        private readonly Control _root;
        private IEnumerator<KeyValuePair<int, TutorialStep>> _enumerator;

        public Tutorial(Control root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            Steps = new SortedList<int, TutorialStep>();
            _root = root;
        }

        public SortedList<int, TutorialStep> Steps { get; private set; }

        public string StartText { get; set; }
        public string EndText { get; set; }

        public string CloseButtonText { get; set; }

        public void AddStep<T>(string controlName, int id, StepOptions options = null, Action preAction = null,
            Action postAction = null)
            where T : DependencyObject
        {
            var control = _root.FindName(controlName) as DependencyObject;
            if (control == null)
            {
                control = LogicalTreeHelper.FindLogicalNode(_root, controlName);
            }

            if (control == null)
            {
                throw new Exception(string.Format("Did not find a control with name '{0}'", controlName));
            }
            Steps.Add(id, new TutorialStep(control, options, preAction, postAction));
        }

        public void AddStep(UIElement control, int id, StepOptions options = null, Action preAction = null,
            Action postAction = null)
        {
            Steps.Add(id, new TutorialStep(control, options, preAction, postAction));
        }

        public void Clear()
        {
            foreach (var step in Steps)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(step.Value.Control);
                if (adornerLayer != null)
                {
                    var adorners = adornerLayer.GetAdorners(step.Value.Control);
                    if (adorners != null)
                    {
                        adornerLayer.Remove(adorners[0]);
                    }
                }
            }
        }

        public TutorialStep GetNextStep()
        {
            if (_enumerator == null)
            {
                _enumerator = Steps.GetEnumerator();
            }

            UIElement control;
            if (_enumerator.Current.Value != null)
            {
                control = _enumerator.Current.Value.Control;
                if (control != null)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(control);
                    if (adornerLayer != null)
                    {
                        var adorners = adornerLayer.GetAdorners(control);
                        if (adorners != null)
                        {
                            adornerLayer.Remove(adorners[0]);
                        }
                    }
                }
            }

            _enumerator.MoveNext();

            if (_enumerator.Current.Value != null)
            {
                if (_enumerator.Current.Value.PreAction != null)
                {
                    _enumerator.Current.Value.PreAction();
                }
                control = _enumerator.Current.Value.Control;
                if (control != null)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(control);
                    if (adornerLayer != null)
                    {
                        adornerLayer.Add(new CustomAdorner(control));
                    }
                }
            }

            return _enumerator.Current.Value;
        }

        private T FindChild<T>(DependencyObject parent, string childName)
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
