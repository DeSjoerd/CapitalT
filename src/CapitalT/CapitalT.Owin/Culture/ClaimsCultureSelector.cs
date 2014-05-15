using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.Owin.Culture
{
    public class ClaimsCultureSelector : ICultureSelector
    {
        public ClaimsCultureSelector()
        {
            Priority = 2;
        }

        public int Priority { get; set; }

        public IEnumerable<CultureSelectorResult> GetUserCultures(IOwinContext owinContext)
        {
            var user = owinContext.Authentication.User;
            if (user != null)
            {
                var localeClaim = user.Claims.Where(c => c.Type == ClaimTypes.Locality)
                    .FirstOrDefault();

                if (localeClaim != null)
                {
                    CultureInfo culture = null;
                    try
                    {
                        culture = CultureInfo.GetCultureInfo(localeClaim.Value);

                    }
                    catch (CultureNotFoundException) { }

                    if (culture != null)
                    {
                        return new[] {
                            new CultureSelectorResult {
                                Culture = culture,
                                Quality = 1.0f,
                                Priority = Priority
                            }
                        };
                    }
                }
            }
            return Enumerable.Empty<CultureSelectorResult>();
        }
    }
}
