public record struct Enumm<TKey, TValue, TPriority>
{
    public TKey Key;
    public TValue Value;
    public TPriority Priority;

    public Enumm(TKey key, TValue value, TPriority priority)
    {
        Key = key;
        Value = value;
        Priority = priority;
    }
}

public class Registry<TKey, TValue, TPriority>
    where TKey : notnull, IEquatable<TKey>
    where TPriority : IComparable<TPriority>
{
    private List<Enumm<TKey, TValue, TPriority>> a = new();

    public void Add(
        TKey key,
        TValue value,
        TPriority priority)
    {
        bool found = false;

        foreach (var i in a)
        {
            if (i.Key.Equals(key))
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            a.Add(
                new Enumm<TKey, TValue, TPriority>(
                    key,
                    value,
                    priority
                )
            );
        }
    }

    public bool Remove(TKey key)
    {
        for (int i = 0; i < a.Count; i++)
        {
            if (a[i].Key.Equals(key))
            {
                a.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    public bool Contains(TKey key)
    {
        foreach (var i in a)
        {
            if (i.Key.Equals(key))
            {
                return true;
            }
        }

        return false;
    }

    public TValue? Get(TKey key)
    {
        foreach (var i in a)
        {
            if (i.Key.Equals(key))
            {
                return i.Value;
            }
        }

        return default;
    }

    public bool TryGet(
        TKey key,
        out TValue value)
    {
        foreach (var i in a)
        {
            if (i.Key.Equals(key))
            {
                value = i.Value;
                return true;
            }
        }

        value = default!;
        return false;
    }

    public bool ChangePriority(
        TKey key,
        TPriority prior)
    {
        for (int i = 0; i < a.Count; i++)
        {
            if (a[i].Key.Equals(key))
            {
                var item = a[i];
                item.Priority = prior;
                a[i] = item;

                return true;
            }
        }

        return false;
    }

    public int Count => a.Count;

    public IEnumerable<TValue> FindAll(
        Predicate<TValue> predicate)
    {
        foreach (var i in a)
        {
            if (predicate(i.Value))
            {
                yield return i.Value;
            }
        }
    }

    public TValue? GetHighestPriority()
    {
        if (a.Count == 0)
        {
            return default;
        }

        var best = a[0];

        for (int i = 1; i < a.Count; i++)
        {
            if (a[i].Priority.CompareTo(best.Priority) > 0)
            {
                best = a[i];
            }
        }

        return best.Value;
    }
}