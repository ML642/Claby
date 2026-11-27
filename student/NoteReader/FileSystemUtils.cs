using System;
using System.IO;
using System.IO.Compression;

public static class FileSystemUtils
{
    public static StreamWriter? Log { get; private set; }
    public static string PrepareDirectory(string path) // Stage1.1
    {
        if (Directory.Exists(path))
        {
            return path;
        }

        if (File.Exists(path) &&
            string.Equals(Path.GetExtension(path), ".zip", StringComparison.OrdinalIgnoreCase))
        {
            string directory = Path.Combine(
                Path.GetTempPath(), Path.GetFileNameWithoutExtension(path));

            Directory.CreateDirectory(directory);
            ZipFile.ExtractToDirectory(path, directory);
            return directory;
        }

        throw new FileSystemException(
            $"Path must refer to an existing directory or ZIP archive: {path}");
    }
    public static int CountFiles(string path, string suffix = "") // Stage1.2
    {
        int count = 0;

        foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
        {
            if (Path.GetFileName(file).EndsWith(suffix, StringComparison.Ordinal))
                count++;
        }

        return count;
    }
    public static void WatchDirectory(string path) // Stage2.2
    {

        using var watcher = new DirectoryWatcher(path);

        watcher.DirectoryChanged += (_, _) =>
        {
            foreach (string directory in watcher.Directories)
                Console.WriteLine($"d:{directory}");

            foreach (string file in watcher.Files)
                Console.WriteLine($"f:{file}");
        };    
        
        void OnExit(object? sender, EventArgs e)
        {
            Log?.WriteLine($"{DateTime.Now}: Application exit.");
            Log?.Dispose();
            Log = null;
        }    

        Log = new StreamWriter(Path.Combine(path, $"{DateTime.Now.ToString("yyyy_MM_dd hh_mm_ss")}.log"));
        AppDomain.CurrentDomain.ProcessExit  += OnExit;
        
        try
        {
            Console.ReadLine();
        }
        finally
        {
            AppDomain.CurrentDomain.ProcessExit -= OnExit;
            Log?.Dispose();
            Log = null;
        }
    }
    public class FileSystemException(string? message) : Exception(message) { }
}
