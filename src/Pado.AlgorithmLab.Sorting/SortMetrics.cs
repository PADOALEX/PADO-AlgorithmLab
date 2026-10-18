namespace Pado.AlgorithmLab.Sorting;

/// <summary>Element comparisons, exchanges, and value assignments (not index checks or allocations).</summary>
public sealed class SortMetrics
{
    public long Comparisons { get; private set; }
    public long Swaps { get; private set; }
    public long Moves { get; private set; }

    internal void Compare() => Comparisons++;
    internal void Move() => Moves++;
    internal void Swap()
    {
        Swaps++;
        Moves += 3; // tmp=a; a=b; b=tmp
    }

    public override string ToString() =>
        $"comparisons={Comparisons:N0}; swaps={Swaps:N0}; moves={Moves:N0}";
}
