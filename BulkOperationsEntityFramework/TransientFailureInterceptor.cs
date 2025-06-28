using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;

namespace BulkOperationsEntityFramework
{

    public class TransientFailureInterceptor : DbCommandInterceptor
    {
        private static readonly Random _random = new Random();

        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            SimulateTransientFailure(interceptionContext);
            base.ReaderExecuting(command, interceptionContext);
        }

        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            SimulateTransientFailure(interceptionContext);
            base.ScalarExecuting(command, interceptionContext);
        }

        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            SimulateTransientFailure(interceptionContext);
            base.NonQueryExecuting(command, interceptionContext);
        }

        private void SimulateTransientFailure<TResult>(DbCommandInterceptionContext<TResult> context)
        {
            // Simulate a transient failure 10% of the time
            double r = _random.NextDouble();
            if (r < 0.1)
            {
                var ex = new SimulatedTransientSqlException();
                string info = "Throwing a transient SqlException. ";
                Trace.WriteLine($"{info} {ex.ToString()}");
                context.Exception = ex;
            }
        }
    }

    public class SimulatedTransientSqlException : Exception
    {
        public SimulatedTransientSqlException()
        : base("Simulated transient SQL exception.") { }
    }

}
