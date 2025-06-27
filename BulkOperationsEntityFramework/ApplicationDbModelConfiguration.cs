using System;
using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace BulkOperationsEntityFramework
{
    public class ApplicationDbModelConfiguration : DbConfiguration
    {

        public ApplicationDbModelConfiguration()
        {
            SetExecutionStrategy(SqlProviderServices.ProviderInvariantName, () =>
             new CustomSqlAzureExecutionStrategy(maxRetryCount: 10, maxDelay: TimeSpan.FromSeconds(5))); //note : max total delay of retries is 30 seconds per default in SQL Server
        }

    }

}


