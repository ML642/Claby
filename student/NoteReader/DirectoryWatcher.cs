public class DirectoryWatcher : IDisposable // Stage2.1
{
    private string _path;

    private int countfiles;
    private int countDir;
    public List<string> files{get;private set;}
    public List<string> directories{get;private set;}

    public string[] Files => files.ToArray();
    public string[] Directories => directories.ToArray();
    public event EventHandler? DirectoryChanged;
    private readonly FileSystemWatcher watcher;
    public DirectoryWatcher(string path)
    {
        this._path = path;
        files = new List<string>();
        directories = new List<string>();

        foreach( string file in Directory.EnumerateFiles(path))
        {
            files.Add(Path.GetFileName(file));
        }

        foreach( string dur in Directory.EnumerateDirectories(path))
        {
            directories.Add(Path.GetFileName(dur));
        }

         watcher  = new FileSystemWatcher(path)
        {
          NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName,
          IncludeSubdirectories = false  
        };

        watcher.Created += OnChange;
        
        watcher.Deleted += OnChange;
        
        watcher.Renamed += OnChange;

        watcher.EnableRaisingEvents = true;

    }
    private void OnChange(object sender, FileSystemEventArgs e)
    {   
        if(e.Name == null)
        {
            return;
        }
        switch (e.ChangeType)
        {   

        case WatcherChangeTypes.Created:
                if (Directory.Exists(e.FullPath))
                    directories.Add(e.Name);
                else if (File.Exists(e.FullPath))
                    files.Add(e.Name);
                break;

        case WatcherChangeTypes.Deleted:
     
                if (Directory.Exists(e.FullPath))
                    directories.Remove(e.Name);
                else if (File.Exists(e.FullPath))
                    files.Remove(e.Name);
            break;

        case WatcherChangeTypes.Renamed:
            var renamed = (RenamedEventArgs)e;
            
            if (renamed.OldName is null || renamed.Name is null)
                break;

            if (files.Remove(renamed.OldName))
                files.Add(renamed.Name);
            else if (directories.Remove(renamed.OldName))
                directories.Add(renamed.Name);

            break;
        }
        DirectoryChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose() => watcher.Dispose();

}
