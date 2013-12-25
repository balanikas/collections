using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.CSharp;
using Timer = System.Timers.Timer;

namespace Collections
{
   
    public partial class MainWindow : Window
    {
        private GameObject _missile;
        private List<GameObject> _gameObjects; 
        private Timer m_AppProfilingTimer;
        private Timer _gameLoopTimer;

        public MainWindow()
        {

            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            InitializeComponent();
            m_AppProfilingTimer = new Timer(500);
            m_AppProfilingTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            m_AppProfilingTimer.Enabled = true;

            _gameLoopTimer = new Timer(100);
            _gameLoopTimer.Elapsed += new ElapsedEventHandler(OnGameUpdate);
            _gameLoopTimer.Enabled = true;

            _gameObjects = new List<GameObject>();
        }

        private void OnGameUpdate(object source, ElapsedEventArgs e)
        {
            Dispatcher.Invoke((Action)delegate()
            {
                if (_missile == null)
                {
                    return;
                }

                _missile.Update();
                foreach (var obj in _gameObjects)
                {

                   obj.Update();
                }

                if (!_missile.IsAlive())
                {
                    _missile.Destroy();
                    _missile = null;
                }

                //colission detection
                var objectsToDestroy = new List<GameObject>();
                
                foreach (var obj in _gameObjects)
                {

                    Rect missileRect = _missile.GetGraphics().GetBounds();
                    Rect enemyRect = obj.GetGraphics().GetBounds();
                    if (enemyRect.IntersectsWith(missileRect))
                    {
                        objectsToDestroy.Add(obj);
                    }
                }

                foreach (var gameObject in objectsToDestroy)
                {
                    gameObject.Destroy();
                    _gameObjects.Remove(gameObject);
                }


            });

        }

        
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Dispatcher.Invoke((Action) delegate()
            {
                var computerInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();

                Process proc = Process.GetCurrentProcess();
                lblMemory.Content = computerInfo.AvailablePhysicalMemory / 1048576 + " / " +
                 computerInfo.TotalPhysicalMemory / 1048576;
                


            });
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
          
        }

     

        private void btnCompile_Click(object sender, RoutedEventArgs e)
        {
       
        }

        private static Dictionary<Type, List<string>> GenerateCollectionTypes()
        {
            var collectionTypes = new Dictionary<Type, List<string>>();

            List<string> actionTypes;
            actionTypes = new List<string>();
            actionTypes.Add("Add");
            actionTypes.Add("Clear");
            actionTypes.Add("ToString");
            collectionTypes.Add(typeof(HashSet<>), actionTypes);

            actionTypes = new List<string>();
            actionTypes.Add("Add");
            actionTypes.Add("Clear");
            actionTypes.Add("ToString");
            collectionTypes.Add(typeof(List<>),actionTypes);

            actionTypes = new List<string>();
            actionTypes.Add("push");
            actionTypes.Add("Clear");
            actionTypes.Add("ToString");
            collectionTypes.Add(typeof(Stack<>), actionTypes);
            return collectionTypes;
        }

        private static List<string> GenerateCollectionMethods()
        {
            var actionTypes = new List<string>();
            actionTypes.Add("Add");
            actionTypes.Add("Clear");
            actionTypes.Add("ToString");

            return actionTypes;
        }

        private void lstCollectionTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var x = (KeyValuePair<Type, List<string>>)lstCollectionTypes.SelectedItem;

            lstActions.ItemsSource = x.Value;
        }

        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
           
            var collectionTypes = GenerateCollectionTypes();

            lstCollectionTypes.ItemsSource = collectionTypes;
            //lstCollectionTypes.SelectedValuePath = "@Value";
           
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            var tt = LoadDynamicTypes();
            lstObjects.ItemsSource = tt;
        }

        private Dictionary<object, string> LoadDynamicTypes()
        {
            var data = new Dictionary<object, string>();

            var compiler = new CSharpCodeProvider();

            var parms = new System.CodeDom.Compiler.CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true
            };
            parms.ReferencedAssemblies.Add("System.dll");
            //parms.ReferencedAssemblies.Add("System.Collections.Generic.dll");


            var files = new Dictionary<string, string>();
            foreach (string file in Directory.EnumerateFiles(@"C:\dev\collections\Collections\Collections\dynamic", "*.cs"))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var fileContent = File.ReadAllText(file);
                files.Add(fileName, File.ReadAllText(file));

                var results = compiler.CompileAssemblyFromSource(parms, fileContent);

                if (results.Errors.Count == 0)
                {
                    data.Add(results.CompiledAssembly.CreateInstance(fileName), fileContent);
                }

                
            }
           

            return data;
        }

        private void lstObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var x = (KeyValuePair<object,string>)lstObjects.SelectedItem;

            txtCode.Text = x.Value;
        }

        private void Slider_KeyDown_1(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var slider = sender as Slider;
            if (e.Key == Key.Space)
            {
                double x = cnvGameArea.ActualWidth*(slider.Value/100);
                Fire(x, 10);
            }
        }

        private void Fire(double x, double y)
        {

            if (_missile != null)
            {
                return;
            }

            var gui = new GuiRect();
            gui.SetHeight(20);
            gui.SetWidth(20);
            gui.SetPosition(x, y);
            var selection = (KeyValuePair<Type, List<string>>)lstCollectionTypes.SelectedItem;

            if ((Type)selection.Key == typeof(List<>))
            {
                gui.SetColor(Colors.Blue);
            }
            else
            {
                gui.SetColor(Colors.GreenYellow);
            }

            gui.AddParent(cnvGameArea.Children);

            Type ammoType = ((KeyValuePair<object, string>)lstObjects.SelectedItem).Key.GetType();

            List<string> actions = lstActions.SelectedItems.Cast<string>().ToList();

            var item = new MissileBehavior(ammoType, selection.Key, actions);

            _missile = new Missile(item, gui);
            _missile.Start();
        }

        private void btnCreateEnemy_Click(object sender, RoutedEventArgs e)
        {
            CreateEnemy(200, 200);
            CreateEnemy(400, 400);

            CreateEnemy(300, 300);
            CreateEnemy(600, 100);

            CreateEnemy(100, 400);
        }

        private void CreateEnemy(double x, double y)
        {
            GuiRect gui = new GuiRect();
            Enemy enemy = new Enemy();
            

            gui.SetColor(Colors.Goldenrod);
            gui.AddParent(cnvGameArea.Children);

            enemy.AddGraphics(gui);
            enemy.Start();
            enemy.Move(x, y);

            _gameObjects.Add(enemy);


            
        }

        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
