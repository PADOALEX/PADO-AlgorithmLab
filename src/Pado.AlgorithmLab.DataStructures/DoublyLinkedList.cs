using System.Collections;

namespace Pado.AlgorithmLab.DataStructures;

/// <summary>A mutable, non-thread-safe doubly linked list with owned nodes.</summary>
public sealed class DoublyLinkedList<T> : IEnumerable<T>
{
    private Node<T>? _head;
    private Node<T>? _tail;
    private int _count;
    private int _version;

    public Node<T>? Head => _head;
    public Node<T>? Tail => _tail;
    public int Count => _count;
    public bool IsEmpty => _count == 0;

    public DoublyLinkedList() { }

    public DoublyLinkedList(IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        foreach (var item in items) AddLast(item);
    }

    public Node<T> AddFirst(T value)
    {
        var node = new Node<T>(value) { Next = _head };
        if (_head is null) _tail = node;
        else _head.Previous = node;
        _head = node;
        _count++;
        _version++;
        return node;
    }

    public Node<T> AddLast(T value)
    {
        var node = new Node<T>(value);
        AppendDetachedNode(node);
        return node;
    }

    // Compatibility with the original teaching code.
    public void AddHead(T value) => AddFirst(value);
    public void AddTail(T value) => AddLast(value);

    public T RemoveFirst()
    {
        if (_head is null) throw new InvalidOperationException("The list is empty.");
        return DetachNode(_head).Value;
    }

    public T RemoveLast()
    {
        if (_tail is null) throw new InvalidOperationException("The list is empty.");
        return DetachNode(_tail).Value;
    }

    public T RemoveAt(int index) => DetachNode(NodeAt(index)).Value;

    public bool Remove(T value)
    {
        var comparer = EqualityComparer<T>.Default;
        for (var current = _head; current is not null; current = current.Next)
        {
            if (!comparer.Equals(current.Value, value)) continue;
            DetachNode(current);
            return true;
        }
        return false;
    }

    public T this[int index] => NodeAt(index).Value;

    private Node<T> NodeAt(int index)
    {
        if ((uint)index >= (uint)_count)
            throw new ArgumentOutOfRangeException(nameof(index));
        if (index < _count / 2)
        {
            var node = _head!;
            for (var i = 0; i < index; i++) node = node.Next!;
            return node;
        }
        else
        {
            var node = _tail!;
            for (var i = _count - 1; i > index; i--) node = node.Previous!;
            return node;
        }
    }

    internal Node<T> DetachNode(Node<T> node)
    {
        if (node.Previous is null) _head = node.Next;
        else node.Previous.Next = node.Next;
        if (node.Next is null) _tail = node.Previous;
        else node.Next.Previous = node.Previous;
        node.Previous = null;
        node.Next = null;
        _count--;
        _version++;
        return node;
    }

    internal Node<T>? DetachFirstNode() =>
        _head is null ? null : DetachNode(_head);

    internal void AppendDetachedNode(Node<T> node)
    {
        if (node.Next is not null || node.Previous is not null)
            throw new ArgumentException("The node must be detached.", nameof(node));
        node.Previous = _tail;
        if (_tail is null) _head = node;
        else _tail.Next = node;
        _tail = node;
        _count++;
        _version++;
    }

    public DoublyLinkedList<T> CloneManual() => new(this);

    public void Clear()
    {
        var node = _head;
        while (node is not null)
        {
            var next = node.Next;
            node.Next = null;
            node.Previous = null;
            node = next;
        }
        _head = _tail = null;
        _count = 0;
        _version++;
    }

    /// <summary>Throws if Count or either direction of node links is inconsistent.</summary>
    public void AssertInvariants()
    {
        if (_count < 0) throw new InvalidOperationException("Negative count.");
        if ((_head is null) != (_tail is null) ||
            (_count == 0) != (_head is null))
            throw new InvalidOperationException("Invalid empty/head/tail state.");
        if (_head?.Previous is not null || _tail?.Next is not null)
            throw new InvalidOperationException("Invalid boundary link.");

        var seen = 0;
        Node<T>? previous = null;
        for (var node = _head; node is not null; node = node.Next)
        {
            if (seen >= _count || !ReferenceEquals(node.Previous, previous))
                throw new InvalidOperationException("Invalid forward link or cycle.");
            previous = node;
            seen++;
        }
        if (seen != _count || !ReferenceEquals(previous, _tail))
            throw new InvalidOperationException("Forward count/tail mismatch.");

        seen = 0;
        Node<T>? next = null;
        for (var node = _tail; node is not null; node = node.Previous)
        {
            if (seen >= _count || !ReferenceEquals(node.Next, next))
                throw new InvalidOperationException("Invalid backward link or cycle.");
            next = node;
            seen++;
        }
        if (seen != _count || !ReferenceEquals(next, _head))
            throw new InvalidOperationException("Backward count/head mismatch.");
    }

    public IEnumerator<T> GetEnumerator()
    {
        var version = _version;
        for (var node = _head; node is not null; node = node.Next)
        {
            if (version != _version)
                throw new InvalidOperationException("Collection was modified while enumerating.");
            yield return node.Value;
        }
        if (version != _version)
            throw new InvalidOperationException("Collection was modified while enumerating.");
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
