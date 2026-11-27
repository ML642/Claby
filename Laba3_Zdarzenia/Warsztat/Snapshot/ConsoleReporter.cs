public class ConsoleReporter : IDisposable
{   
    private readonly BackupEngine engine;


    public ConsoleReporter(BackupEngine engine)
    {
        this.engine = engine;
        
        engine.FileProcessed += HandleFileProcessed;
        engine.ProgressChanged += HandleProgressChanged;
        engine.Completed += HandleCompleted;
    }

    public void Dispose()
    {
        engine.FileProcessed -= HandleFileProcessed;
        engine.ProgressChanged -= HandleProgressChanged;
        engine.Completed -= HandleCompleted;
    }


    private void HandleFileProcessed(object? sender, FileProcessedEventArgs e)
        {
            if (e.Kind == ChangeKind.Unchanged)
                {
                    return;
                }

                Console.WriteLine($"{e.Kind.ToSymbol()} {e.File.RelativePath}");
        }

    private void HandleProgressChanged(object? sender, BackupProgressEventArgs e)
    {
        int barWidth = 30;

        int filled = (int)(e.Fraction * barWidth);

        string bar = new string('#', filled)
                + new string('.', barWidth - filled);

        string message =
            $"[{bar}] {e.Fraction:P0} " +
            $"({e.ProcessedFiles}/{e.TotalFiles} files, " +
            $"{e.ProcessedBytes.ToHumanSize()})";

        Console.Write("\r" + message.PadRight(previousLineLength));

        previousLineLength = message.Length;
    }

    private void HandleCompleted(object? sender, BackupCompletedEventArgs e)
        {
                Console.WriteLine();

                foreach (FileChange change in e.Summary.Changes)
                {
                    if (change.Kind == ChangeKind.Deleted)
                    {
                        Console.WriteLine($"{change.Kind.ToSymbol()} {change.Path}");
                    }
                }

                Console.WriteLine(e.Summary);
            }
       
}