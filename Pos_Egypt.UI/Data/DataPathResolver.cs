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
        var dataRoot = string.IsNullOrWhiteSpace(configuredValue)
            ? Path.Combine(localApplicationData, "HeCashier")
            : configuredValue.Trim();

        string fullRoot;
        try
        {
            fullRoot = Path.GetFullPath(dataRoot);
        }
        catch (Exception exception)
        {
            return new DataPathResolution(dataRoot, Path.Combine(dataRoot, "hecashier.db"),
                $"مسار البيانات غير صالح: {exception.Message}");
        }

        var driveRoot = Path.GetPathRoot(fullRoot);
        if (string.IsNullOrEmpty(driveRoot) || !(driveExists ?? Directory.Exists)(driveRoot))
        {
            return new DataPathResolution(fullRoot, Path.Combine(fullRoot, "hecashier.db"),
                $"محرك البيانات المحدد غير متاح: {driveRoot ?? fullRoot}");
        }

        return new DataPathResolution(fullRoot, Path.Combine(fullRoot, "hecashier.db"), null);
    }
}
