using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Autofac;
using Collections;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace WpfClient.ViewModels
{
    public class ExploreModeViewModel : ViewModelBase
    {
        private readonly IRuntime _runtime;

        public ExploreModeViewModel(IRuntime runtime)
        {

            _runtime = runtime;
            CmdMouseDown = new RelayCommand<MouseEventArgs>(OnMouseDown);
            CmdClearCanvas = new RelayCommand(OnClearCanvas);
            CmdClearLog = new RelayCommand(OnClearLog);
            Types = ViewModelLocator.Types;

            //Console.SetOut(new ConsoleWriter(_logger));
            _runtime.Start();
        }

        private void OnMouseDown(MouseEventArgs e)
        {
            var element = e.OriginalSource as UIElement;

            if (!_runtime.IsRunning())
            {
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (element is Canvas)
                {
                    CreateRunner(element);
                }
            }
            else
            {
                // _runtime.Remove(e.EventInfo);
            }
        }

        public TypesViewModel Types
        {
            get; set;
        }


        public RelayCommand<MouseEventArgs> CmdMouseDown { get; private set; }
        public RelayCommand CmdClearLog { get; private set; }
        public RelayCommand CmdClearCanvas { get; private set; }

        private void OnClearCanvas()
        {
            _runtime.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            OnClearLog();
            
        }

        private void OnClearLog()
        {
           ViewModelLocator.LogViewer.LogEntries.Clear();
           
        }

        private void MouseOverHandler(object source, MouseOverEventArgs e)
        {
        }

        private void circle_OnKeyPressed(object source, KeyPressedEventArgs e)
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
        private void CreateRunner(UIElement element)
        {
            Point location = Mouse.GetPosition(element);

            LoadedType objectType = Types.SelectedType;
            if (objectType == null)
            {
                return;
            }

            var methods = new List<MethodInfo>();
            methods.Add(Types.SelectedMethod);


            IGui gui = null;
            DrawTypes drawType = Settings.DrawAs;
            if (drawType == DrawTypes.Circle)
            {
                var circle = new CustomCircle(((Canvas)element).Children, location);


                circle.OnMouseOver += MouseOverHandler;
                circle.OnKeyPressed += circle_OnKeyPressed;
                gui = circle;
            }
            else if (drawType == DrawTypes.Rectangle)
            {
                var rectangle = new CustomRectangle(((Canvas)element).Children, location);

                rectangle.OnMouseOver += MouseOverHandler;
                gui = rectangle;
            }

            IRunnable runnable;
            IRunner runner;
            try
            {
                runnable = new RunnableObject(objectType.TypeInfo, methods);
            }
            catch (Exception e)
            {
                //_logger.ErrorNow(e.Message);
                //var guiMessage = new ErrorMessage(e.Message, 100);

                //runner = ObjectFactory.CreateRunner(Settings.ThreadingType, null, gui, _logger, Settings.Loops);
                //gui.Draw();
                //_runtime.Add(runner);
                //gui.Update(guiMessage);
                return;
            }

            var settings = new RunnerSettings();
            settings.Iterations = Settings.Loops;
            settings.RunnerType = Settings.ThreadingType;

            runner = ViewModelLocator.CreateRunner(runnable, settings);
            runner.AddUiListener(gui);
            ContextMenu ctxMenu = ShapeContextMenu.Get(
                (s, e) => _runtime.Remove(runner.Id),
                (s, e) => MainWindow.ToggleFlyout(1, _runtime.GetById(runner.Id), true),
                (s, e) => MessageBox.Show("shows the code that executes in the code explorer")
                );

            ((CustomShape)gui).AddContextMenu(ctxMenu);

            _runtime.Add(runner);
            runner.Start();
        }

      
    }
}
