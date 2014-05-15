using Autofac;
using CapitalT.Autofac;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapitalT.Examples.Owin.SystemWeb
{
    public class CapitalTOwinModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
        }

        internal class OwinUserCultureAccessor : IUserCultureAccessor
        {
            private readonly IOwinContext _owinContext;

            public OwinUserCultureAccessor(IOwinContext owinContext = null)
            {
                _owinContext = owinContext;
            }

            public System.Globalization.CultureInfo Get()
            {
                if (_owinContext != null)
                {
                    return _owinContext.GetUserCulture();
                }
                else
                {
                    return Capital.DefaultCulture;
                }
            }
        }
    }
}