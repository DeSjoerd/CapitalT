using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.Owin.Culture
{
    public class HttpHeaderCultureSelector : ICultureSelector
    {
        public HttpHeaderCultureSelector()
        {
            Priority = 1;
        }

        public int Priority { get; set; }

        public IEnumerable<CultureSelectorResult> GetUserCultures(IOwinContext owinContext)
        {
            var acceptLanguage = owinContext.Request.Headers.Get("Accept-Language");
            if (acceptLanguage == null)
            {
                yield break;
            }
            var userLanguages = acceptLanguage.Split(',').Select(userLanguage => userLanguage.Trim());
            foreach (var userLanguage in userLanguages)
            {
                var languageQuality = userLanguage.Split(';');
                var language = languageQuality.Length >= 1 ? languageQuality[0] : null;
                if (language != null)
                {
                    CultureInfo culture = null;
                    var quality = 0.0f;
                    try
                    {
                        culture = CultureInfo.GetCultureInfo(language);

                        if (languageQuality.Length >= 2)
                        {
                            var qualityString = languageQuality[1].Split('=').LastOrDefault();
                            float parsedQuality;
                            if (float.TryParse(qualityString, NumberStyles.Float, CultureInfo.InvariantCulture, out parsedQuality))
                            {
                                quality = parsedQuality;
                            }
                        }
                        else
                        {
                            quality = 1.0f;
                        }
                    }
                    catch (CultureNotFoundException) { }
                    if (culture != null)
                    {
                        yield return new CultureSelectorResult
                        {
                            Culture = culture,
                            Priority = Priority,
                            Quality = quality
                        };
                    }
                }
            }
        }
    }
}
