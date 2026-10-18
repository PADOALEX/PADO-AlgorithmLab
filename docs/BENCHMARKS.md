# Benchmark methodology

## Two independent measurement modes

### Manual operation counters + wall-clock experiments

Run `dotnet run --project src/Pado.AlgorithmLab.Cli -c Release -- --experiment Results/manual-results.csv`. It runs 27 combinations across `100, 1000, 10000` and `Random, Sorted, Reverse`.

The input is cloned before each timed batch; all algorithms receive the same deterministic original input for a given size/order. There are five warm-up series and twenty measured series. Per-series sorts: 100 (size 100), 10 (1000), one (10000). A result row reports comparison/swap/move counts **per single sort** and mean, minimum and maximum milliseconds **per single sort**. Timing starts immediately before the batch sorting loop and ends after it; counter tracking and sorting allocations are inside timing, while input clone, validation, CSV writing, and output display are outside. Batching introduces a small amortized loop/call overhead.

Metrics count only element-value comparisons. In Selection, an actual exchange adds one swap and three moves. Shell performs gap-separated exchanges, each with the same costs. Merge has no swaps, and counts one move for every write into its temporary array and one for every copy back to the original. These are pedagogical operation counts; they do **not** directly measure memory bandwidth, CPU instructions or allocation count. Comparing move counts across different algorithm families requires respecting these definitions.

### BenchmarkDotNet (uninstrumented)

Run `dotnet run --project src/Pado.AlgorithmLab.Benchmarks -c Release -- --filter '*SortingBenchmarks*'`.

BenchmarkDotNet .NET 10: `IterationSetup` clones the input outside the timed method, then `Sort()` runs exactly one uninstrumented sort. `IterationCleanup` checks sortedness. The benchmark displays distributions and managed allocations; fixed 3 warm-ups and 8 measurement iterations per combination, one process launch. Fresh input avoids accidentally benchmarking sorting an already-sorted array when the order is `Random` or `Reverse`.

Why these numbers may differ: BenchmarkDotNet excludes custom metric counting; manual timings include it. Different machines, runtime versions, CPU frequency, build configuration, power state, JIT and garbage collection affect durations. **Do not compare the historical Windows results to current runner values as a controlled experiment.** The experiment suite uses a deterministic pseudo-random LCG, not cryptographic randomness.

## Interpretation

- Selection Sort has a fixed exact element-comparison count `n(n-1)/2`, even for sorted input.
- Shell Sort's performance depends on `n/2, n/4, ..., 1` gaps and data distribution; results from a different gap sequence are not directly comparable.
- Merge Sort uses O(n) auxiliary storage and is stable, unlike the other two algorithms.
- No timings are hardcoded into production code or asserted in unit tests.
- A platform-specific report is evidence of one run; do not generalize it as a hardware-independent speed guarantee.

## Producing a report suitable for README

1. Run CI for correctness first.
2. Execute the BenchmarkDotNet project on the same machine in Release.
3. Save the generated Markdown and CSV files from `BenchmarkDotNet.Artifacts/results/`.
4. Record the CPU, OS, .NET version, BenchmarkDotNet version, commit SHA and date alongside the measured means and allocations.
5. Re-run when changes to algorithm implementation could affect performance.

The GitHub [Benchmarks workflow](../.github/workflows/benchmarks.yml) uploads the raw benchmark artifacts rather than silently overwriting historical measurements.
