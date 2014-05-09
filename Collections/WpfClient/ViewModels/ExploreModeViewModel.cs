using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Collections;
using Collections.Runtime;
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
            _runtime.Reset();
        }

        public TypesViewModel Types { get; set; }

        public RelayCommand<MouseEventArgs> CmdMouseDown { get; private set; }
        public RelayCommand CmdClearLog { get; private set; }
        public RelayCommand CmdClearCanvas { get; private set; }

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
                    CreateRunner(element as Canvas);
                }
            }
        }

        private void OnClearCanvas()
        {
            _runtime.Runners.RemoveAll();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            OnClearLog();
        }

        private void OnClearLog()
        {
            ViewModelLocator.LogViewer.LogEntries.Clear();
            
        }


        private CustomShape CreateDrawingShape(Canvas canvas, IRuntime runtime)
        {
            var location = Mouse.GetPosition(canvas);
            CustomShape shape = null;
            if (Settings.DrawAs == DrawTypes.Circle)
            {
                shape = new CustomCircle(canvas.Children, location);
            }
            else if (Settings.DrawAs == DrawTypes.Rectangle)
            {
                shape = new CustomRectangle(canvas.Children, location);
            }

            shape.OnMouseOver += (source, args) =>
            {

            };
            shape.OnKeyPressed += (source, args) =>
            {
                if (args.Key == Key.D1)
                {
                    runtime.Runners.RemoveById(args.EventInfo);
                }
                else if (args.Key == Key.D2)
                {
                    MainWindow.ToggleFlyout(1, runtime.Runners.GetById(args.EventInfo));
                }
            };

            return shape;
        }

        private void CreateRunner(Canvas element)
        {
            LoadedType type = Types.SelectedType;
            if (type == null)
            {
                return;
            }

            var shape = CreateDrawingShape(element, _runtime);

            IRunnable runnable;
            try
            {
                var methods = new List<MethodInfo> { Types.SelectedMethod };
                runnable = new RunnableItem(type.TypeInfo, methods);
            }
            catch (Exception e)
            {
                _runtime.Logger.ErrorNow(e.Message);
                return;
            }

            var settings = new RunnerSettings
            {
                Iterations = Settings.Loops, 
                RunnerType = Settings.ThreadingType
            };


            var runner = _runtime.CreateAndAddRunner(runnable, settings);
            
            ContextMenu ctxMenu = ShapeContextMenu.Create(
                (s, e) => _runtime.Runners.Remove(runner),
                (s, e) => MainWindow.ToggleFlyout(1, _runtime.Runners.GetById(runner.Id), true),
                (s, e) => { }
                );

            shape.AddContextMenu(ctxMenu);
           
            runner.AddUiListener(shape);
 
            runner.Start();
        }
    }
}