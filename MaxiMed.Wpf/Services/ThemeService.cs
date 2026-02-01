using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MaxiMed.Wpf.Services
{
    public static class ThemeService
    {
        public static void SetTheme(bool dark)
        {
            var app = System.Windows.Application.Current;

            var existing = app.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null &&
                    (d.Source.OriginalString.EndsWith("ThemeLight.xaml") ||
                     d.Source.OriginalString.EndsWith("ThemeDark.xaml")));

            if (existing != null)
                app.Resources.MergedDictionaries.Remove(existing);

            var uri = dark
                ? "/Resources/Theme/ThemeDark.xaml"
                : "/Resources/Theme/ThemeLight.xaml";

            app.Resources.MergedDictionaries.Insert(1, new ResourceDictionary
            {
                Source = new Uri(uri, UriKind.Relative)
            });
        }

        // accentFileName: "ThemeAccentBlue.xaml"
        public static void SetAccent(string accentFileName)
        {
            var app = System.Windows.Application.Current;

            var existing = app.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("ThemeAccent"));

            if (existing != null)
                app.Resources.MergedDictionaries.Remove(existing);

            var uri = "/Resources/Theme/" + accentFileName;

            app.Resources.MergedDictionaries.Insert(0, new ResourceDictionary
            {
                Source = new Uri(uri, UriKind.Relative)
            });
        }
        public static class UiOptions
        {
            public static bool IsDarkTheme { get; set; }
            public static bool AnimationsEnabled { get; set; } = true;
        }
    }
}
