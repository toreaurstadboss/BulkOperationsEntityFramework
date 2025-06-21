using Bogus;
using BulkOperationsEntityFramework.Models;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlTypes;
using System.Diagnostics;
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
                //Assert.Inconclusive("Check test output");
                Assert.That(user, Is.Not.Null, "User should not be null");

                var userById = context.Users.Find(2);
                Assert.That(userById, Is.Not.Null, "User by id should not be null");

                userById.PhoneNumber = Faker.Phone.PhoneNumber();

                var moreUsers = GetUsers(4);
                context.Users.AddRange(moreUsers);

                context.SaveChanges();

                var lastAddedUser = moreUsers.Last();
                context.Users.Remove(lastAddedUser);

                context.SaveChanges();
            }
        }

        [Test]
        public void CanMockUser()
        {
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(x => x.Users.Find(It.IsAny<int>())).Returns(new User
            {
                Id = 1,
                FirstName = "Test",
                LastName = "MyUser"

            });

            var user = mockContext.Object.Users.Find(12);
            Assert.That(user, Is.Not.Null, "Mocked user should not be null");
            Assert.That(user.FirstName, Is.EqualTo("Test"), "First name should be 'Test'");
            Assert.That(user.LastName, Is.EqualTo("MyUser"), "Last name should be 'MyUser'");
        }

        [Test]
        [NonParallelizable] // Ensure this test runs sequentially to avoid conflicts with the in-memory database 
        [Ignore("Unstable for now. TODO: make it run stable and pass every time.")]
        public void CanMockUserWithEf6Effort()
        {
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();
            // Create an in-memory connection for Effort  
            var connection = Effort.DbConnectionFactory.CreateTransient();

            // Use the connection in your context  
            using (var context = new ApplicationDbContext(connection))
            {
                DbInterception.Remove(new SerilogCommandInterceptor()); // Remove the interceptor to avoid logging in Effort

                // Seed data  
                context.Users.Add(new User
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "MyUser"
                });
                context.SaveChanges();

                // Act  
                var user = context.Users.Find(1);

                // Assert  
                Assert.That(user, Is.Not.Null, "Mocked user should not be null");
                Assert.That(user.FirstName, Is.EqualTo("Test"), "First name should be 'Test'");
                Assert.That(user.LastName, Is.EqualTo("MyUser"), "Last name should be 'MyUser'");

                Console.WriteLine($"Number of users in in-memory db: {context.Users.Count()}");
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
                Assert.Pass("Check test output");
            }
        }

        [Test]
        [Ignore("This test should not be run in case of Benchmark has been run, as the target database table will contain many rows")]
        public async Task TestAsyncFetchOfUsers()
        {
            using (var context = new ApplicationDbContext())
            {
                foreach (var i in Enumerable.Range(1, 1))
                {
                    var stopWatch = Stopwatch.StartNew();

                    var user = await context.Users.FindAsync(i);
                    Assert.That(user, Is.Not.Null);
                    Console.WriteLine($"The test took: {stopWatch.ElapsedMilliseconds} ms");

                }
            }
        }

        [Test]
        public void TestLargeContainsList()
        {
            var userIds = Enumerable.Range(0, 2300);
            using (var context = new ApplicationDbContext())
            {
                context.Database.Log = Console.Write; 

                var users = context.Users.Where(x => userIds.Contains(x.Id)).ToList();
                Assert.That(users.Count, Is.GreaterThan(0));
            }
        }

        [Test]
        public async Task TestAsyncFetchOfListOfUsers()
        {
            using (var context = new ApplicationDbContext())
            {
                foreach (var i in Enumerable.Range(1, 1))
                {
                    var stopWatch = Stopwatch.StartNew();

                    var users = await context.Users.ToListAsync();
                    Assert.That(users, Is.Not.Null);
                    Console.WriteLine($"The test took: {stopWatch.ElapsedMilliseconds} ms");

                }
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
                    Assert.Pass("Check test output");
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
