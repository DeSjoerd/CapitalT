using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.Translate
{
    public interface ITranslationProvider
    {
        IEnumerable<CultureInfo> GetAvailableCultures();

        Dictionary<string, string> GetTranslations(CultureInfo culture);
    }
}
