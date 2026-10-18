namespace Pado.AlgorithmLab.DataStructures;

/// <summary>Original laboratory variant 9: average, adjacent duplicates, 1-based position split.</summary>
public static class Variant9Algorithms
{
    public static bool TryCalculateAverageManual(
        DoublyLinkedList<int> list, out long sum, out double average)
    {
        ArgumentNullException.ThrowIfNull(list);
        sum = 0;
        average = 0;
        var count = 0;
        for (var node = list.Head; node is not null; node = node.Next)
        {
            sum += node.Value;
            count++;
        }
        if (count == 0) return false;
        average = (double)sum / count;
        return true;
    }

    /// <summary>Removes only ADJACENT equal values; preserves the surviving node's identity.</summary>
    public static int RemoveConsecutiveDuplicatesManual<T>(
        DoublyLinkedList<T> list, IEqualityComparer<T>? equalityComparer = null)
    {
        ArgumentNullException.ThrowIfNull(list);
        var equality = equalityComparer ?? EqualityComparer<T>.Default;
        var removed = 0;
        for (var node = list.Head; node is not null && node.Next is not null;)
        {
            if (equality.Equals(node.Value, node.Next.Value))
            {
                list.DetachNode(node.Next);
                removed++;
            }
            else node = node.Next;
        }
        return removed;
    }

    /// <summary>Moves (not copies) original nodes to odd/even 1-based positions; consumes source.</summary>
    public static void SplitByPositionManual<T>(
        DoublyLinkedList<T> source,
        out DoublyLinkedList<T> oddPositions,
        out DoublyLinkedList<T> evenPositions)
    {
        ArgumentNullException.ThrowIfNull(source);
        oddPositions = new DoublyLinkedList<T>();
        evenPositions = new DoublyLinkedList<T>();
        var odd = true;
        Node<T>? node;
        while ((node = source.DetachFirstNode()) is not null)
        {
            if (odd) oddPositions.AppendDetachedNode(node);
            else evenPositions.AppendDetachedNode(node);
            odd = !odd;
        }
    }
}
