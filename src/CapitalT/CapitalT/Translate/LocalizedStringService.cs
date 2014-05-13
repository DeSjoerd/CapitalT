using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.Translate
{
    public class LocalizedStringService : ILocalizedStringService
    {
        private readonly ITranslationProvider _translationProvider;

        public LocalizedStringService(ITranslationProvider translationProvider)
        {
            if (translationProvider == null)
            {
                throw new ArgumentNullException("translationProvider");
            }

            this._translationProvider = translationProvider;
        }

        public string GetLocalizedString(string scope, string text, CultureInfo culture)
        {
            var localizedStrings = _translationProvider.GetTranslations(culture);
            if (localizedStrings == null)
            {
                return GetParentTranslation(scope, text, culture);
            }

            string scopedKey = (scope + "|" + text).ToLowerInvariant();
            if (localizedStrings.ContainsKey(scopedKey))
            {
                return localizedStrings[scopedKey];
            }

            string genericKey = ("|" + text).ToLowerInvariant();
            if (localizedStrings.ContainsKey(genericKey))
            {
                return localizedStrings[genericKey];
            }

            return GetParentTranslation(scope, text, culture);
        }

        private string GetParentTranslation(string scope, string text, CultureInfo culture)
        {
            try
            {
                CultureInfo parentCultureInfo = culture.Parent;
                if (parentCultureInfo.IsNeutralCulture)
                {
                    var localizedStrings = _translationProvider.GetTranslations(parentCultureInfo);
                    if (localizedStrings == null)
                    {
                        return text;
                    }

                    string scopedKey = (scope + "|" + text).ToLowerInvariant();
                    if (localizedStrings.ContainsKey(scopedKey))
                    {
                        return localizedStrings[scopedKey];
                    }

                    string genericKey = ("|" + text).ToLowerInvariant();
                    if (localizedStrings.ContainsKey(genericKey))
                    {
                        return localizedStrings[genericKey];
                    }
                }
            }
            catch (CultureNotFoundException) { }

            // not found
            return text;
        }
    }
}
