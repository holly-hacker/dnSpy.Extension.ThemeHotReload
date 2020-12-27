using System;
using System.Collections.Generic;

namespace HoLLy.dnSpyExtension.ThemeHotReload.FileWatchers
{
    public class DebouncedMultiplePathWatcher : MultiplePathWatcher
    {
        private readonly TimeSpan _timeSpan;
        private readonly Dictionary<string, Debouncer> _debouncers = new();

        public DebouncedMultiplePathWatcher(IEnumerable<string> paths, Action<string> onChange, TimeSpan timeSpan) : base(paths, onChange)
        {
            _timeSpan = timeSpan;
        }

        protected override void FileChanged(string path)
        {
            if (_debouncers.TryGetValue(path, out var foundDebouncer))
            {
                foundDebouncer.Trigger(() => OnChange(path));
            }
            else
            {
                var debouncer = new Debouncer(_timeSpan);
                debouncer.Trigger(() => OnChange(path));
                _debouncers[path] = debouncer;
            }
        }
    }
}