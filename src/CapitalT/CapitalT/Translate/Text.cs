using CapitalT.Culture;
using CapitalT.Translate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.Translate
{
    /// <summary>
    /// Instance per dependency
    /// </summary>
    internal class Text
    {
        private readonly string _scope;
        private readonly Lazy<CultureInfo> _culture;
        private readonly ILocalizedStringService _localizedStringService;

        internal Text(string scope, CultureInfo culture, ILocalizedStringService localizedStringService)
        {
            this._scope = scope;
            this._culture = new Lazy<CultureInfo>(() => culture);
            this._localizedStringService = localizedStringService;
        }

        internal Text(string scope, UserCulture culture, ILocalizedStringService localizedStringService)
        {
            this._scope = scope;
            this._culture = new Lazy<CultureInfo>(() => culture.Culture);
            this._localizedStringService = localizedStringService;
        }

        internal LocalizedString Get(string textHint, params object[] args)
        {
            var localizedFormat = _localizedStringService.GetLocalizedString(_scope, textHint, _culture.Value);

            return args.Length == 0
                ? new LocalizedString(localizedFormat, _scope, textHint, args)
                : new LocalizedString(string.Format(_culture.Value, localizedFormat, args), _scope, textHint, args);
        }
    }
}
