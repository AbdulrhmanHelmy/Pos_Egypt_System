using System.Globalization;
using System.IO;

namespace HeCashier.Diagnostics;

public sealed class FileLogger(Func<string> dataRootProvider)
{
    private readonly object _sync = new();

    public string GetCurrentLogFilePath()
    {
        var root = dataRootProvider();
        return Path.Combine(root, "Logs", $"hecashier-{DateTime.Now:yyyyMMdd}.log");
    }

    public string Log(Exception exception) => Log(exception.ToString());

    public string Log(string message)
    {
        var path = GetCurrentLogFilePath();
        try
        {
            lock (_sync)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.AppendAllText(path,
                    $"[{DateTime.Now.ToString("O", CultureInfo.InvariantCulture)}] {message}{Environment.NewLine}");
            }
            return path;
        }
        catch (Exception loggingException)
        {
            var fallbackPath = Path.Combine(Path.GetTempPath(), "HeCashier", "Logs", Path.GetFileName(path));
            try
            {
                lock (_sync)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fallbackPath)!);
                    File.AppendAllText(fallbackPath,
                        $"[{DateTime.Now.ToString("O", CultureInfo.InvariantCulture)}] {message}{Environment.NewLine}Failed to write '{path}': {loggingException}{Environment.NewLine}");
                }
                return fallbackPath;
            }
            catch (Exception fallbackException)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to write log files '{path}' and '{fallbackPath}': {fallbackException}");
                return path;
            }
        }
    }
}
