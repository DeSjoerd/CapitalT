using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using CapitalT.Translate;
using System.IO;
using System.Globalization;

namespace CapitalT.Tests
{
    [TestClass]
    public class TranslationProviderTests
    {
        [TestMethod]
        public void App_Data_Directory_Is_Based_On_ApplicationBase()
        {
            var expected = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "App_Data");
            var actual = IOHelpers.GetAppDataPath();
        }

        [TestMethod]
        public void GetAvailableCultures()
        {
            // gets the available cultures, located in Bin/Debug
            var translationProvider = new TranslationProvider(IOHelpers.GetOrCreateLocalizationRootDirectory());

            for (var i = 0; i < 100000; i++)
            {
                var cultures = translationProvider.GetAvailableCultures();
            }
        }

        [TestMethod]
        public void GetTranslations_en()
        {
            var translationProvider = new TranslationProvider(IOHelpers.GetOrCreateLocalizationRootDirectory());
            var translations = translationProvider.GetTranslations(CultureInfo.GetCultureInfo("en"));
        }
    }
}
