using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.Translate
{
    internal class IOHelpers
    {
        private const string AppDataFolder = "App_Data";
        private const string LocalizationFolder = "Localization";

        internal static string GetAppDataPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, AppDataFolder);
        }

        internal static string GetLocalizationFolderPath()
        {
            return Path.Combine(GetAppDataPath(), LocalizationFolder);
        }

        internal static DirectoryInfo GetOrCreateLocalizationRootDirectory()
        {
            var directory = new DirectoryInfo(GetLocalizationFolderPath());
            if (!directory.Exists)
            {
                directory.Create();
            }
            return directory;
        }
    }
}
