using CapitalT.Translate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT
{
    public class CapitalConfig
    {
        private CultureInfo _defaultCulture;
        private Func<IEnumerable<CultureInfo>> _appCulturesAccessor;
        private Func<ITranslationProvider> _translationProviderFactory;
        private ITranslationProvider _translationProvider;

        private Func<ILocalizedStringService> _localizedStringServiceFactory;
        private ILocalizedStringService _localizedStringService;

        public CapitalConfig()
        {
            DefaultCulture = CultureInfo.GetCultureInfo("en");

            AppCulturesAccessor = () => TranslationProvider.GetAvailableCultures();
            TranslationProviderFactory = () => new TranslationProvider(IOHelpers.GetOrCreateLocalizationRootDirectory());
            LocalizedStringServiceFactory = () => new LocalizedStringService(TranslationProvider);
        }

        /// <summary>
        /// Gets or sets the default culture. (defaults to "en")
        /// </summary>
        public CultureInfo DefaultCulture
        {
            get { return _defaultCulture; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _defaultCulture = value;
            }
        }

        /// <summary>
        /// Gets or sets the appCulturesAccessor. (defaults to TranslationProvider.GetAvailableCultures())
        /// </summary>
        public Func<IEnumerable<CultureInfo>> AppCulturesAccessor
        {
            get { return _appCulturesAccessor; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _appCulturesAccessor = value;
            }
        }

        /// <summary>
        /// Gets or sets the TranslationProviderFactory. Used to create a single cached instance.
        /// Clears the cached instances of TranslationProvider and LocalizedStringService.
        /// </summary>
        public Func<ITranslationProvider> TranslationProviderFactory
        {
            get { return _translationProviderFactory; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _translationProviderFactory = value;
                _translationProvider = null;
                _localizedStringService = null;
            }
        }

        /// <summary>
        /// Gets or sets the LocalizedStringServiceFactory. Used to create a single cached instance.
        /// Clears the cached instances of TranslationProvider and LocalizedStringService.
        /// </summary>
        public Func<ILocalizedStringService> LocalizedStringServiceFactory
        {
            get { return _localizedStringServiceFactory; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _localizedStringServiceFactory = value;
                _translationProvider = null;
                _localizedStringService = null;
            }
        }

        /// <summary>
        /// Gets the cached Translation provider.
        /// Used by the default implementation of LocalizedStringService.
        /// </summary>
        public ITranslationProvider TranslationProvider
        {
            get
            {
                var translationProvider = this._translationProvider;
                if (translationProvider == null)
                {
                    translationProvider = this._translationProvider = TranslationProviderFactory();
                }
                return translationProvider;
            }
        }

        /// <summary>
        /// Gets the cached LocalizedStringService.
        /// Default uses the TranslationProvider.
        /// </summary>
        public ILocalizedStringService LocalizedStringService
        {
            get
            {
                var localizedStringService = this._localizedStringService;
                if (localizedStringService == null)
                {
                    localizedStringService = this._localizedStringService = LocalizedStringServiceFactory();
                }
                return localizedStringService;
            }
        }
    }
}
