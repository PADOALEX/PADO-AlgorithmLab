using Pado.AlgorithmLab.Sorting;
using Xunit;

namespace Pado.AlgorithmLab.Tests;

public sealed class SortingTests
{
    public static IEnumerable<object[]> Algorithms() =>
        SortCatalog.All<int>().Select(x => new object[] { x.Name });

    [Theory]
    [MemberData(nameof(Algorithms))]
    public void EmptySingleDuplicatesAndBounds_AgreeWithReference(string name)
    {
        foreach (var input in new[]
        {
            Array.Empty<int>(), new[] { 1 }, new[] { 2, 2, 2 },
            new[] { int.MaxValue, int.MinValue, 0, int.MinValue },
            new[] { 1, 2, 3, 4 }, new[] { 4, 3, 2, 1 }
        })
        {
            var expected = (int[])input.Clone();
            Array.Sort(expected);
            var actual = (int[])input.Clone();
            SortCatalog.Create<int>(name).Sort(actual);
            Assert.Equal(expected, actual);
        }
    }

    [Theory]
    [MemberData(nameof(Algorithms))]
    public void RandomizedCases_MatchArraySort(string name)
    {
        foreach (var length in new[] { 0, 1, 2, 3, 10, 100, 1000 })
        {
            foreach (var seed in new[] { 1u, 20260910u, 12345u })
            {
                var input = DatasetFactory.Create(length, DataOrder.Random, seed);
                var expected = (int[])input.Clone();
                Array.Sort(expected);
                var metrics = new SortMetrics();
                SortCatalog.Create<int>(name).Sort(input, metrics);
                Assert.Equal(expected, input);
                Assert.True(metrics.Comparisons >= 0);
            }
        }
    }

    [Theory]
    [InlineData("selection", 3L, 2L, 6L)]
    [InlineData("shell", 3L, 2L, 6L)]
    [InlineData("merge", 3L, 0L, 10L)]
    public void ManualMetrics_AreExplicitAndRepeatable(
        string name, long comparisons, long swaps, long moves)
    {
        var input = new[] { 3, 1, 2 };
        var stats = new SortMetrics();
        SortCatalog.Create<int>(name).Sort(input, stats);
        Assert.Equal(new[] { 1, 2, 3 }, input);
        Assert.Equal(comparisons, stats.Comparisons);
        Assert.Equal(swaps, stats.Swaps);
        Assert.Equal(moves, stats.Moves);
    }

    [Fact]
    public void MergeSort_IsStable_WithCustomComparer()
    {
        var values = new[]
        {
            new Item(2, 0), new Item(1, 1), new Item(2, 2),
            new Item(1, 3), new Item(2, 4)
        };
        new MergeSort<Item>().Sort(values, comparer:
            Comparer<Item>.Create((a, b) => a.Key.CompareTo(b.Key)));
        Assert.Equal(new[] { 1, 3, 0, 2, 4 }, values.Select(x => x.Id).ToArray());
        Assert.True(new MergeSort<Item>().IsStable);
        Assert.False(new ShellSort<int>().IsStable);
        Assert.False(new SelectionSort<int>().IsStable);
    }

    [Fact]
    public void GenericSortSupportsDescendingComparers()
    {
        foreach (var algorithm in SortCatalog.All<int>())
        {
            var values = new[] { 3, 1, 4, 1, 5 };
            algorithm.Sort(values, comparer: Comparer<int>.Create((a, b) => b.CompareTo(a)));
            Assert.Equal(new[] { 5, 4, 3, 1, 1 }, values);
        }
    }

    [Fact]
    public void AllNineDatasets_AreReproducible()
    {
        var cases = DatasetFactory.AllCases().ToArray();
        Assert.Equal(9, cases.Length);
        foreach (var (size, order, values) in cases)
        {
            Assert.Equal(size, values.Length);
            Assert.Equal(values, DatasetFactory.Create(size, order));
        }
        Assert.Throws<ArgumentOutOfRangeException>(() => DatasetFactory.Create(-1, DataOrder.Random));
        Assert.Throws<ArgumentOutOfRangeException>(() => DatasetFactory.Create(1, (DataOrder)99));
    }

    [Fact]
    public void NullArraysAndUnknownAlgorithm_FailExplicitly()
    {
        foreach (var sort in SortCatalog.All<int>())
            Assert.Throws<ArgumentNullException>(() => sort.Sort(null!));
        Assert.Throws<ArgumentException>(() => SortCatalog.Create<int>("unknown"));
    }

    private readonly record struct Item(int Key, int Id);
}
