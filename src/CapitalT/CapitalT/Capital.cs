using CapitalT.Culture;
using CapitalT.Translate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT
{
    public static class Capital
    {
        private static CapitalConfig _capitalConfig = new CapitalConfig();
        public static CapitalConfig Config
        {
            get { return _capitalConfig; }
            set
            {
                if(value == null) {
                    throw new ArgumentNullException("value");
                }
                _capitalConfig = value;
            }
        }

        public static IEnumerable<CultureInfo> AppCultures
        {
            get { return Config.AppCulturesAccessor(); }
        }

        public static CultureInfo DefaultCulture
        {
            get { return Config.DefaultCulture; }
        }

        public static Localizer EmptyT { get { return NullLocalizer.Instance; } }

        public static Localizer TFor<Scope>(CultureInfo culture)
        {
            return TFor(typeof(Scope), culture);
        }

        public static Localizer TFor(Type scope, CultureInfo culture)
        {
            if (scope == null)
            {
                throw new ArgumentNullException("scope");
            }
            return TFor(scope.FullName, culture);
        }

        public static Localizer TFor(string scope, CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }
            return new Text(scope, culture, Config.LocalizedStringService).Get;
        }

        public static Localizer TFor<Scope>(UserCulture culture)
        {
            return TFor(typeof(Scope), culture);
        }

        public static Localizer TFor(Type scope, UserCulture culture)
        {
            if (scope == null)
            {
                throw new ArgumentNullException("scope");
            }
            return TFor(scope.FullName, culture);
        }

        public static Localizer TFor(string scope, UserCulture culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }
            return new Text(scope, culture, Config.LocalizedStringService).Get;
        }
    }
}
