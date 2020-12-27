using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using dnSpy.Contracts.Themes;

namespace HoLLy.dnSpyExtension.ThemeHotReload
{
    public static class ReflectionHelper
    {
        private const string ThemeNamespace = "dnSpy.Themes";
        private const string ThemeClass = "Theme";
        private static readonly Assembly DnSpyAssembly;

        static ReflectionHelper()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            DnSpyAssembly = assemblies.Single(x => x.GetName().Name == "dnSpy");
        }

        public static ITheme LoadTheme(XElement element)
        {
            var type = DnSpyAssembly.GetTypes().Single(x => x.Namespace == ThemeNamespace && x.Name == ThemeClass);
            var ctor = type.GetConstructor(new[] {typeof(XElement)}) ?? throw new Exception("Couldn't find Theme ctor");
            var theme = ctor.Invoke(new object[] {element})!;
            return (ITheme) theme;
        }

        public static void SetTheme(IThemeService themeService, ITheme theme)
        {
            var prop = themeService.GetType().GetProperty("Theme");

            if (prop is not null)
                prop.SetValue(themeService, theme);
            else
                throw new Exception("Couldn't find Theme property on theme service");
        }
    }
}