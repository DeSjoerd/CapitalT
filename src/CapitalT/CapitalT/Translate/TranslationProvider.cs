using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.Translate
{
    public class TranslationProvider : ITranslationProvider
    {
        private const string AppCulturesCacheKey = "CapitalT.AppCultures";
        private const string TranslationProviderCacheName = "CapitalT.TranslationProvider.Cache";

        private MemoryCache _memCache;
        private DirectoryInfo _rootDirectory;

        public TranslationProvider(DirectoryInfo rootDirectory)
        {
            _memCache = new MemoryCache(TranslationProviderCacheName);
            _rootDirectory = rootDirectory;
        }

        public IEnumerable<CultureInfo> GetAvailableCultures()
        {
            var availableCultures = _memCache.Get(AppCulturesCacheKey) as IEnumerable<CultureInfo>;
            if (availableCultures == null)
            {
                var directory = _rootDirectory;
                var localeDirectories = directory.GetDirectories()
                    .Where(dir => dir.EnumerateFiles("*.po").Any())
                    .ToList();

                var cultures = new List<CultureInfo>();
                foreach (var localeDir in localeDirectories)
                {
                    var localeName = localeDir.Name;
                    try
                    {
                        var culture = CultureInfo.GetCultureInfo(localeName);
                        cultures.Add(culture);
                    }
                    catch (CultureNotFoundException) { }
                }
                availableCultures = cultures.AsEnumerable();
                
                _memCache.Add(
                    new CacheItem(AppCulturesCacheKey, availableCultures), 
                    CreateCacheItemPolicyWithFileMonitor(new List<FileSystemInfo>{ directory }.Union(localeDirectories))
                );
            }
            return availableCultures;
        }

        public Dictionary<string, string> GetTranslations(CultureInfo culture)
        {
            var cacheKey = GetLocalizedStringsCacheKey(culture);
            var translations = _memCache.Get(cacheKey) as Dictionary<string, string>;
            if (translations == null)
            {
                var translationsDirectory = GetTranslationsDirectory(culture);
                if (!translationsDirectory.Exists || !translationsDirectory.EnumerateFiles("*.po").Any())
                {
                    translations = new Dictionary<string, string>();
                    _memCache.Add(
                        new CacheItem(cacheKey, translations),
                        CreateCacheItemPolicyWithFileMonitor(new[] { _rootDirectory })
                    );
                }
                else
                {
                    var localizedStrings = new Dictionary<string, string>();
                    var translationFiles = translationsDirectory.EnumerateFiles("*.po").ToList();
                    foreach (var translationFile in translationFiles)
                    {
                        try
                        {
                            using (var stream = translationFile.OpenRead())
                            using (var reader = new StreamReader(stream))
                            {
                                ParseLocalizationStream(reader, localizedStrings, true);
                            }
                        }
                        catch (IOException) { }
                    }

                    translations = localizedStrings;
                    _memCache.Add(
                        new CacheItem(cacheKey, translations),
                        CreateCacheItemPolicyWithFileMonitor(new FileSystemInfo[] {_rootDirectory}.Union(translationFiles))
                    );
                }
            }
            return translations;
        }

        private DirectoryInfo GetTranslationsDirectory(CultureInfo culture)
        {
            var cultureName = culture.Name;
            return new DirectoryInfo(Path.Combine(_rootDirectory.FullName, cultureName));
        }

        private static readonly Dictionary<char, char> _escapeTranslations = new Dictionary<char, char> {
            { 'n', '\n' },
            { 'r', '\r' },
            { 't', '\t' }
        };

        private static string Unescape(string str)
        {
            StringBuilder sb = null;
            bool escaped = false;
            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (escaped)
                {
                    if (sb == null)
                    {
                        sb = new StringBuilder(str.Length);
                        if (i > 1)
                        {
                            sb.Append(str.Substring(0, i - 1));
                        }
                    }
                    char unescaped;
                    if (_escapeTranslations.TryGetValue(c, out unescaped))
                    {
                        sb.Append(unescaped);
                    }
                    else
                    {
                        // General rule: \x ==> x
                        sb.Append(c);
                    }
                    escaped = false;
                }
                else
                {
                    if (c == '\\')
                    {
                        escaped = true;
                    }
                    else if (sb != null)
                    {
                        sb.Append(c);
                    }
                }
            }
            return sb == null ? str : sb.ToString();
        }

        private static void ParseLocalizationStream(TextReader reader, IDictionary<string, string> translations, bool merge)
        {
            string poLine, id, scope;
            id = scope = String.Empty;
            while ((poLine = reader.ReadLine()) != null)
            {
                if (poLine.StartsWith("#:"))
                {
                    scope = ParseScope(poLine);
                    continue;
                }

                if (poLine.StartsWith("msgctxt"))
                {
                    scope = ParseContext(poLine);
                    continue;
                }

                if (poLine.StartsWith("msgid"))
                {
                    id = ParseId(poLine);
                    continue;
                }

                if (poLine.StartsWith("msgstr"))
                {
                    string translation = ParseTranslation(poLine);
                    // ignore incomplete localizations (empty msgid or msgstr)
                    if (!String.IsNullOrWhiteSpace(id) && !String.IsNullOrWhiteSpace(translation))
                    {
                        string scopedKey = (scope + "|" + id).ToLowerInvariant();
                        if (!translations.ContainsKey(scopedKey))
                        {
                            translations.Add(scopedKey, translation);
                        }
                        else
                        {
                            if (merge)
                            {
                                translations[scopedKey] = translation;
                            }
                        }
                    }
                    id = scope = String.Empty;
                }

            }
        }

        private static string ParseTranslation(string poLine)
        {
            return Unescape(poLine.Substring(6).Trim().Trim('"'));
        }

        private static string ParseId(string poLine)
        {
            return Unescape(poLine.Substring(5).Trim().Trim('"'));
        }

        private static string ParseScope(string poLine)
        {
            return Unescape(poLine.Substring(2).Trim().Trim('"'));
        }

        private static string ParseContext(string poLine)
        {
            return Unescape(poLine.Substring(7).Trim().Trim('"'));
        }

        private static string GetLocalizedStringsCacheKey(CultureInfo culture)
        {
            return "po:" + culture.Name;
        }

        private static CacheItemPolicy CreateCacheItemPolicyWithFileMonitor(IEnumerable<FileSystemInfo> files)
        {
            var policy = new CacheItemPolicy();
            policy.ChangeMonitors.Add(new HostFileChangeMonitor(files.Select(f => f.FullName).ToList()));
            return policy;
        }
    }
}
