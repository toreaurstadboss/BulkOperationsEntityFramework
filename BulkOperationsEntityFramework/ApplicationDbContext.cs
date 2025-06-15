using BulkOperationsEntityFramework.Models;
using BulkOperationsEntityFramework.Test;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;

namespace BulkOperationsEntityFramework
{

    public class ApplicationDbContext : DbContext
    {

        static ApplicationDbContext()
        {
            if (!AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.StartsWith("Effort")))
            {
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Properties<string>().Configure(p => p.HasMaxLength(255)); // Set max length for all string properties
        }

    }

}


