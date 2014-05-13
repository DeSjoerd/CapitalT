using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.Examples.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            Debugger.Break();
        }
    }
}
