namespace Pado.AlgorithmLab.Sorting;

/// <summary>Reproducible datasets based on the original 1664525/1013904223 LCG.</summary>
public static class DatasetFactory
{
    public const uint OriginalSeed = 20260910;

    public static int[] Create(int size, DataOrder order, uint seed = OriginalSeed)
    {
        if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));
        var items = new int[size];
        var state = seed == 0 ? 1u : seed;
        for (var i = 0; i < size; i++)
        {
            switch (order)
            {
                case DataOrder.Random:
                    state = unchecked(1664525u * state + 1013904223u);
                    items[i] = (int)(state % 100000);
                    break;
                case DataOrder.Sorted:
                    items[i] = i;
                    break;
                case DataOrder.Reverse:
                    items[i] = size - i;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(order));
            }
        }
        return items;
    }

    public static IEnumerable<(int Size, DataOrder Order, int[] Data)> AllCases()
    {
        foreach (var size in new[] { 100, 1000, 10000 })
            foreach (var order in Enum.GetValues<DataOrder>())
                yield return (size, order, Create(size, order));
    }
}
