using System;
using System.Linq;
using System.Windows;

namespace MaxiMed.Wpf.Services
{
    public static class ThemeService
    {
        public static UiOptions Options { get; private set; } = new UiOptions();

        public static void Initialize()
        {
            Options = UiOptions.Load();
            ApplyFromOptions(Options);
        }

        // универсальный pack uri без хардкода имени сборки
        private static Uri Pack(string relativePath)
        {
            var asm = typeof(ThemeService).Assembly.GetName().Name; // "MaxiMed.Wpf"
            return new Uri($"pack://application:,,,/{asm};component/{relativePath}", UriKind.Absolute);
        }

        public static void ApplyFromOptions(UiOptions opt)
        {
            SetTheme(opt.IsDarkTheme);
            SetAccent(opt.Accent);

            System.Windows.Application.Current.Resources["BaseFontSize"] = opt.BaseFontSize;

            var cr = opt.CornerRadius switch
            {
                "Sharp" => 8d,
                "Rounded" => 12d,
                "ExtraRounded" => 16d,
                _ => 12d
            };
            System.Windows.Application.Current.Resources["CornerRadiusBase"] = new CornerRadius(cr);

            var density = opt.Density ?? "Normal";
            System.Windows.Application.Current.Resources["UiDensity"] = density;
            System.Windows.Application.Current.Resources["UiControlHeight"] = density switch
            {
                "Compact" => 34d,
                "Comfort" => 44d,
                _ => 38d
            };
            System.Windows.Application.Current.Resources["UiControlPadding"] = density switch
            {
                "Compact" => new Thickness(10, 4, 10, 4),
                "Comfort" => new Thickness(14, 8, 14, 8),
                _ => new Thickness(12, 6, 12, 6)
            };

            System.Windows.Application.Current.Resources["UiAnimSpeed"] = opt.AnimationSpeed ?? "Normal";
            System.Windows.Application.Current.Resources["UiAnimMs"] = (opt.AnimationSpeed ?? "Normal") switch
            {
                "Fast" => 120d,
                "Slow" => 240d,
                _ => 160d
            };
        }

        public static void SetTheme(bool isDark)
        {
            var uri = isDark
                ? Pack("Resources/Theme/Theme.Dark.xaml")
                : Pack("Resources/Theme/Theme.Light.xaml");

            ReplaceMergedDictionary(
                match: s => s.Contains("Resources/Theme/Theme.Dark.xaml", StringComparison.OrdinalIgnoreCase)
                         || s.Contains("Resources/Theme/Theme.Light.xaml", StringComparison.OrdinalIgnoreCase),
                newUri: uri);

            System.Diagnostics.Debug.WriteLine("SET THEME: " + (isDark ? "DARK" : "LIGHT"));
        }

        public static void SetAccent(string accentName)
        {
            var file = accentName switch
            {
                "Green" => "ThemeAccentGreen.xaml",
                "Purple" => "ThemeAccentPurple.xaml",
                _ => "ThemeAccentBlue.xaml"
            };

            var uri = Pack("Resources/Theme/" + file);

            ReplaceMergedDictionary(
                match: s => s.IndexOf("Resources/Theme/ThemeAccent", StringComparison.OrdinalIgnoreCase) >= 0,
                newUri: uri);
        }

        private static void ReplaceMergedDictionary(Func<string, bool> match, Uri newUri)
        {
            var app =   System.Windows. Application.Current;
            if (app is null) return;

            var md = app.Resources.MergedDictionaries;

            for (int i = md.Count - 1; i >= 0; i--)
            {
                var s = md[i].Source?.OriginalString ?? "";
                if (match(s))
                    md.RemoveAt(i);
            }

            // ВАЖНО: добавляем В КОНЕЦ, чтобы новый словарь имел максимальный приоритет
            md.Add(new ResourceDictionary { Source = newUri });
        }
    }
}
