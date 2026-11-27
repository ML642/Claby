using System.Data;
using System.IO.Compression;

public class Practise
{
    class Publisher
    {
        public event Action? MessageSent;

        public void SendMessage()
        {
            MessageSent?.Invoke();
        }       
    }

        public  class CountingStream: Stream{

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

            public override long Seek(long offset, SeekOrigin origin)
            => _stream.Seek(offset, origin);

            public override void SetLength(long value)
            => _stream.SetLength(value);
        };

    public static void Main()
    {  
        File.WriteAllBytes("dane.bin",new byte[1_000_000]);
                                      
        using Stream stream = File.Open("dane.bin" , FileMode.Open);
        using Stream output = File.Open("dane.bin.gz",FileMode.Create);


        using CountingStream afterCompression =  new(output);
        using GZipStream gzStream = new GZipStream( afterCompression ,CompressionLevel.SmallestSize);
        using CountingStream beforeCompression = new(gzStream);

        
        stream.CopyTo(beforeCompression);


        Console.WriteLine($"{beforeCompression.BytesRead}");
        Console.WriteLine($"{afterCompression.BytesRead}");            
    }

    
    
}