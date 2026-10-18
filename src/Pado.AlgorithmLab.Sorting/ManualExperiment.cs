using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Pado.AlgorithmLab.Sorting;

/// <summary>Instrumented educational experiment; NOT a replacement for BenchmarkDotNet.</summary>
public static class ManualExperiment
{
    public const int WarmupSeries = 5;
    public const int MeasurementSeries = 20;

    public sealed record Result(
        string Algorithm, int Size, DataOrder Order,
        long Comparisons, long Swaps, long Moves,
        double MeanMilliseconds, double MinMilliseconds, double MaxMilliseconds);

    public static IReadOnlyList<Result> Run(TextWriter? progress = null)
    {
        var results = new List<Result>();
        foreach (var (size, order, source) in DatasetFactory.AllCases())
        {
            foreach (var algorithm in SortCatalog.All<int>())
            {
                var copiesPerSeries = size <= 100 ? 100 : size <= 1000 ? 10 : 1;
                for (var i = 0; i < WarmupSeries; i++)
                    RunSeries(algorithm, source, copiesPerSeries, out _);

                SortMetrics? canonical = null;
                var total = 0.0;
                var min = double.MaxValue;
                var max = 0.0;
                for (var i = 0; i < MeasurementSeries; i++)
                {
                    var milliseconds = RunSeries(algorithm, source, copiesPerSeries, out var metrics);
                    if (canonical is null) canonical = metrics;
                    else if (canonical.Comparisons != metrics.Comparisons ||
                             canonical.Swaps != metrics.Swaps || canonical.Moves != metrics.Moves)
                        throw new InvalidOperationException("Non-deterministic metrics.");
                    total += milliseconds;
                    min = Math.Min(min, milliseconds);
                    max = Math.Max(max, milliseconds);
                }
                var entry = new Result(algorithm.Name, size, order,
                    canonical!.Comparisons, canonical.Swaps, canonical.Moves,
                    total / MeasurementSeries, min, max);
                results.Add(entry);
                progress?.WriteLine($"{algorithm.Name,-9} {size,5} {order,-7} {entry.MeanMilliseconds,12:F5} ms");
            }
        }
        return results;
    }

    private static double RunSeries(ISortAlgorithm<int> algorithm, int[] source, int count,
        out SortMetrics metrics)
    {
        // Copy outside timer, as in the source lab. Timing includes metric instrumentation.
        var copies = new int[count][];
        for (var i = 0; i < count; i++) copies[i] = (int[])source.Clone();

        metrics = new SortMetrics();
        var start = Stopwatch.GetTimestamp();
        for (var i = 0; i < count; i++)
        {
            var current = i == 0 ? metrics : new SortMetrics();
            algorithm.Sort(copies[i], current);
            if (i > 0 && (current.Comparisons != metrics.Comparisons ||
                          current.Swaps != metrics.Swaps || current.Moves != metrics.Moves))
                throw new InvalidOperationException("Different counts on identical data.");
        }
        var elapsed = Stopwatch.GetElapsedTime(start).TotalMilliseconds / count;
        foreach (var copy in copies)
            for (var i = 1; i < copy.Length; i++)
                if (copy[i - 1] > copy[i])
                    throw new InvalidOperationException("Algorithm returned unsorted data.");
        return elapsed;
    }

    public static void WriteCsv(string path, IReadOnlyList<Result> results)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(results);
        var sb = new StringBuilder("Algorithm,Size,Order,Comparisons,Swaps,Moves,MeanMs,MinMs,MaxMs\n");
        foreach (var row in results)
            sb.AppendLine(string.Join(",", row.Algorithm, row.Size.ToString(CultureInfo.InvariantCulture),
                row.Order.ToString(), row.Comparisons.ToString(CultureInfo.InvariantCulture),
                row.Swaps.ToString(CultureInfo.InvariantCulture),
                row.Moves.ToString(CultureInfo.InvariantCulture),
                row.MeanMilliseconds.ToString("F6", CultureInfo.InvariantCulture),
                row.MinMilliseconds.ToString("F6", CultureInfo.InvariantCulture),
                row.MaxMilliseconds.ToString("F6", CultureInfo.InvariantCulture)));
        var directory = Path.GetDirectoryName(Path.GetFullPath(path))!;
        Directory.CreateDirectory(directory);
        var temporary = path + ".tmp";
        File.WriteAllText(temporary, sb.ToString(), Encoding.UTF8);
        File.Move(temporary, path, true);
    }
}
