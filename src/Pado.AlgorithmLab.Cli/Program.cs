using System.Globalization;
using Pado.AlgorithmLab.DataStructures;
using Pado.AlgorithmLab.Sorting;

namespace Pado.AlgorithmLab.Cli;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 0)
            {
                switch (args[0].ToLowerInvariant())
                {
                    case "--help" or "-h" or "help": Help(); return 0;
                    case "--demo" or "demo": ListDemo(); SortDemo(); return 0;
                    case "--list" or "list": ListDemo(); return 0;
                    case "--sort" or "sort":
                        SortDemo(args.Length >= 2 ? args[1] : "merge"); return 0;
                    case "--experiment" or "experiment":
                        Experiment(args.Length >= 2 ? args[1] : "Results/manual-results.csv");
                        return 0;
                    default: Help(); return 2;
                }
            }

            while (true)
            {
                Console.WriteLine("\nPADO AlgorithmLab | 1: Linked list  2: Sort  3: Experiment  0: Exit");
                Console.Write("> ");
                switch (Console.ReadLine()?.Trim())
                {
                    case null or "0": return 0;
                    case "1": ListDemo(); break;
                    case "2":
                        Console.Write("Algorithm (selection / shell / merge): ");
                        SortDemo(Console.ReadLine()?.Trim() ?? "merge");
                        break;
                    case "3": Experiment("Results/manual-results.csv"); break;
                    default: Console.WriteLine("Select 0, 1, 2 or 3."); break;
                }
            }
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException
                                           or IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"Error: {exception.Message}");
            return 1;
        }
    }

    private static void Help()
    {
        Console.WriteLine("PADO AlgorithmLab (.NET 10)");
        Console.WriteLine("  --demo               Run both educational demonstrations");
        Console.WriteLine("  --list               Run variant 9 list algorithms");
        Console.WriteLine("  --sort [name]        selection | shell | merge");
        Console.WriteLine("  --experiment [path] Run 27 instrumented experiments and export CSV");
        Console.WriteLine("  --help               Show this help");
        Console.WriteLine("Without arguments: interactive menu.");
    }

    private static void ListDemo()
    {
        var list = new DoublyLinkedList<int>(new[] { 4, 4, 7, 7, 7, 2, 4 });
        list.AssertInvariants();
        Console.WriteLine("\nVariant 9 doubly linked list:");
        Console.WriteLine("Original: " + string.Join(", ", list));
        if (Variant9Algorithms.TryCalculateAverageManual(list, out var sum, out var mean))
            Console.WriteLine($"Sum={sum}; average={mean.ToString("F3", CultureInfo.InvariantCulture)}");
        var removed = Variant9Algorithms.RemoveConsecutiveDuplicatesManual(list);
        list.AssertInvariants();
        Console.WriteLine($"Removed {removed} adjacent duplicates: {string.Join(", ", list)}");
        Variant9Algorithms.SplitByPositionManual(list, out var odd, out var even);
        list.AssertInvariants();
        odd.AssertInvariants();
        even.AssertInvariants();
        Console.WriteLine("Odd positions:  " + string.Join(", ", odd));
        Console.WriteLine("Even positions: " + string.Join(", ", even));
        Console.WriteLine("Source count after moving nodes: " + list.Count);
    }

    private static void SortDemo(string name = "merge")
    {
        var algorithm = SortCatalog.Create<int>(name);
        var input = new[] { 8, 5, 3, 9, 1, 6, 2, 4 };
        var metrics = new SortMetrics();
        Console.WriteLine($"\n{algorithm.Name} Sort (stable={algorithm.IsStable})");
        Console.WriteLine("Input:  " + string.Join(", ", input));
        algorithm.Sort(input, metrics);
        Console.WriteLine("Sorted: " + string.Join(", ", input));
        Console.WriteLine("Manual counters: " + metrics);
    }

    private static void Experiment(string path)
    {
        Console.WriteLine("Instrumented experiments: 3 algorithms x 3 sizes x 3 input orders.");
        Console.WriteLine("Results depend on this machine; all timing includes counters.");
        var data = ManualExperiment.Run(Console.Out);
        ManualExperiment.WriteCsv(path, data);
        Console.WriteLine($"Saved {data.Count} rows to {Path.GetFullPath(path)}");
    }
}
