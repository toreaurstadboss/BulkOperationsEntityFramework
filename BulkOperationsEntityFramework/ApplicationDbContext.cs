using BulkOperationsEntityFramework.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace BulkOperationsEntityFramework
{

    public class ApplicationDbContext : DbContext
    {

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


