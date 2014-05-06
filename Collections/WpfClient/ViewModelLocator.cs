using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Collections;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using WpfClient.ViewModels;

namespace WpfClient
{
    public class ViewModelLocator
    {
        private static readonly IContainer _container;
        static ViewModelLocator()
        {
            var cb = new ContainerBuilder();

            cb.RegisterType<Logger>().As<ILogger>().InstancePerLifetimeScope();
            cb.RegisterType<Runtime>().As<IRuntime>().InstancePerLifetimeScope();
            cb.RegisterType<TypesViewModel>().InstancePerLifetimeScope();
            cb.RegisterType<ExploreModeViewModel>().InstancePerLifetimeScope();
            cb.RegisterType<MainWindowViewModel>().InstancePerLifetimeScope();
            cb.RegisterType<LogViewerViewModel>().InstancePerLifetimeScope();
            

            _container = cb.Build();

            var logger = _container.Resolve<ILogger>();
            _container.Resolve<ExploreModeViewModel>();

            logger.Subscribe(_container.Resolve<LogViewerViewModel>());
        }

        public static TypesViewModel Types
        {
            get
            {
                return _container.Resolve<TypesViewModel>();
            }
        }

        public static ExploreModeViewModel ExploreMode
        {
            get
            {
                return _container.Resolve<ExploreModeViewModel>();
               
            }
        }

        public static ILogger Logger
        {
            get { return _container.Resolve<ILogger>(); }
        }

        public static MainWindowViewModel MainWindow
        {
            get { return _container.Resolve<MainWindowViewModel>(); }
        }

        public static LogViewerViewModel LogViewer
        {
            get
            {
                return _container.Resolve<LogViewerViewModel>(); 
            }
        }

        public static IRunner CreateRunner(IRunnable runnable, RunnerSettings settings)
        {
            switch (settings.RunnerType)
            {
                case RunnerType.BackgroundWorkerBased:
                    return new BWBasedRunner(runnable,  _container.Resolve<ILogger>(), settings);
                case RunnerType.ParallelTaskBased:
                    return new TplBasedRunner(runnable,  _container.Resolve<ILogger>(), settings);
                default:
                    throw new NotImplementedException();
            }
        }
    }

 
}
