public class BackupEngine
{
    public string SourceDirectory { get; }
    public string RepositoryDirectory { get; }

    private readonly IReadOnlyList<string> ignorePatterns;

    public BackupEngine(
        string sourceDirectory,
        string repositoryDirectory,
        IReadOnlyList<string> ignorePatterns)
    {
        SourceDirectory = sourceDirectory;
        RepositoryDirectory = repositoryDirectory;
        this.ignorePatterns = ignorePatterns;
    }


    EventHandler<FileFoundEventArgs>? FileFound;
    EventHandler<FileProcessedEventArgs>? FileProcessed;
    EventHandler<BackupProgressEventArgs>? ProgressChanged;
    EventHandler<BackupCompletedEventArgs>? Completed;

    protected virtual void OnFileFound(FileFoundEventArgs e)
        {
            FileFound?.Invoke(this, e);
        }

        protected virtual void OnFileProcessed(FileProcessedEventArgs e)
        {
            FileProcessed?.Invoke(this, e);
        }

        protected virtual void OnProgressChanged(BackupProgressEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }

        protected virtual void OnCompleted(BackupCompletedEventArgs e)
        {
            Completed?.Invoke(this, e);
        }
    
    
}