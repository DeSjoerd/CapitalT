using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CapitalT.Translate;
using System.Globalization;
using Moq;
using System.Collections.Generic;

namespace CapitalT.Tests
{
    [TestClass]
    public class LocalizedStringServiceTests
    {
        private Mock<ITranslationProvider> translationProviderMock;
        private LocalizedStringService localizedStringService;


        private string scope = typeof(LocalizedStringServiceTests).FullName;
        private CultureInfo nlNL_Culture = CultureInfo.GetCultureInfo("nl-NL");
        private CultureInfo nl_Culture = CultureInfo.GetCultureInfo("nl");

        [TestInitialize]
        public void Init()
        {
            translationProviderMock = new Mock<ITranslationProvider>();
            localizedStringService = new LocalizedStringService(translationProviderMock.Object);
        }

        [TestMethod]
        public void When_ScopedKey_Is_Found_Return_TranslatedText()
        {
            translationProviderMock.Setup(t => t.GetTranslations(It.Is<CultureInfo>(c => c.Equals(nlNL_Culture))))
                .Returns(() => new Dictionary<string, string>
                {
                    { ScopedKey(scope, "textHint"), "textHint:nl-NL" }
                });

            var result = localizedStringService.GetLocalizedString(scope, "textHint", nlNL_Culture);

            var expected = "textHint:nl-NL";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void When_ScopedKey_In_Specific_Culture_Isnt_Found_Try_GenericKey()
        {
            translationProviderMock.Setup(t => t.GetTranslations(It.Is<CultureInfo>(c => c.Equals(nlNL_Culture))))
                .Returns(() => new Dictionary<string, string>
                {
                    { GenericKey("textHint"), "textHint:Generic:nl-NL" }
                });

            var result = localizedStringService.GetLocalizedString(scope, "textHint", nlNL_Culture);

            var expected = "textHint:Generic:nl-NL";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void When_Translation_With_GenericKey_Isnt_Found_Try_Parent_Culture()
        {
            translationProviderMock.Setup(t => t.GetTranslations(It.Is<CultureInfo>(c => c.Equals(nl_Culture))))
                .Returns(() => new Dictionary<string, string>
                {
                    { ScopedKey(scope, "textHint"), "textHint:nl" }
                });

            var result = localizedStringService.GetLocalizedString(scope, "textHint", CultureInfo.GetCultureInfo("nl-NL"));

            var expected = "textHint:nl";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void When_Translation_In_Parent_Culture_With_ScopedKey_Isnt_Found_Try_GenericKey()
        {
            translationProviderMock.Setup(t => t.GetTranslations(It.Is<CultureInfo>(c => c.Equals(nl_Culture))))
                .Returns(() => new Dictionary<string, string>
                {
                    { GenericKey("textHint"), "textHint:nl" }
                });

            var result = localizedStringService.GetLocalizedString(scope, "textHint", CultureInfo.GetCultureInfo("nl-NL"));

            var expected = "textHint:nl";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void When_Translation_In_Parent_Culture_With_GenericKey_Inst_Found_Return_ProvidedText()
        {
            var result = localizedStringService.GetLocalizedString(scope, "textHint", CultureInfo.GetCultureInfo("nl-NL"));
            var expected = "textHint";
        }

        private static string ScopedKey(string scope, string text)
        {
            return (scope + "|" + text).ToLowerInvariant();
        }

        private static string GenericKey(string text)
        {
            return ("|" + text.ToLowerInvariant());
        }
    }
}
