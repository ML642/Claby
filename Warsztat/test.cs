using System.IO;
using System.Linq;
using Testably.Abstractions.Testing;

internal static class FileCheckDemo
{
    public static void Run()
    {
        MockFileSystem fileSystem = new MockFileSystem();
        string dirPath = fileSystem.Path.Combine(fileSystem.Directory.GetCurrentDirectory(), "dir");
        fileSystem.Directory.CreateDirectory(dirPath);
        string filePath = fileSystem.Path.Combine(dirPath, "file.txt");
        fileSystem.File.WriteAllText(filePath, "Привет из MockFileSystem!");

        foreach (string file in fileSystem.Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories))
        {
            Console.WriteLine(file);
            IEnumerable<string> lines = fileSystem.File.ReadAllLines(file);
            foreach (string line in lines)
                Console.WriteLine(line);
        }
    }
}
