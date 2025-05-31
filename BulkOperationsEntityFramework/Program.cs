using BenchmarkDotNet.Running;

namespace BulkOperationsEntityFramework
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Run the benchmarks in this project (assembly)
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}
