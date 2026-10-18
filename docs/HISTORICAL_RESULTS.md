# Historical source-lab measurements

The following numbers are **actual data transcribed from `SortingLabManual(4).zip/SortingLabManual/Results/results.txt`**, **not** new runs of this repository and **not** BenchmarkDotNet results. The archived file identifies the original environment as **Windows NT 10.0.26200.0**, **Release**, **five warm-up** and **twenty measured** series. The original lab used a custom performance-counter timer with manual counters enabled. Hardware model and precise runtime patch were not recorded; this table is retained as provenance, not a transferable performance guarantee.

| N | Input | Selection ms | Shell ms | Merge ms |
| ---: | --- | ---: | ---: | ---: |
| 100 | Random | 0.0066 | 0.0016 | 0.0031 |
| 100 | Sorted | 0.0060 | 0.0010 | 0.0029 |
| 100 | Reverse | 0.0065 | 0.0017 | 0.0031 |
| 1000 | Random | 0.7336 | 0.0790 | 0.0588 |
| 1000 | Sorted | 0.4277 | 0.0131 | 0.0386 |
| 1000 | Reverse | 0.4415 | 0.0220 | 0.0367 |
| 10000 | Random | 61.3110 | 1.5409 | 1.0817 |
| 10000 | Sorted | 40.6485 | 0.1982 | 0.4799 |
| 10000 | Reverse | 40.7930 | 0.2906 | 0.4354 |

These are milliseconds **per individual sort** with counting overhead included. The original report also includes exact operation counts; for example random N=100: Selection 4950 comparisons / 98 swaps / 294 moves; Shell 810 comparisons / 358 swaps / 1074 moves; Merge 539 comparisons / 0 swaps / 1344 moves.

To obtain **new** results, run `--experiment` or BenchmarkDotNet on the current code, with the environment and commit recorded.
