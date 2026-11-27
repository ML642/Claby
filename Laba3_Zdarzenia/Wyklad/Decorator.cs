using System.Diagnostics;

public class LoggingStream : Stream
{
    private readonly Stream _inner;

    public LoggingStream(Stream stream)
    {
        this._inner = stream;
    }
    
    public override void Write(byte[] buffer,int offset,int count)
    {
        _inner.Write(buffer,offset,count);
        Console.WriteLine(count);
    }
                            
    public override bool CanRead => _inner.CanRead();
    public override bool CanWrite => _inner.CanWrite();
    public override bool CanSeek => _inner.CanSeek();


}