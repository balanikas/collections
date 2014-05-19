﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Collections;
using Collections.Messages;
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
            InfoView = new InfoView();

            //Console.SetOut(new ConsoleWriter(_logger));
            _runtime.Reset();
        }

        public TypesViewModel Types { get; set; }

        public InfoView InfoView { get; set; }

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


       

        private void CreateRunner(Canvas element)
        {
            LoadedType type = Types.SelectedType;
            if (type == null)
            {
                return;
            }

            var shape = UIHelper.CreateDrawingShape(element);

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
                Iterations = Settings.Instance.Get(Settings.Keys.ExploreModeIterationCount),
                RunnerType =  Settings.Instance.Get(Settings.Keys.ThreadingType)
            };


            var runner = _runtime.CreateAndAddRunner(runnable, settings);
            
            ContextMenu ctxMenu = ShapeContextMenu.Create(
                (s, e) => _runtime.Runners.Remove(runner),
                (s, e) =>  MainWindow.ToggleFlyout(1, _runtime.Runners.GetById(runner.Id), true),
                (s, e) => { }
                );

            shape.AddContextMenu(ctxMenu);
            shape.OnLeftClick += (source, args) =>
            {
                InfoView.Register(_runtime.Runners.GetById(args.EventInfo));
            };
            runner.AddUiListener(shape);
            InfoView.Register(runner);

            runner.Start();
        }

     
    }

    public class InfoView :  ViewModelBase, IGui
    {
        private MethodExecutionMessage _message;
        private IRunner _runner;
        public string Id { get; set; }

        public InfoView()
        {
            Message = new MethodExecutionMessage();
        }
        public void Initialize()
        {
           
        }

     

        public void Register(IRunner runner)
        {
            if (_runner != null)
            {
                _runner.RemoveUiListener(this);
            }
          
           
            _runner = runner;
            
            Message.Summary = _runner.GetCurrentState();
            _runner.AddUiListener(this);
        }

        public void Update(MethodExecutionMessage message)
        {
            Message = message;
            
        }


        public void Destroy()
        {
            Message = null;
        }

        public MethodExecutionMessage Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }


    }
}