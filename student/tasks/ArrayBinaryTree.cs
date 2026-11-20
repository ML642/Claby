using System.Collections;

namespace tasks;

/// <summary>
/// Interface for an array-based binary tree.
/// </summary>
public interface IArrayBinaryTree<T> : IEnumerable<T>
{
    /// <summary>
    /// Gets the number of nodes in the tree.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the index of the root.
    /// </summary>
    int RootIndex { get; }

    /// <summary>
    /// Sets the value of the root node.
    /// </summary>
    void SetRoot(T value);

    /// <summary>
    /// Returns the indices of the left and right children for a given parent.
    /// </summary>
    (int leftIndex, int rightIndex) GetChildrenIndices(int parentIndex);

    /// <summary>
    /// Sets the value of the left child of a given parent.
    /// </summary>
    void SetLeftChild(int parentIndex, T value);

    /// <summary>
    /// Sets the value of the right child of a given parent.
    /// </summary>
    void SetRightChild(int parentIndex, T value);

    /// <summary>
    /// Gets the node at the specified index.
    /// </summary>
    T this[int index] { get; }

    /// <summary>
    /// Checks if a node at the specified index exists.
    /// </summary>
    bool Exists(int index);

    /// <summary>
    /// Clears all nodes from the tree.
    /// </summary>
    void Clear();
}

/// <summary>
/// An implementation of a binary tree based on an array.
/// Indices are calculated as:
/// - Left Child: 2 * i + 1
/// - Right Child: 2 * i + 2
/// - Parent: (i - 1) / 2
/// </summary>
public class ArrayBinaryTree<T> : IArrayBinaryTree<T>
{
    private T[] values;
    private bool[] present;

    public ArrayBinaryTree(int capacity = 0)
    {
        capacity = Math.Max(0, capacity);
        values = new T[capacity];
        present = new bool[capacity];
    }

    public T this[int index] => Exists(index)
        ? values[index]
        : throw new ArgumentOutOfRangeException(nameof(index));

    public int Count { get; private set; }

    public int RootIndex => 0;

    public void Clear()
    {
        Array.Clear(values);
        Array.Clear(present);
        Count = 0;
    }

    public bool Exists(int index) => index >= 0 && index < present.Length && present[index];

    public (int leftIndex, int rightIndex) GetChildrenIndices(int parentIndex)
    {
        if (parentIndex < 0 || parentIndex > (int.MaxValue - 2) / 2)
            throw new ArgumentOutOfRangeException(nameof(parentIndex));

        return (parentIndex * 2 + 1, parentIndex * 2 + 2);
    }

    public IEnumerator<T> GetEnumerator() => TraverseInOrder(RootIndex).GetEnumerator();

    private IEnumerable<T> TraverseInOrder(int index)
    {
        if (!Exists(index))
            yield break;

        var (left, right) = GetChildrenIndices(index);
        if (Exists(left))
            foreach (var value in TraverseInOrder(left))
                yield return value;

        yield return values[index];

        if (Exists(right))
            foreach (var value in TraverseInOrder(right))
                yield return value;
    }

    public void SetLeftChild(int parentIndex, T value)
    {
        if (!Exists(parentIndex))
            throw new ArgumentOutOfRangeException(nameof(parentIndex));

        var (left, _) = GetChildrenIndices(parentIndex);
        SetNode(left, value);
    }

    public void SetRightChild(int parentIndex, T value)
    {
        if (!Exists(parentIndex))
            throw new ArgumentOutOfRangeException(nameof(parentIndex));

        var (_, right) = GetChildrenIndices(parentIndex);
        SetNode(right, value);
    }

    public void SetRoot(T value) => SetNode(RootIndex, value);

    private void SetNode(int index, T value)
    {
        EnsureCapacity(index);
        if (!present[index])
        {
            present[index] = true;
            Count++;
        }

        values[index] = value;
    }

    private void EnsureCapacity(int index)
    {
        if (index < values.Length)
            return;

        int capacity = Math.Max(1, values.Length);
        while (capacity <= index)
            capacity = capacity > int.MaxValue / 2 ? index + 1 : capacity * 2;

        Array.Resize(ref values, capacity);
        Array.Resize(ref present, capacity);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
