using BenchmarkDotNet.Running;

namespace Pado.AlgorithmLab.Benchmarks;

internal static class Program
{
    private static void Main(string[] args) =>
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
