using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.Owin
{
    public class CapitalTMiddleware : OwinMiddleware
    {
        private readonly CapitalTOptions _options;

        public CapitalTMiddleware(OwinMiddleware next, CapitalTOptions options)
            : base(next)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            this._options = options;
        }

        public override async Task Invoke(IOwinContext context)
        {
            var culture = SelectCulture(context);
            context.Set(OwinKeys.UserCultureKey, culture);

            await Next.Invoke(context);
        }

        private CultureInfo SelectCulture(IOwinContext context)
        {
            var results = _options.CultureSelectors.SelectMany(c => c.GetUserCultures(context))
               .OrderByDescending(result => result.Quality)
               .ThenByDescending(result => result.Priority);

            var appCultures = Capital.AppCultures;

            foreach (var result in results)
            {
                if (appCultures.Contains(result.Culture))
                {
                    return result.Culture;
                }
            }
            return Capital.DefaultCulture;
        }

    }
}
