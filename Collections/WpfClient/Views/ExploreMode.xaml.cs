using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CollectionsSOLID;

namespace WpfClient.Views
{
    /// <summary>
    /// Interaction logic for ExploreMode.xaml
    /// </summary>
    public partial class ExploreMode : UserControl
    {
      
        Runtime _runtime;
        ILogger _logger;
        public ExploreMode()
        {
            InitializeComponent();

            _logger = new TextboxLogger(txtLog);

          
            Canvas.MouseDown += _canvas_MouseDown;
            //_canvas.Background = this.Resources["CanvasGameStarted"] as LinearGradientBrush;

            //gridGameArea.Children.Add(_canvas);

            _runtime = new Runtime();

            _runtime.Start();

        }


        private void btnStop_Click(object sender, RoutedEventArgs e)
        {


            _runtime.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }


        private void btnClearLog(object sender, RoutedEventArgs e)
        {
            txtLog.Document.Blocks.Clear();
        }



        private void RightClickHandler(object source, RightClickEventArgs e)
        {
            _runtime.Remove(e.EventInfo);
        }

        private void MouseOverHandler(object source, MouseOverEventArgs e)
        {

        }



        void _canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_runtime.IsRunning())
            {
                return;
            }
            Point p = Mouse.GetPosition(Canvas);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                CreateRunner(p);
            }
            //if (e.RightButton == MouseButtonState.Pressed)
            //{
            //    HitTestResult target = VisualTreeHelper.HitTest(_canvas, e.GetPosition(_canvas));

            //    while (!(target.VisualHit is Control) && (target != null))
            //    {
            //        var c = VisualTreeHelper.GetParent(target.VisualHit);
            //    }
            //}

        }


        private void CreateRunner(Point location)
        {
            LoadedType objectType = ucTypes.SelectedType;
            if (objectType == null)
            {
                return;
            }

            var methods = ucTypes.SelectedMethods;


            IGui gui = null;
            var drawType = Settings.DrawAs;
            if (drawType == DrawTypes.Circle)
            {
                var circle = new CustomCircle(Canvas.Children, location);


                circle.OnMouseOver += new MouseOverEventHandler(MouseOverHandler);
                circle.OnKeyPressed += circle_OnKeyPressed;
                gui = circle;
            }
            else if (drawType == DrawTypes.Rectangle)
            {
                var rectangle = new CustomRectangle(Canvas.Children, location);


                rectangle.OnMouseOver += new MouseOverEventHandler(MouseOverHandler);
                gui = rectangle;
            }

            IBehavior behavior;
            IRunner runner;
            try
            {
                behavior = new ObjectBehavior(objectType.TypeInfo, methods);
            }
            catch (Exception e)
            {
                _logger.ErrorNow(e.Message);
                //var guiMessage = new ErrorMessage(e.Message, 100);

                //runner = ObjectFactory.Get(Settings.ThreadingType, null, gui, _logger, Settings.Loops);
                //gui.Draw();
                //_runtime.Add(runner);
                //gui.Update(guiMessage);
                return;
            }

            runner = RunnerFactory.Get(Settings.ThreadingType, behavior, gui, _logger, Settings.Loops);

            var ctxMenu = ShapeContextMenu.Get(
                (s, e) => _runtime.Remove(runner.Id), 
                (s, e) => MainWindow.ToggleFlyout(1, _runtime.GetById(runner.Id), true),
                (s, e) => MessageBox.Show("shows the code that executes in the code explorer")
                );

            ((CustomShape)gui).AddContextMenu(ctxMenu);

            _runtime.Add(runner);
            runner.Start();

        }

        void circle_OnKeyPressed(object source, KeyPressedEventArgs e)
        {
            if (e.Key == Key.D1)
            {
                _runtime.Remove(e.EventInfo);
            }
            else if (e.Key == Key.D2)
            {
                MainWindow.ToggleFlyout(1, _runtime.GetById(e.EventInfo));
            }
        }
    }
}
