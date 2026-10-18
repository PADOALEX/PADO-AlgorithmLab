namespace Pado.AlgorithmLab.Sorting;

public interface ISortAlgorithm<T>
{
    string Name { get; }
    bool IsStable { get; }
    /// <summary>Sorts in place. Counters are optional; no library sort is used.</summary>
    void Sort(T[] values, SortMetrics? metrics = null, IComparer<T>? comparer = null);
}
