using System.IO;

namespace HeCashier.Data;

public sealed record DataPathResolution(string DataRoot, string DatabasePath, string? Error);

public static class DataPathResolver
{
    public static DataPathResolution Resolve(
        string? environmentRoot,
        string? configuredRoot,
        string localApplicationData,
        Func<string, bool>? driveExists = null)
    {
        var configuredValue = !string.IsNullOrWhiteSpace(environmentRoot)
            ? environmentRoot
            : configuredRoot;
        var fallbackRoot = Path.Combine(localApplicationData, "HeCashier");
        var configuredRootPath = string.IsNullOrWhiteSpace(configuredValue)
            ? fallbackRoot
            : configuredValue.Trim();

        if (TryGetAvailableRoot(configuredRootPath, driveExists, out var fullRoot))
            return new DataPathResolution(fullRoot, Path.Combine(fullRoot, "hecashier.db"), null);

        if (TryGetAvailableRoot(fallbackRoot, driveExists, out fullRoot))
            return new DataPathResolution(fullRoot, Path.Combine(fullRoot, "hecashier.db"), null);

        return new DataPathResolution(fallbackRoot, Path.Combine(fallbackRoot, "hecashier.db"),
            $"تعذر الوصول إلى مسار البيانات المحدد أو المسار المحلي: {fallbackRoot}");
    }

    private static bool TryGetAvailableRoot(
        string root,
        Func<string, bool>? driveExists,
        out string fullRoot)
    {
        try
        {
            fullRoot = Path.GetFullPath(root);
            var driveRoot = Path.GetPathRoot(fullRoot);
            return !string.IsNullOrEmpty(driveRoot) && (driveExists ?? Directory.Exists)(driveRoot);
        }
        catch
        {
            fullRoot = root;
            return false;
        }
    }
}
