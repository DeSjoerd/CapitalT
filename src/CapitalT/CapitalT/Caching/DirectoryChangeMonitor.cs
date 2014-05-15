using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CapitalT.Caching
{
    internal class DirectoryChangeMonitor : ChangeMonitor
    {
        private readonly object _onChangedLock = new object();

        private readonly string _uniqueId;
        private readonly string _directoryPath;
        private readonly FileSystemWatcher _watcher;

        public DirectoryChangeMonitor(string directoryPath)
            : this(directoryPath, string.Empty)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="filter">for example *.txt watches all files which end with .txt</param>
        internal DirectoryChangeMonitor(string directoryPath, string filter, bool includeSubdirectories = false)
        {
            _uniqueId = Guid.NewGuid().ToString();

            _watcher = new FileSystemWatcher(directoryPath, filter);
            _watcher.Changed += FilesChanged;
            _watcher.Created += FilesChanged;
            _watcher.Deleted += FilesChanged;
            _watcher.Renamed += FilesChanged;
            _watcher.Error += OnError;

            _watcher.IncludeSubdirectories = includeSubdirectories;
            _watcher.EnableRaisingEvents = true;

            this.InitializationComplete();
        }

        private void FilesChanged(object sender, FileSystemEventArgs args)
        {
            OnChanged(null);
        }

        private void OnError(object sender, ErrorEventArgs args)
        {
            OnChanged(null);
            Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            _watcher.Dispose();
        }

        public override string UniqueId
        {
            get { return _uniqueId; }
        }
    }
}
