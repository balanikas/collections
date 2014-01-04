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

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        


        Canvas _canvas;
        Runtime _game;
        CancellationTokenSource _cts;
        ILogger _logger;

        public MainWindow()
        {
            InitializeComponent();
           
            
            _cts = new CancellationTokenSource();

            LoadGuiData();
            _logger = new TextboxLogger(txtLog);
            
            _canvas = new Canvas();
            _canvas.MouseDown += _canvas_MouseDown;
            
            gridGameArea.Children.Add(_canvas);


            _game = new Runtime();
            _canvas.Background = this.Resources["CanvasGameStarted"] as LinearGradientBrush;
            Task task = Task.Factory.StartNew(() =>
            {
                _game.Start();
            }, _cts.Token);

        }

     

        private void CreateGuiObject(Point p)
        {
            LoadedType objectType = ucTypes.SelectedType;
            if(objectType == null)
            {
                return;
            }
            //var actions = lstActions.SelectedItems.Cast<string>().ToList();
            var actions = ucTypes.SelectedMethods;
           

            IGui gui = null;
            var drawType = Settings.DrawAs;
            if(drawType == DrawTypes.Circle)
            {
                var circle = new CustomCircle(_canvas.Children, p);
               // circle.OnRightClick += new RightClickEventHandler(RightClickHandler);
                circle.OnMouseOver += new MouseOverEventHandler(MouseOverHandler);
                gui = circle;
            }
            else if (drawType == DrawTypes.Rectangle)
            {
                var rectangle = new CustomRectangle(_canvas.Children, p);
               // rectangle.OnRightClick += new RightClickEventHandler(RightClickHandler);
                rectangle.OnMouseOver += new MouseOverEventHandler(MouseOverHandler);
                gui = rectangle;
            }
            
            IBehavior behavior;
            IRunner runner;
            try
            {
                behavior = new ObjectBehavior(objectType.TypeInfo, actions);
            }
            catch (Exception e)
            {
                var guiMessage = new ErrorMessage(e.Message, 100);
                
                runner = ObjectFactory.Get(Settings.ThreadingType, null, gui, _logger, Settings.Loops);
                gui.Draw();
                _game.Add(runner);
                gui.Update(guiMessage);
                return;
            }
            


            runner = ObjectFactory.Get(Settings.ThreadingType,behavior, gui,_logger,Settings.Loops);

            runner.Start();
            
            _game.Add(runner);
            
        }

        private void RightClickHandler(object source, RightClickEventArgs e)
        {
            _game.Remove(e.EventInfo);
        }

        private void MouseOverHandler(object source, MouseOverEventArgs e)
        {
            
        }


        private void btnStop_Click(object sender, RoutedEventArgs e)
        {

            
            _game.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }

        void _canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(!_game.IsRunning())
            {
                return;
            }
            Point p = Mouse.GetPosition(_canvas);
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                CreateGuiObject(p);
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

       

        private void LoadGuiData()
        {
            var collectionTypes = CollectionsSOLID.SupportedTypes.Get();
            
            lstCollectionTypes.ItemsSource = collectionTypes;

            
        }

      

        private void lstCollectionTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var x = (KeyValuePair<Type, List<string>>)lstCollectionTypes.SelectedItem;

            lstActions.ItemsSource = x.Value;
        }


        private void ToggleFlyout(int index)
        {
            var flyout = this.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }

            flyout.IsOpen = !flyout.IsOpen;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(0);
        }
      

      
    }
}
