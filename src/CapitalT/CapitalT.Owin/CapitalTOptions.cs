using CapitalT.Owin.Culture;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.Owin
{
    public class CapitalTOptions
    {
        public CapitalTOptions()
            : this(Enumerable.Empty<ICultureSelector>())
        { }

        public CapitalTOptions(IEnumerable<ICultureSelector> cultureSelectors)
        {
            this.CultureSelectors = new List<ICultureSelector>(cultureSelectors);
        }

        public List<ICultureSelector> CultureSelectors { get; private set; }
    }
}
