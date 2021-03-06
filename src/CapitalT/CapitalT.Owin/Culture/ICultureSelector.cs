﻿using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.Owin.Culture
{
    public class CultureSelectorResult
    {
        public int Priority { get; set; }
        public float Quality { get; set; }
        public CultureInfo Culture { get; set; }
    }

    public interface ICultureSelector
    {
        IEnumerable<CultureSelectorResult> GetUserCultures(IOwinContext owinContext);
    }
}
