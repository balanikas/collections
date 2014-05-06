using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collections;

namespace WpfClient
{

    using Autofac;

    public class Bootstrap
    {
        public static IContainer Container { get; private set; }
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            //builder.RegisterType<TextboxLogger>().As<ILogger>();
          
            builder.RegisterType<Runtime>().As<IRuntime>();
            builder.RegisterType<TypesProvider>();

            Container = builder.Build();
            return Container;
        }
    }

}
