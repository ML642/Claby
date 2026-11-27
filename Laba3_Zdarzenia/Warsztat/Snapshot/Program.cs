public class Qain
{       
        public static void PrintUsage()
        {
            Console.WriteLine("Print usage");
        }
        private static readonly string[] IgnorePatterns = ["*.tmp", "*.bak", "bin", "obj", "cache"];
        public static int Main(string[] args)
        {
                
           switch (args)
            {
                case ["scan", var source]:
                    return Scan(source);
                case ["backup",var source,var repository]:
                    return BackUp(source, repository);
                
                
                default:
                    Qain.PrintUsage();
                    return 1;
            } 
        }

        private static int Scan(string source)
            {
                    if (!Directory.Exists(source))
                    {
                        Console.WriteLine($"plik nie znajdzien: {source}");
                        return 1;
                    }

                    var scanner = new DirectoryScanner(source, IgnorePatterns);

                    List<FileEntry> files = scanner.Scan()
                    .OrderBy(file=>file.RelativePath,StringComparer.Ordinal)
                    .ToList();
                    
                    Console.WriteLine("Scanning ../sample");
                    foreach (FileEntry file in files)
                    {
                        Console.WriteLine($"{file.Size.ToHumanSize()}   {file.RelativePath}");
                    }
                     Console.WriteLine($"{files.Count} files {files.Sum(file=>file.Size).ToHumanSize()} bytes ");
                    files = files.OrderByDescending(file=>file.Size).Take(3).ToList();
                    Console.WriteLine("Largest files");
                    foreach (FileEntry file in files)
                    {
                        Console.WriteLine($"{file.Size.ToHumanSize(),8}   {file.RelativePath}");
                    }
                    return 0;
                }
    private static int BackUp(string source, string repository)
        {
            if (!Directory.Exists(source))
            {
                Console.WriteLine($"Source directory not found: {source}");
                return 1;
            }

            if (PathUtils.IsInside(repository, source))
            {
                Console.WriteLine("Repository cannot be inside source.");
                return 1;
            }

            string manifestPath = Path.Combine(repository, "latest.manifest");
            Manifest? previous = File.Exists(manifestPath)
                ? Manifest.Load(manifestPath)
                : null;

            var current = new Manifest();
            var changes = new List<FileChange>();
            var scanner = new DirectoryScanner(source, IgnorePatterns);

            foreach (var file in scanner.Scan()
                        .OrderBy(f => f.RelativePath, StringComparer.Ordinal))
            {
                string hash = FileHasher.ComputeSha256(file.FullPath);
                var entry = new ManifestEntry(file.RelativePath, file.Size, hash);

                current.Add(entry);
                ChangeKind kind = previous?.Classify(entry) ?? ChangeKind.New;
                changes.Add(new FileChange(entry.Path, kind));
            }

            if (previous is not null)
            {
                foreach (var oldEntry in previous.Entries)
                {
                    if (!current.Contains(oldEntry.Path))
                        changes.Add(new FileChange(oldEntry.Path, ChangeKind.Deleted));
                }
            }

            foreach (var change in changes)
            {
                if(change.Kind != ChangeKind.Unchanged)
                    Console.WriteLine($"{change.Kind.ToSymbol()} {change.Path}");
            }

            Console.WriteLine(
                $"New: {changes.Count(c => c.Kind == ChangeKind.New)}, " +
                $"modified: {changes.Count(c => c.Kind == ChangeKind.Modified)}, " +
                $"deleted: {changes.Count(c => c.Kind == ChangeKind.Deleted)}, " +
                $"unchanged: {changes.Count(c => c.Kind == ChangeKind.Unchanged)}");

            Directory.CreateDirectory(repository);
            current.Save(manifestPath);
            return 0;
        }


}