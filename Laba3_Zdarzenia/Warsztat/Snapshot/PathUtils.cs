public static class PathUtils
{
    public static bool IsInside(string path, string directory)
    {
        string FullPath = Path.GetFullPath(path);
        string FullPath2 = Path.GetFullPath(directory);
        
        var relative = Path.GetRelativePath(FullPath2,FullPath);

        if(relative == "..")
        {
            return false;
        }
        if (relative.StartsWith($"..{Path.DirectorySeparatorChar}",StringComparison.Ordinal))
        {
            return false;
        }
        if(Path.IsPathRooted( relative)){return false;}
        
        return true;
    }
}