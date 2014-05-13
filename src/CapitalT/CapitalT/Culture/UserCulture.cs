using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.Culture
{
    public class UserCulture
    {
        private static CultureInfo _defaultCulture = CultureInfo.GetCultureInfo("en");
        public static CultureInfo DefaultCulture
        {
            get { return _defaultCulture; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _defaultCulture = DefaultCulture;
            }
        }

        private readonly IEnumerable<ICultureSelector> _cultureSelectors;
        private Lazy<CultureInfo> _culture;

        public UserCulture(IEnumerable<ICultureSelector> cultureSelectors)
        {
            this._cultureSelectors = cultureSelectors;
            this._culture = new Lazy<CultureInfo>(Get);
        }

        public CultureInfo Culture
        {
            get { return _culture.Value; }
        }

        private CultureInfo Get()
        {
            var results = this._cultureSelectors.SelectMany(c => c.GetUserCultures())
                .OrderByDescending(result => result.Quality)
                .ThenByDescending(result => result.Priority);

            var appCultures = Capital.AppCultures;

            foreach(var result in results) {
                if (appCultures.Contains(result.Culture))
                {
                    return result.Culture;
                }
            }
            return Capital.DefaultCulture;
        }
    }
}
