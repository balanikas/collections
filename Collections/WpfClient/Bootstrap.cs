using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfClient
{

    using Autofac;

    public class Bootstrap
    {
        public IContainer Configure()
        {
            var builder = new ContainerBuilder();



            return builder.Build();
        }
    }

}
