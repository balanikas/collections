using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CollectionsSOLID;

using MahApps.Metro.Controls;
using System.Globalization;
using System.Diagnostics;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        static MainWindow _self;

        Canvas _canvas;
        Runtime _runtime;
        ILogger _logger;

        public MainWindow()
        {
            InitializeComponent();

            _self = this;
            _logger = new TextboxLogger(txtLog);
            
            _canvas = new Canvas();
            _canvas.MouseDown += _canvas_MouseDown;
            _canvas.Background = this.Resources["CanvasGameStarted"] as LinearGradientBrush;

            gridGameArea.Children.Add(_canvas);

            _runtime = new Runtime();
            
            _runtime.Start();
        }

        public static void ShowProgressBar()
        {
            _self.prgBar.Visibility = Visibility.Visible;
        }
        public static void HideProgressBar()
        {
            _self.prgBar.Visibility = Visibility.Hidden;
        }

        private void CreateRunner(Point location)
        {
            LoadedType objectType = ucTypes.SelectedType;
            if(objectType == null)
            {
                return;
            }

            var methods = ucTypes.SelectedMethods;
           

            IGui gui = null;
            var drawType =  Settings.DrawAs;
            if(drawType == DrawTypes.Circle)
            {
                var circle = new CustomCircle(_canvas.Children, location);
                
             
                circle.OnMouseOver += new MouseOverEventHandler(MouseOverHandler);
                circle.OnKeyPressed += circle_OnKeyPressed;
                gui = circle;
            }
            else if (drawType == DrawTypes.Rectangle)
            {
                var rectangle = new CustomRectangle(_canvas.Children, location);


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
            
            var ctxMenu = ShapeContextMenu.Get((s, e) => _runtime.Remove(runner.Id), (s,e) => ToggleFlyout(1, _runtime.GetById(runner.Id)));
            ((CustomShape)gui).AddContextMenu(ctxMenu);

            _runtime.Add(runner);
            runner.Start();
            
        }

        void circle_OnKeyPressed(object source, KeyPressedEventArgs e)
        {
            if(e.Key == Key.D1)
            {
                _runtime.Remove(e.EventInfo);
            }
            else if (e.Key == Key.D2)
            {
                ToggleFlyout(1, _runtime.GetById(e.EventInfo));
            }
        }

        //void menuInfo_Click(object sender, RoutedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
        //void menuClose_Click(object sender, RoutedEventArgs e)
        //{
        //    _runtime.Remove(runner.Id);
        //}

        private void RightClickHandler(object source, RightClickEventArgs e)
        {
            _runtime.Remove(e.EventInfo);
        }

        private void MouseOverHandler(object source, MouseOverEventArgs e)
        {
            
        }


        private void btnStop_Click(object sender, RoutedEventArgs e)
        {

            
            _runtime.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }

        void _canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(!_runtime.IsRunning())
            {
                return;
            }
            Point p = Mouse.GetPosition(_canvas);
            if(e.LeftButton == MouseButtonState.Pressed)
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

        private void lstCollectionTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var x = (KeyValuePair<Type, List<string>>)lstCollectionTypes.SelectedItem;

            lstActions.ItemsSource = x.Value;
        }


        private void ToggleFlyout(int index, IRunner userState = null)
        {
            var flyout = this.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            if(flyout is RunnerInfoFlyout && userState != null)
            {
                ((RunnerInfoFlyout)flyout).AddContent((RunSummaryMessage)userState.GetState());
            }

            flyout.IsOpen = !flyout.IsOpen;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(0);
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(2);
        }

        private void btnClearLog(object sender, RoutedEventArgs e)
        {
            txtLog.Document.Blocks.Clear();
        }
      

      
    }
}
