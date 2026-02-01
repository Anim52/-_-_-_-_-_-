using System;
using System.IO;
using System.Text.Json;

namespace MaxiMed.Wpf.Services;

public sealed class UiOptions
{
    public bool IsDarkTheme { get; set; } = false;
    public bool AnimationsEnabled { get; set; } = true;

    // Personalization
    public string LastPage { get; set; } = "Patients";
    public string Accent { get; set; } = "Blue";            // Blue|Green|Purple
    public string Density { get; set; } = "Normal";         // Normal|Compact|Comfort
    public double BaseFontSize { get; set; } = 13;          // 11..16
    public string CornerRadius { get; set; } = "Rounded";   // Sharp|Rounded|ExtraRounded

    // Behaviour
    public bool ConfirmDestructiveActions { get; set; } = true;
    public bool RememberLastPage { get; set; } = true;
    public string StartPage { get; set; } = "Patients";     // Patients|Appointments|Reports|...

    // Effects
    public string AnimationSpeed { get; set; } = "Normal";  // Fast|Normal|Slow

    public static string GetPath()
    {
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MaxiMed");
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, "uioptions.json");
    }

    public static UiOptions Load()
    {
        try
        {
            var path = GetPath();
            if (!File.Exists(path)) return new UiOptions();
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<UiOptions>(json) ?? new UiOptions();
        }
        catch
        {
            return new UiOptions();
        }
    }

    public void Save()
    {
        var path = GetPath();
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
}
