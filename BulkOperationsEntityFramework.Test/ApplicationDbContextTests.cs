using Bogus;
using BulkOperationsEntityFramework.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkOperationsEntityFramework.Test
{

    [TestFixture]
    public class ApplicationDbContextTests
    {

        [Test]
        public void CanLogWithoutLogProperty()
        {
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.FirstOrDefault();
                Assert.Inconclusive("Check test output");
            }
        }

        [Test]
        public void CanLogDatabaseModificationsEfGeneratedSql()
        {
            using (var context = new ApplicationDbContext())
            {
                context.Database.Log = Console.Write; 
                var firstUser = context.Users.FirstOrDefault();
                Console.WriteLine(JsonConvert.SerializeObject(firstUser));

                var userById = context.Users.Find(2);
                Assert.Inconclusive("Check test output");
            }
        }

        [Test]
        public void CanLogDatabaseModifications()
        {
            using (var context = new ApplicationDbContext())
            {
                context.Database.Log = Console.Write; // Log SQL to console             
                var users = GetUsers(1);
                context.Users.AddRange(users);
                context.SaveChanges();
                var firstUser = context.Users.FirstOrDefault();
                Console.WriteLine(JsonConvert.SerializeObject(firstUser));
                Assert.Inconclusive("Check test output");
            }
        }

        private static readonly Faker Faker = new Faker();

        private User[] GetUsers(int size) =>
            Enumerable.Range(1, size).Select(i => new User
            {
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.FirstName(),
                LastName = Faker.Name.LastName(),
                PhoneNumber = Faker.Phone.PhoneNumber()
            }).ToArray();

    }
}
