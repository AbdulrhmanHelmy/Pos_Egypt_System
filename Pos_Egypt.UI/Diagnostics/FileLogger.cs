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
        }
        catch (Exception loggingException)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to write log file '{path}': {loggingException}");
        }

        return path;
    }
}
