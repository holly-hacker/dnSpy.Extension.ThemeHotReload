using System.Collections.Generic;
using dnSpy.Contracts.Extension;

namespace HoLLy.dnSpyExtension.ThemeHotReload
{
    public class Extension : IExtension
    {
        public IEnumerable<string> MergedResourceDictionaries
        {
            get { yield break; }
        }

        public ExtensionInfo ExtensionInfo => new()
        {
            ShortDescription = "Theme Hot Reload",
        };

        public void OnEvent(ExtensionEvent e, object? obj)
        {
        }
    }
}