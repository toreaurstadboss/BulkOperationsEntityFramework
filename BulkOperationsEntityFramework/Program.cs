using BenchmarkDotNet.Running;
using BulkOperationsEntityFramework.Benchmarks;

namespace BulkOperationsEntityFramework
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Run the benchmarks in the project
            var summary = BenchmarkRunner.Run(typeof(BulkInsertBenchmark));
        }
    }
}
