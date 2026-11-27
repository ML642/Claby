using System.IO.Enumeration;
using System.Net;
public class DirectoryScanner
{
    string _directory;
    IReadOnlyList<string> _ignore;
    public string RootDirectory {get=>_directory;set=>_directory = value;}
    public DirectoryScanner(string rootDirectory, IReadOnlyList<string> ignorePatterns)
    {
        _directory = Path.GetFullPath(rootDirectory);
        _ignore = ignorePatterns;
    }

    public string ToRelativePath(string FullPath)
    {
        string s = Path.GetRelativePath(_directory,FullPath);

        return s.Replace(Path.DirectorySeparatorChar,'/');

    }

    public bool IsIgnored(string relativePath)
    {
        string[] a =  relativePath.Split('/');

        foreach (var i in a)
        {
            foreach (var pattern in _ignore)
            {
                if(FileSystemName.MatchesSimpleExpression(pattern, i))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public IEnumerable<FileEntry> Scan()
    {
        var options = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true  
        };
        foreach( FileInfo file in new DirectoryInfo(RootDirectory).EnumerateFiles("*", options))
        {
            string rel = ToRelativePath(file.FullName);

            if(IsIgnored(rel))continue;

            yield return new FileEntry(rel,file.FullName,file.Length,file.LastWriteTime);
        }

    }
}