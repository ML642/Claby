

using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

public static class CountingStream: Stream{

    private readonly Stream _stream;

    public long BytesRead {get; private set;}
    public CountingStream(Stream stream){
        _stream = stream;
    }

    public override void Write(byte[] buffer, int offset,int count)
    {

        _stream.Write(buffer,offset,count);
        BytesWritten += count;

    }

    public override bool CanRead => _stream.CanRead;
    public override bool CanSeek => _stream.CanSeek;
    public override bool CanWrite => _stream.CanWrite;  

    public override long Length => _stream.Length;

    public override long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    public override void Flush()=> _stream.Flush();

    public override int Read(byte[] buffer,int offset,int count)=> _stream.Read(buffer,offset,count);

    public override long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);

    public override void SetLength(long value) => _stream.SetLength(value);

        
};