using CapitalT;
using CapitalT.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Owin
{
    public static class OwinContextExtensions
    {
        public static CultureInfo GetUserCulture(this IOwinContext owinContext)
        {
            var culture = owinContext.Get<CultureInfo>(OwinKeys.UserCultureKey);
            return culture != null ? culture : Capital.DefaultCulture;
        }
    }
}
