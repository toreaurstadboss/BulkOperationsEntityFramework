using BulkOperationsEntityFramework.Lib.Services;
using System;
using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace BulkOperationsEntityFramework
{
    public class ApplicationDbConfiguration : DbConfiguration
    {

        public ApplicationDbConfiguration()
        {
            SetExecutionStrategy(SqlProviderServices.ProviderInvariantName, () =>
             new CustomSqlAzureExecutionStrategy(maxRetryCount: 10, maxDelay: TimeSpan.FromSeconds(5))); //note : max total delay of retries is 30 seconds per default in SQL Server

            SetPluralizationService(new NorwegianPluralizationService());
        }

    }

}


