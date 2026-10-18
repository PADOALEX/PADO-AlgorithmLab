namespace Pado.AlgorithmLab.Sorting;

/// <summary>Original stable top-down merge sort; a temporary n-element buffer is used.</summary>
public sealed class MergeSort<T> : ISortAlgorithm<T>
{
    public string Name => "Merge";
    public bool IsStable => true;

    public void Sort(T[] values, SortMetrics? metrics = null, IComparer<T>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(values);
        comparer ??= Comparer<T>.Default;
        var buffer = new T[values.Length];
        SortRange(values, buffer, 0, values.Length - 1, comparer, metrics);
    }

    private static void SortRange(T[] items, T[] buffer, int left, int right,
        IComparer<T> comparer, SortMetrics? metrics)
    {
        if (left >= right) return;
        var middle = left + (right - left) / 2;
        SortRange(items, buffer, left, middle, comparer, metrics);
        SortRange(items, buffer, middle + 1, right, comparer, metrics);
        var a = left;
        var b = middle + 1;
        var output = left;
        while (a <= middle && b <= right)
        {
            metrics?.Compare();
            buffer[output++] = comparer.Compare(items[a], items[b]) <= 0
                ? items[a++] : items[b++];
            metrics?.Move();
        }
        while (a <= middle)
        {
            buffer[output++] = items[a++];
            metrics?.Move();
        }
        while (b <= right)
        {
            buffer[output++] = items[b++];
            metrics?.Move();
        }
        for (var i = left; i <= right; i++)
        {
            items[i] = buffer[i];
            metrics?.Move();
        }
    }
}
