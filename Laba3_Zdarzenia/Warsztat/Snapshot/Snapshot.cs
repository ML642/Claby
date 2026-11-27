using Microsoft.VisualBasic;

public record ManifestEntry(string Path, long Size, string Hash);

public class Manifest
{
    private readonly SortedDictionary<string, ManifestEntry> dict = new(StringComparer.Ordinal);

    public IReadOnlyCollection<ManifestEntry> Entries => dict.Values;

    public int Count => dict.Count;

    public long TotalSize{get;private set;}
    public void Add(ManifestEntry entry)
    {
        dict.Add(entry.Path, entry);
        TotalSize += entry.Size;
    }
    
    public bool Contains(string path)
    {
        return dict.ContainsKey(path);
    }

    public ChangeKind Classify(ManifestEntry current)
    {
        if (dict.ContainsKey(current.Path))
        {
            if(current.Hash == dict[current.Path].Hash){
                return ChangeKind.Unchanged;
            }
            else
            {
                return ChangeKind.Modified;
            }
        }
        else
        {
            return ChangeKind.New;
        }
    }
    void WriteTo(Stream stream)
    {
        using StreamWriter wr = new StreamWriter(stream);

        foreach((var i,var j) in dict)
        {       
                wr.WriteLine($"{j.Hash} {j.Size} {i}");
        }

    }

    public static Manifest ReadFrom(Stream stream)
    {
        using StreamReader str = new StreamReader(stream);
        var re = new Manifest();
        while(str.ReadLine() is {} line)
        {   
            if(line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            string[] s = line.Split(' ', 3);
            if(s.Count() != 3)
            {
                throw new InvalidDataException();
            }
            if (!long.TryParse(s[1], out long size))
                throw new InvalidDataException("Invalid file size");
            ManifestEntry er = new ManifestEntry(s[2],size,s[0]);
            re.Add(er);
        }
        return re;
    }

    public void Save(string path)
    {
        using var fl = File.Create(path);

        WriteTo(fl);   

    }    

    public static Manifest Load(string path)
    {
        using var fl = File.OpenRead(path);
        Manifest a = ReadFrom(fl);
        return a;
    }

}
