# PADO AlgorithmLab

**Data Structures & Sorting Algorithms in C# · .NET 10**

[![.NET 10 CI](https://github.com/PADOALEX/PADO-AlgorithmLab/actions/workflows/ci.yml/badge.svg)](https://github.com/PADOALEX/PADO-AlgorithmLab/actions/workflows/ci.yml)
![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)
![Language](https://img.shields.io/badge/Language-C%23-239120?logo=csharp)

A small, intentionally algorithm-focused C# portfolio project. It combines two original university labs: a **custom doubly linked list (variant 9)** and three **hand-written sorting algorithms**. The goal is readable implementations, explicit operational metrics, reproducible experiments, automated tests, and honest performance comparisons — **no Web API, database, or unnecessary application layers**.

> **Original algorithms preserved conceptually:** selection of the minimum and swap; gap-halving Shell sort using gap-separated swaps; top-down merge sort using a temporary buffer; variant-9 list tasks that move the existing nodes when splitting. The extracted source files were audited before refactoring; see [Source audit](docs/SOURCE_AUDIT.md).

## What's included

| Component | Purpose |
| --- | --- |
| [DataStructures](src/Pado.AlgorithmLab.DataStructures) | Own `DoublyLinkedList<T>` / `Node<T>`; links, count, index access, add/remove/clear/clone, forward traversal and invariant checks |
| [Variant9Algorithms](src/Pado.AlgorithmLab.DataStructures/Variant9Algorithms.cs) | Integer average; removal of **adjacent** duplicates; destructive split by **1-based** odd/even positions |
| [Sorting](src/Pado.AlgorithmLab.Sorting) | `ISortAlgorithm<T>`, selection, Shell and merge sorting; custom comparers and optional counters |
| [CLI](src/Pado.AlgorithmLab.Cli) | Interactive demos plus scriptable experiment export |
| [Benchmarks](src/Pado.AlgorithmLab.Benchmarks) | BenchmarkDotNet .NET 10 benchmarks on fresh inputs |
| [Tests](tests/Pado.AlgorithmLab.Tests) | xUnit regression, 1093 exhaustive small-list patterns, reference comparison and edge cases |
| [CI](.github/workflows/ci.yml) | Restore, Release build, unit tests and CLI smoke test on Linux |

## Get started

Install the [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0), clone this repository, then run from the repository root:

```powershell
dotnet restore PADO.AlgorithmLab.slnx
dotnet build PADO.AlgorithmLab.slnx -c Release
dotnet test tests/Pado.AlgorithmLab.Tests/Pado.AlgorithmLab.Tests.csproj -c Release
dotnet run --project src/Pado.AlgorithmLab.Cli -c Release
```

The CLI offers linked-list demo, sorting demo, and experiments. Non-interactive alternatives:

```powershell
dotnet run --project src/Pado.AlgorithmLab.Cli -c Release -- --demo
dotnet run --project src/Pado.AlgorithmLab.Cli -c Release -- --sort shell
dotnet run --project src/Pado.AlgorithmLab.Cli -c Release -- --list
dotnet run --project src/Pado.AlgorithmLab.Cli -c Release -- --experiment Results/manual-results.csv
```

`--experiment` runs all **27 combinations** (3 sorts × 3 sizes × 3 orders) and saves real timings and counters as CSV. The results directory is gitignored: timings must be generated on the machine being measured.

### Programmatic examples

```csharp
using Pado.AlgorithmLab.DataStructures;
using Pado.AlgorithmLab.Sorting;

var list = new DoublyLinkedList<int>(new[] { 1, 1, 3, 3, 2 });
Variant9Algorithms.TryCalculateAverageManual(list, out long sum, out double average);
int removed = Variant9Algorithms.RemoveConsecutiveDuplicatesManual(list);
// list: 1, 3, 2. First node from each run is retained.

Variant9Algorithms.SplitByPositionManual(list, out var odd, out var even);
// odd: 1, 2; even: 3; list is now empty.
// Nodes were moved, not re-created.
odd.AssertInvariants();
even.AssertInvariants();

int[] values = { 8, 5, 3, 9, 1, 6, 2, 4 };
var metrics = new SortMetrics();
ISortAlgorithm<int> sort = new ShellSort<int>();
sort.Sort(values, metrics);
// values: 1, 2, 3, 4, 5, 6, 8, 9
Console.WriteLine(metrics);
```

## Complexity and behavior

| Algorithm / operation | Best time | Average time | Worst time | Extra space | Stable? |
| --- | --- | --- | --- | --- | --- |
| Selection Sort | O(n²) | O(n²) | O(n²) | O(1) | No |
| Shell Sort (gaps n/2, n/4, ..., 1) | Depends on gaps and data | Gap-dependent | O(n²) upper bound | O(1) | No |
| Merge Sort | O(n log n) | O(n log n) | O(n log n) | O(n) | Yes |
| List AddFirst / AddLast | O(1) | O(1) | O(1) | O(1) per node | — |
| List RemoveFirst / RemoveLast | O(1) | O(1) | O(1) | O(1) | — |
| List index lookup / RemoveAt | O(1) at ends | O(n) | O(n) | O(1) | — |
| List average / consecutive-dedup / split | O(n) | O(n) | O(n) | O(1) auxiliary* | — |
| List AssertInvariants | O(n) | O(n) | O(n) | O(1) | — |

\* Split creates two empty list objects but **reuses** existing nodes. Selection Sort and Shell Sort operate in-place. Merge Sort is stable because it selects the left element on equal comparisons. The list is **not thread-safe**; its enumerator detects modifications between iterations.

## Measuring performance correctly

- **Comparisons** = calls to element value comparers, not loop bounds or index checks.
- **Swaps** = logical exchanges between two array slots, excluding self-swaps.
- **Moves** = assignments of element values: a swap costs **3** (temporary plus two writes); merge counts every write to its buffer **and** source array.
- **Manual timings** = `Stopwatch` elapsed milliseconds for sorting with counters turned on; copying input arrays, validation and file output are outside the timed interval. Five warm-up series and twenty measured series; 100 runs per series for n=100, 10 for n=1000, and one for n=10000.
- **BenchmarkDotNet** = instrumentation **off**; fresh working array prepared in `IterationSetup`, outside the benchmark method; `Sort()` is the timed method. It separately reports time and managed allocations. BenchmarkDotNet measurements are **not directly comparable** to the instrumented manual timings.

For methodology and caveats, see [Benchmarks and counters](docs/BENCHMARKS.md). Deterministic data follows the original LCG seed `20260910`, **including the continuous generator stream across dataset sizes**. The unmodified source-lab manual measurements are recorded as **historical**, not relabeled as modern BenchmarkDotNet output.

### BenchmarkDotNet

```powershell
dotnet run --project src/Pado.AlgorithmLab.Benchmarks -c Release -- --list flat
dotnet run --project src/Pado.AlgorithmLab.Benchmarks -c Release -- --filter '*SortingBenchmarks*'
```

Run in Release without a debugger, preferably when the machine is idle. In PowerShell, keep the asterisks inside single quotes. This suite has **27 parameter combinations** and can take time, particularly Selection Sort on 10,000 elements. The [benchmark workflow](.github/workflows/benchmarks.yml) can run it on a GitHub-hosted runner and keep CSV/log artifacts.

**Real benchmark measurements:** the [GitHub Actions benchmark run](https://github.com/PADOALEX/PADO-AlgorithmLab/actions/runs/39146417500) completed all 27 cases on 18 October 2026 (.NET 10.0.13, Ubuntu). For **random inputs, N=10,000**, it reported approximately **66.52 ms Selection**, **1.27 ms Shell** and **3.93 ms Merge** (one run; results include experimental noise). See the permanent [full 27-row BenchmarkDotNet table](docs/BENCHMARK_RESULTS_2026-10-18.md) with the commit, environment, allocation data and uncertainty. Archived [Windows lab timings](docs/HISTORICAL_RESULTS.md) are retained separately, not passed off as fresh BenchmarkDotNet results.

## Source and quality

The original `DoublyLinkedList_Variant9` and `SortingLabManual` archives contained their own `bin/` and `obj/` trees, which were excluded. The refactor retains the teaching algorithms and adds type-safety, tests, testable project boundaries and clear timing semantics. GitHub Actions provides a repeatable baseline. The .NET SDK, xUnit and BenchmarkDotNet are the only development dependencies; no service or database is required.

**Suggested GitHub topics:** `csharp`, `dotnet`, `dotnet10`, `algorithms`, `data-structures`, `sorting-algorithms`, `doubly-linked-list`, `benchmarkdotnet`, `xunit`, `computer-science`.

Maintained as part of the **PADO** C#/.NET portfolio.
