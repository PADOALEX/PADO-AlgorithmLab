# Source audit: original educational projects

Audited input files: `DoublyLinkedList(1).zip` and `SortingLabManual(4).zip`. Only source logic was migrated; generated `bin`/`obj` folders were intentionally excluded.

## DoublyLinkedList_Variant9

- `DataStructures/Node.cs`: user-owned node with `Value`, `Previous`, `Next`. Original `int` storage is now `Node<T>`, keeping both pointers readable and limiting their setters to the library.
- `DataStructures/DoublyLinkedList.cs` and `ListOperations/AddHead.cs`, `AddTail.cs`, `Clear.cs`, `Clone.cs`: original head/tail and O(1) append/prepend logic preserved; now generic, with `Count`, bounds-checked indexing, removals and `AssertInvariants`.
- `Algorithms/Task1Average.cs`: original explicit loop and `long` accumulation preserved. No mean is returned for an empty list.
- `Algorithms/Task2RemoveDuplicates.cs`: compares **consecutive** nodes, fixes both links, updates tail, unlinks deleted nodes, and preserves surviving node identities. The new implementation calls a central detach operation and accepts a generic equality comparer.
- `Algorithms/Task3SplitByPosition.cs`: alternates between odd/even **1-based positions**, moves existing nodes, empties the original. This destructive contract is retained.
- The original custom manual testing program exhaustively covered lengths 0..6 over -1,0,1; the updated xUnit suite repeats all **1093 lists**, plus new edge cases.

## SortingLabManual

- `Algorithms/SelectionSortAlgorithm.cs`: scan suffix for minimum, only swap when minimum index changes. Worst-case element comparisons: `n(n-1)/2`.
- `Algorithms/ShellSortAlgorithm.cs`: the **original swap-based** gap-halving implementation, not the more common shift-based insertion variant; preserves operation-count semantics.
- `Algorithms/MergeSortAlgorithm.cs`: top-down recursion, left-side tie wins, buffer writes and copy-back writes counted; stable and O(n) extra space.
- `Models/SortStatistics.cs`: original counts were mutable fields; now managed by `SortMetrics`. Stopwatch times are separated from sorting algorithms.
- `Testing/BenchmarkRunner.cs`: source used 5 warm-up and 20 measurement series, with 100/10/1 runs by size. ManualExperiment retains that educational methodology; BenchmarkDotNet is an **additional**, uninstrumented benchmark path.
- `Data/ManualRandomGenerator.cs` and `Data/ArrayGenerator.cs`: seed `20260910`, LCG constants `1664525` and `1013904223`, modulus `100000`; the original generator was advanced continuously across 100, 1000 and 10000. DatasetFactory reproduces these offsets.
- `Timing/ManualTimer.cs`: platform-specific Win32 / Linux P/Invoke was suitable for the source lab; migrated to cross-platform `Stopwatch` for manual experiments. This changes the timer implementation, **not sorting logic**.

## Intentional design changes

1. Replace a global, non-generic `partial` list with an encapsulated generic library; expose variant-specific algorithms separately.
2. Keep node links readable for educational inspection but not externally mutable.
3. Separate algorithm operation counters from wall-clock timing, and make counters optional for benchmarking.
4. Move to standard xUnit, .NET 10 `.slnx`, test discovery and GitHub Actions.
5. Provide experiment CSV export; do not commit computer-specific timing claims as universal results.

## Validation

The project is checked via the GitHub Actions Release build, xUnit tests and CLI smoke run. Sorting is compared against `Array.Sort` only **in tests**; no library sorting implementation calls `Array.Sort`, `List.Sort`, or `LinkedList<T>`. Original archived reports are retained as historical documentation, not as results measured in this environment.

The refactor is intentionally faithful to the source **algorithms**, but it is not a line-by-line archive of the old application front ends.
