using Serilog;
using System;
using System.Data.Entity.SqlServer;

namespace BulkOperationsEntityFramework
{

    public class CustomSqlAzureExecutionStrategy : SqlAzureExecutionStrategy
    {

        [ThreadStatic]
        private static int _currentRetryCount = 0;

        public CustomSqlAzureExecutionStrategy(int maxRetryCount, TimeSpan maxDelay)
        : base(maxRetryCount, maxDelay) { }

        protected override bool ShouldRetryOn(Exception ex)
        {
            _currentRetryCount++;
            Console.WriteLine($"{nameof(CustomSqlAzureExecutionStrategy)}: Retry-count within thread: {_currentRetryCount}");

            Log.Information("{Class}: Retry-count within thread: {RetryCount} {ExceptionType}", nameof(CustomSqlAzureExecutionStrategy), _currentRetryCount, ex.GetType().Name);

            return base.ShouldRetryOn(ex) || ex is SimulatedTransientSqlException;
        }

    }

}
