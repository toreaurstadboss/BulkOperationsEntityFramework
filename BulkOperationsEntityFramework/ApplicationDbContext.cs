using BulkOperationsEntityFramework.Models;
using BulkOperationsEntityFramework.Test;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;

namespace BulkOperationsEntityFramework
{

    public class ApplicationDbContext : DbContext
    {

        static ApplicationDbContext()
        {
            DbInterception.Add(new SerilogCommandInterceptor());
        }

        public ApplicationDbContext() : base("name=App")
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

    }

}


