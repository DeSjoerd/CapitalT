using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Diagnostics;
using Autofac;
using System.Globalization;
using CapitalT.Autofac;
using CapitalT.Owin;
using CapitalT.Owin.Culture;

[assembly: OwinStartup(typeof(CapitalT.Examples.Owin.SystemWeb.Startup))]

namespace CapitalT.Examples.Owin.SystemWeb
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            var dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            app.UseAutofacMiddleware(BuildContainer());
            
            app.UseCapitalT(new CapitalTOptions(new ICultureSelector[] { new HttpHeaderCultureSelector() }));

            Debugger.Break();
        }

        private ILifetimeScope BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<CapitalTModule>();
            builder.RegisterModule<CapitalTOwinModule>();

            return builder.Build();
        }

        private class OwinUserCultureAccessor : IUserCultureAccessor
        {
            private IOwinContext _context;
            public OwinUserCultureAccessor(IOwinContext context)
            {
                this._context = context;
            }

            public CultureInfo Get()
            {
                return _context.GetUserCulture();
            }
        }

    }
}
