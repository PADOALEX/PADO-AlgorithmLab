using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Pado.AlgorithmLab.Sorting;

namespace Pado.AlgorithmLab.Benchmarks;

/// <summary>Rebuilds an unsorted input OUTSIDE each timed invocation.</summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0, launchCount: 1, warmupCount: 3, iterationCount: 8)]
public class SortingBenchmarks
{
    [Params(100, 1000, 10000)]
    public int Size { get; set; }

    [Params(DataOrder.Random, DataOrder.Sorted, DataOrder.Reverse)]
    public DataOrder Order { get; set; }

    [Params("selection", "shell", "merge")]
    public string Algorithm { get; set; } = "merge";

    private int[] _original = null!;
    private int[] _working = null!;
    private ISortAlgorithm<int> _sort = null!;

    [GlobalSetup]
    public void Prepare()
    {
        _original = DatasetFactory.Create(Size, Order);
        _sort = SortCatalog.Create<int>(Algorithm);
    }

    // IterationSetup is excluded from the measurement and forces a fresh array every time.
    [IterationSetup]
    public void Reset() => _working = (int[])_original.Clone();

    [Benchmark]
    public void Sort() => _sort.Sort(_working); // no instrumentation overhead

    [IterationCleanup]
    public void VerifySorted()
    {
        for (var i = 1; i < _working.Length; i++)
            if (_working[i - 1] > _working[i])
                throw new InvalidOperationException("Invalid benchmark result.");
    }
}
