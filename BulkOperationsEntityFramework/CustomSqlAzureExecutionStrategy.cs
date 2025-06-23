using System;
using System.Data.Entity.SqlServer;

namespace BulkOperationsEntityFramework
{

    public class CustomSqlAzureExecutionStrategy : SqlAzureExecutionStrategy
    {

        public CustomSqlAzureExecutionStrategy(int maxRetryCount, TimeSpan maxDelay)
        : base(maxRetryCount, maxDelay) { }

        protected override bool ShouldRetryOn(Exception ex)
        {
            return base.ShouldRetryOn(ex) || ex is SimulatedTransientSqlException;
        }

    }

}
