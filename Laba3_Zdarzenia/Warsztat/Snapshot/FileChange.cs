public enum ChangeKind
{
    Unchanged,
    New,
    Modified,
    Deleted
}

public record FileChange(string Path, ChangeKind Kind);

public static class ChangeKindExtensions
{
    public static char ToSymbol(this ChangeKind kind)
    {
        return kind switch
        {
            ChangeKind.Unchanged => ' ',
            ChangeKind.New => '+',
            ChangeKind.Modified => '~',
            ChangeKind.Deleted => '-',
            _ => ' '
        };
    }
}
