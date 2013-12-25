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

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        


        Canvas _canvas;
        Game _game;
        CancellationTokenSource _cts;
        public MainWindow()
        {
            InitializeComponent();

            _cts = new CancellationTokenSource();

            LoadGuiData();

            _game = new Game();
            _canvas = new Canvas();
            _canvas.Background = this.Resources["CanvasGameStopped"] as LinearGradientBrush;
            _canvas.MouseDown += _canvas_MouseDown;
            
            gridGameArea.Children.Add(_canvas);
        }

     

        private void CreateGuiObject(Point p)
        {
            Type objectType = ucTypes.SelectedType;
            //var actions = lstActions.SelectedItems.Cast<string>().ToList();
            var actions = ucTypes.SelectedMethods;
            var collection = (KeyValuePair<Type, List<string>>)lstCollectionTypes.SelectedItem;

            IGui gui = null;
            var drawType = Settings.DrawAs;
            if(drawType == DrawTypes.Circle)
            {
                var circle = new CustomCircle(_canvas.Children, p);
                circle.OnRightClick += new RightClickEventHandler(RightClickHandler);
                circle.OnMouseOver += new MouseOverEventHandler(MouseOverHandler);
                gui = circle;
            }
            else if (drawType == DrawTypes.Rectangle)
            {
                var rectangle = new CustomRectangle(_canvas.Children, p);
                rectangle.OnRightClick += new RightClickEventHandler(RightClickHandler);
                rectangle.OnMouseOver += new MouseOverEventHandler(MouseOverHandler);
                gui = rectangle;
            }
            
            //IBehavior behavior = new CollectionBehavior(objectType, collection.Key, actions);
            IBehavior behavior = new ObjectBehavior(objectType,  actions);
            IRunner runner = ObjectFactory.Get(Settings.ThreadingType,behavior, gui,Settings.Loops);

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

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            _canvas.Background = this.Resources["CanvasGameStarted"] as LinearGradientBrush;
            Task task = Task.Factory.StartNew(() =>
            {
                _game.Start();
            }, _cts.Token);

            //task.ContinueWith((t)=>
            //{
                
            //    if(t.Status == TaskStatus.Canceled)
            //    {
            //        _game.Stop();
                   
            //    }
            //},_cancellation.Token);
            
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {

            _game.Stop();
            //_cancellation.Cancel();
            System.Diagnostics.Trace.WriteLine("cancelling");

            //gridGameArea.Children.Clear();
            _canvas.Background = _canvas.Background = this.Resources["CanvasGameStopped"] as LinearGradientBrush;
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
