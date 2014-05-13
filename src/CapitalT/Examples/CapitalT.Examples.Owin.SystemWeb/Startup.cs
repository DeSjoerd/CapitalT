using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Diagnostics;

[assembly: OwinStartup(typeof(CapitalT.Examples.Owin.SystemWeb.Startup))]

namespace CapitalT.Examples.Owin.SystemWeb
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            var dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory") as string;

            Debugger.Break();
        }
    }
}
