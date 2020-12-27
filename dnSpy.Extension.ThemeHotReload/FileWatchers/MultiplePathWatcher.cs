using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HoLLy.dnSpyExtension.ThemeHotReload.FileWatchers
{
    public class MultiplePathWatcher : IDisposable
    {
        protected readonly Action<string> OnChange;
        private readonly FileSystemWatcher[] _watchers;
        
        public MultiplePathWatcher(IEnumerable<string> paths, Action<string> onChange)
        {
            OnChange = onChange;
            _watchers = paths.Where(Directory.Exists).Select(p => new FileSystemWatcher(p)).ToArray();
            foreach (var watcher in _watchers)
            {
                watcher.Changed += WatcherOnChanged;
                watcher.EnableRaisingEvents = true;
            }
        }

        private void WatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            FileChanged(e.FullPath);
        }

        protected virtual void FileChanged(string path)
        {
            OnChange(path);
        }

        public void Dispose()
        {
            foreach (var watcher in _watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Changed -= WatcherOnChanged;
            }
        }
    }
}