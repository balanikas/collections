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
        private bool _isActivated;

        public ExploreModeViewModel(IRuntime runtime)
        {
            _runtime = runtime;
            CmdMouseDown = new RelayCommand<MouseEventArgs>(OnMouseDown);
            CmdClearCanvas = new RelayCommand(OnClearCanvas);
            CmdClearLog = new RelayCommand(OnClearLog);
            Types = ViewModelLocator.Types;
            InfoView = new MethodExecutionView();

            _runtime.Reset();
        }

        public TypesViewModel Types { get; set; }

        public MethodExecutionView InfoView { get; set; }

        public RelayCommand<MouseEventArgs> CmdMouseDown { get; private set; }
        public RelayCommand CmdClearLog { get; private set; }
        public RelayCommand CmdClearCanvas { get; private set; }

        public bool IsActivated
        {
            get
            {
                return _isActivated;
            }
            set
            {
                if (value)
                {
                    OnActivated();
                }
                else
                {
                    OnDeactivated();
                }
                _isActivated = value;
            }
        }

        private void OnActivated()
        {
            _runtime.Runners.RemoveAll();
            OnClearLog();
            
        }

        private void OnDeactivated()
        {
            _runtime.Runners.RemoveAll();
            OnClearLog();

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
                if (element is Canvas && Types.SelectedMethod != null)
                {
                    var shape = UIHelper.CreateDrawingShape((Canvas)element);
                    CreateRunner((Canvas)element, shape);
                }
            }
        }

        private void OnClearCanvas()
        {
            _runtime.Runners.RemoveAll();
            OnClearLog();
            InfoView.IsExpanded = false;
        }

        private void OnClearLog()
        {
            ViewModelLocator.LogViewer.LogEntries.Clear();
            
        }

        public void CreateRunner(Canvas element, CustomShape shape)
        {
            LoadedType type = Types.SelectedType;
            if (type == null)
            {
                return;
            }

            

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

            var settings = new RunnerSettings(
                Settings.Instance.Get(Settings.Keys.ExploreModeIterationCount),
                Settings.Instance.Get(Settings.Keys.ThreadingType),
                Settings.Instance.Get(Settings.Keys.CompilerServiceType)
                );


            var runner = _runtime.CreateAndAddRunner(runnable, settings);
            
            ContextMenu ctxMenu = ShapeContextMenu.Create(
                (s, e) => _runtime.Runners.Remove(runner),
                (s, e) =>
                {

                    InfoView.IsExpanded = true;
                    InfoView.Register(runner);
                },
                (s, e) => {  }
                );

            shape.AddContextMenu(ctxMenu);
            shape.OnLeftClick += (source, args) =>
            {
                InfoView.Register(_runtime.Runners.GetById(args.EventInfo));
                InfoView.IsExpanded = true;
            };
            runner.AddUiListener(shape);
            InfoView.Register(runner);

            runner.Start();
        }

     
    }
}