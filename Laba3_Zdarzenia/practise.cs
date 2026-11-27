using System.Diagnostics.Contracts;

static List<string> GetTextFiles(string directory)
{
    List<string > a = new List<string>();
    foreach(string name in Directory.EnumerateFiles(directory))
    {
        if(Path.GetExtension(name) == ".txt")
        {
            a.Add(Path.GetFileName(name));
        }
    }
    return a;  
};


static int CountNonEmptyLines(string path)
{
    using var reader = new StreamReader(path);
    int count = 0;
    while( reader.ReadLine()  is {} line)
    {
        for(int i = 0 ; i < rd ; i++)
        {
            if (!string.IsNullOrWhiteSpace(line))
            count++;
            }
    }
    return count;
};

static void NumberLines(string inputPath, string outputPath)
{
    using var reader = new StreamReader(inputPath);
    using var writer = new StreamWriter(outputPath);

    int count = 1;
    while( reader.ReadLine()  is {} line)
    {

        writer.WriteLine($"{count}: " + line);
        count++;
    }
}

public sealed class CountNonEmptyLines: Stream
{
    private readonly Stream _inner;

    public long BytesWritten{get;private set;}

    public CountingWriteStream(Stream inner)
    {
        this._inner = inner;
    }

    public override void Flush() => _inner.Flush();

    public override int Read(byte[] buffer, int offset, int count)
        => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin)
        => throw new NotSupportedException();

    public override void SetLength(long value)
        => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
        BytesWritten += count;
        _inner.Write(buffer,offset,count);
    }

    protected override void Dispose(bool disposing)
    {

        if (disposing)
            _inner.Dispose();

        base.Dispose(disposing);
    }
    public void man()
    {
        Directory.CreateDirectory("asd");
        Directory.Exists();
        File.Exists();
        
    }
}

