public class BackupSummary(IReadOnlyList<FileChange> changes)
{
    public IReadOnlyList<FileChange> Changes { get; } = changes;

    public int New => Changes.Count(change => change.Kind == ChangeKind.New);
    public int Modified => Changes.Count(change => change.Kind == ChangeKind.Modified);
    public int Deleted => Changes.Count(change => change.Kind == ChangeKind.Deleted);
    public int Unchanged => Changes.Count(change => change.Kind == ChangeKind.Unchanged);

    public override string ToString()
    {
        return $"New: {New}, modified: {Modified}, deleted: {Deleted}, unchanged: {Unchanged}";
    }
}
