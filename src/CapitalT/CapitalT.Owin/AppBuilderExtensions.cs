using CapitalT.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin
{
    public static class AppBuilderExtensions
    {
        public static void UseCapitalT(this IAppBuilder app, CapitalTOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            app.Use<CapitalTMiddleware>(options);
        }
    }
}
