using BulkOperationsEntityFramework.Conventions;
using BulkOperationsEntityFramework.Models;
using BulkOperationsEntityFramework.Test;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.SqlServer;
using System.Linq;

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

    [DbConfigurationType(typeof(ApplicationDbModelConfiguration))]
    public class ApplicationDbContext : DbContext
    {

        static ApplicationDbContext()
        {
            if (!AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.StartsWith("Effort")))
            {
                DbInterception.Add(new TransientFailureInterceptor()); //add an interceptor that simulates a transient connection error occuring (30% chance of it happening)
                DbInterception.Add(new SerilogCommandInterceptor()); //do not add logging if EF6 Effor is used (for unit testing)
            }
        }

        public ApplicationDbContext(DbConnection connection) : base(connection, false)
        {
        }

        public ApplicationDbContext() : base("name=App")
        {
        }

        public virtual DbSet<User> Users { get; set; }

        public DbSet<ArchivedUser> ArchivedUsers { get; set; }

        public DbSet<Guest> ArchivedGuests { get; set; }

        public DbSet<Session> Session { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Properties<string>().Configure(p => p.HasMaxLength(255)); // Set max length for all string properties

            modelBuilder.ApplyCustomCodeConventions(this); // Apply custom code conventions based on DbSet types. Pass in db context.

            modelBuilder.Conventions.Add(new GuidKeyConvention());

            //modelBuilder.Types().Where(p => p.GetCustomAttributes(false).OfType<SchemaAttribute>().Any())
            //    .Configure(t => t.ToTable(t.ClrType.Name, t.ClrType.GetCustomAttribute<SchemaAttribute>().SchemaName ?? "dbo")); //add support for setting Schema via Schema attribute using custom code convention

        }

    }

}


