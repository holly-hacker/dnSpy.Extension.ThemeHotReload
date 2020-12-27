using System;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using dnSpy.Contracts.App;
using dnSpy.Contracts.Extension;
using dnSpy.Contracts.Themes;
using HoLLy.dnSpyExtension.ThemeHotReload.FileWatchers;

namespace HoLLy.dnSpyExtension.ThemeHotReload
{
    [ExportAutoLoaded(LoadType = AutoLoadedLoadType.AppLoaded)]
    public class ThemeWatcher : IAutoLoaded, IDisposable
    {
        private readonly IAppWindow _appWindow;
        private readonly IThemeService _themeService;
        private readonly MultiplePathWatcher _watcher;

        [ImportingConstructor]
        public ThemeWatcher(IAppWindow appWindow, IThemeService themeService)
        {
            _appWindow = appWindow;
            _themeService = themeService;

            var paths = AppDirectories.GetDirectories("Themes");
            _watcher = new DebouncedMultiplePathWatcher(paths, OnFileChanged, TimeSpan.FromSeconds(0.5));
        }

        public void Dispose() => _watcher.Dispose();

        private void OnFileChanged(string path)
        {
            _appWindow.MainWindow.Dispatcher.Invoke(() =>
            {
                XDocument doc;
                try
                {
                    doc = XDocument.Load(path);
                }
                catch (Exception exception)
                {
                    MsgBox.Instance.Show(exception, "Reading updated theme failed");
                    return;
                }

                var theme = ReflectionHelper.LoadTheme(doc.Root!);
                ReflectionHelper.SetTheme(_themeService, theme);
            });
        }
    }
}