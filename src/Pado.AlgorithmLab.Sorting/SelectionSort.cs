namespace Pado.AlgorithmLab.Sorting;

/// <summary>Selection sort from the original lab. In-place, unstable, O(n²) comparisons.</summary>
public sealed class SelectionSort<T> : ISortAlgorithm<T>
{
    public string Name => "Selection";
    public bool IsStable => false;

    public void Sort(T[] values, SortMetrics? metrics = null, IComparer<T>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(values);
        comparer ??= Comparer<T>.Default;
        for (var i = 0; i < values.Length - 1; i++)
        {
            var minimum = i;
            for (var j = i + 1; j < values.Length; j++)
            {
                metrics?.Compare();
                if (comparer.Compare(values[j], values[minimum]) < 0) minimum = j;
            }
            if (minimum == i) continue;
            (values[i], values[minimum]) = (values[minimum], values[i]);
            metrics?.Swap();
        }
    }
}
