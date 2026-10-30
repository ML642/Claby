namespace PackageManager;

public class Package : IComparable<Package> {
    public string Name { get; }
    public string Version { get; }
    public int DownloadKb { get; }
    public IReadOnlyList<string> DependsOn { get; }

    /* your code here */
    public Package(string name, string version, int downloadKb, params string[] dependsOn)
    {
        this.Name = name;
        this.Version = version;
        this.DownloadKb = downloadKb;
        this.DependsOn = dependsOn;
    }

    public int CompareTo(Package? other)
    {
        if (other == null)
            return -1;

        // bigger packages first
        int result = other.DownloadKb.CompareTo(this.DownloadKb);

        if (result != 0)
            return result;

        // same size -> sort by name
        return string.Compare(
            this.Name,
            other.Name,
            StringComparison.InvariantCultureIgnoreCase
        );
    }


    public override string ToString() => $"{Name} {Version} ({DownloadKb} KB)";
}