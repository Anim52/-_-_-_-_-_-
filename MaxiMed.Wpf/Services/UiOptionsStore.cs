using System;
using System.IO;
using System.Text.Json;

namespace MaxiMed.Wpf.Services;

public static class UiOptionsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private static string GetPath()
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
            return JsonSerializer.Deserialize<UiOptions>(json, JsonOptions) ?? new UiOptions();
        }
        catch
        {
            return new UiOptions();
        }
    }

    public static void Save(UiOptions options)
    {
        try
        {
            var path = GetPath();
            var json = JsonSerializer.Serialize(options, JsonOptions);
            File.WriteAllText(path, json);
        }
        catch
        {
            // ignore
        }
    }
}
