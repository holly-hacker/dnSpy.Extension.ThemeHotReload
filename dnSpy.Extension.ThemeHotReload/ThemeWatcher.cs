using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using dnSpy.Contracts.App;
using dnSpy.Contracts.Extension;
using dnSpy.Contracts.Themes;

namespace HoLLy.dnSpyExtension.ThemeHotReload
{
    [ExportAutoLoaded(LoadType = AutoLoadedLoadType.AppLoaded)]
    public class ThemeWatcher : IAutoLoaded
    {
        private readonly IAppWindow _appWindow;
        private readonly IThemeService _themeService;
        private readonly FileSystemWatcher _watcher;

        [ImportingConstructor]
        public ThemeWatcher(IAppWindow appWindow, IThemeService themeService)
        {
            _appWindow = appWindow;
            _themeService = themeService;

            // TODO: account for multiple paths
            var path = AppDirectories.GetDirectories("Themes").First()!;
            _watcher = new FileSystemWatcher(path);
            _watcher.Changed += WatcherOnChanged;
            _watcher.EnableRaisingEvents = true;

            MsgBox.Instance.Show("Loaded!\n");
        }

        private void WatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            _appWindow.MainWindow.Dispatcher.Invoke(() =>
            {
                // TODO: debounce
                try
                {
                    var doc = XDocument.Load(e.FullPath);
                    var theme = ReflectionHelper.LoadTheme(doc.Root!);
                    MsgBox.Instance.Show($"{e.FullPath}\n{theme}");
                }
                catch (Exception exception)
                {
                    MsgBox.Instance.Show(exception);
                }
            });
        }
    }
}