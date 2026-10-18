namespace Pado.AlgorithmLab.Sorting;

/// <summary>Original gap-halving Shell sort with gap-spaced swaps (not insertion shifts).</summary>
public sealed class ShellSort<T> : ISortAlgorithm<T>
{
    public string Name => "Shell";
    public bool IsStable => false;

    public void Sort(T[] values, SortMetrics? metrics = null, IComparer<T>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(values);
        comparer ??= Comparer<T>.Default;
        for (var gap = values.Length / 2; gap > 0; gap /= 2)
        {
            for (var i = gap; i < values.Length; i++)
            {
                for (var j = i; j >= gap; j -= gap)
                {
                    metrics?.Compare();
                    if (comparer.Compare(values[j - gap], values[j]) <= 0) break;
                    (values[j - gap], values[j]) = (values[j], values[j - gap]);
                    metrics?.Swap();
                }
            }
        }
    }
}
