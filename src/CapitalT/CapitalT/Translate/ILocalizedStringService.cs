using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.Translate
{
    public interface ILocalizedStringService
    {
        string GetLocalizedString(string scope, string text, CultureInfo culture);
    }
}
