
using System.Security.Cryptography;

public static class FileHasher
{
    
    public static string ComputeSha256(string path)
    {
        using Stream a = File.OpenRead(path);

        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);


        byte[] buffer = new byte[64 * 1024];
        int read;
        while ((read = a.Read(buffer, 0, buffer.Length)) > 0)
        {
            hash.AppendData(buffer, 0, read);
        }
        return Convert.ToHexStringLower(hash.GetHashAndReset());
    }
}
