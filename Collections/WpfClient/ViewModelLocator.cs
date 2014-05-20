using Autofac;
using Collections;
using Collections.Compiler;
using Collections.Logging;
using Collections.Runtime;
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

            cb.RegisterType<RoslynCompiler>().As<ICompiler>();
            cb.RegisterType<DefaultCompiler>().As<ICompiler>();
            cb.RegisterType<TypesProvider>();

            cb.RegisterType<TypesViewModel>().InstancePerLifetimeScope();
            cb.RegisterType<ExploreModeViewModel>().InstancePerLifetimeScope();
            cb.RegisterType<PlayModeViewModel>().InstancePerLifetimeScope();
            cb.RegisterType<MainWindowViewModel>().InstancePerLifetimeScope();
            cb.RegisterType<LogViewerViewModel>().InstancePerLifetimeScope();


            _container = cb.Build();


            var logger = _container.Resolve<ILogger>();
            logger.Subscribe(_container.Resolve<LogViewerViewModel>());
        }

        public static TypesViewModel Types
        {
            get
            {
                _container.Resolve<TypesViewModel>();
                return _container.Resolve<TypesViewModel>();
            }
        }

        public static ExploreModeViewModel ExploreMode
        {
            get { return _container.Resolve<ExploreModeViewModel>(); }
        }

        public static PlayModeViewModel PlayModeMode
        {
            get { return _container.Resolve<PlayModeViewModel>(); }
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
            get { return _container.Resolve<LogViewerViewModel>(); }
        }

       
    }
}