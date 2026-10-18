namespace Pado.AlgorithmLab.Sorting;

/// <summary>One shared contract, without erasing individual stability or complexity.</summary>
public static class SortCatalog
{
    public static IReadOnlyList<ISortAlgorithm<T>> All<T>() =>
        new ISortAlgorithm<T>[] { new SelectionSort<T>(), new ShellSort<T>(), new MergeSort<T>() };

    public static ISortAlgorithm<T> Create<T>(string name) =>
        name?.Trim().ToLowerInvariant() switch
        {
            "selection" or "selectionsort" => new SelectionSort<T>(),
            "shell" or "shellsort" => new ShellSort<T>(),
            "merge" or "mergesort" => new MergeSort<T>(),
            _ => throw new ArgumentException("Use selection, shell, or merge.", nameof(name))
        };
}
