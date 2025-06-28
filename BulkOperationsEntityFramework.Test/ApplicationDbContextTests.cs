using Bogus;
using BulkOperationsEntityFramework.Lib.Services;
using BulkOperationsEntityFramework.Models;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Diagnostics;
using System.Linq;
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
        [TestCaseSource(nameof(NorwegianPluralizationCases))]
        public void CanUsePluralizationService(string word, string expected)
        {
            var norwegianPluralizationService = new NorwegianPluralizationService();
            string pluralizedWord = norwegianPluralizationService.Pluralize(word);
            pluralizedWord.Should().Be(expected, "Norwegian Pluralization service should return the correct plural form of the word.");
        }

        [Test, TestCaseSource(nameof(NorwegianSingularizationCases))]
        public void NorwegianPluralizationService_CanSingularize(string plural, string expectedSingular)
        {
            var norwegianPluralizationService = new NorwegianPluralizationService();

            var actual = norwegianPluralizationService.Singularize(plural);
            Assert.That(actual, Is.EqualTo(expectedSingular), $"Expected singular of '{plural}' to be '{expectedSingular}', but got '{actual}'.");
        }

        public static IEnumerable<TestCaseData> NorwegianPluralizationCases
        {
            get
            {
                yield return new TestCaseData("Bil", "Biler");
                yield return new TestCaseData("Bok", "Bøker");
                yield return new TestCaseData("Hund", "Hunder");
                yield return new TestCaseData("Stol", "Stoler");
                yield return new TestCaseData("Jente", "Jenter");
                yield return new TestCaseData("Gutt", "Gutter");
                yield return new TestCaseData("Lærer", "Lærere");
                yield return new TestCaseData("Barn", "Barn");
                yield return new TestCaseData("Fjell", "Fjell");
                yield return new TestCaseData("Sko", "Sko");
                yield return new TestCaseData("Ting", "Ting");
                yield return new TestCaseData("Mann", "Menn");
                yield return new TestCaseData("Kvinne", "Kvinner");
                yield return new TestCaseData("Bror", "Brødre");
                yield return new TestCaseData("Far", "Fedre");
                yield return new TestCaseData("Mor", "Mødre");
                yield return new TestCaseData("Datter", "Døtre");
                yield return new TestCaseData("Søster", "Søstre");
                yield return new TestCaseData("Øye", "Øyne");
                yield return new TestCaseData("Hand", "Hender");
                yield return new TestCaseData("Fot", "Føtter");
                yield return new TestCaseData("Tå", "Tær");
                yield return new TestCaseData("Tann", "Tenner");
                yield return new TestCaseData("Natt", "Netter");
                yield return new TestCaseData("Tre", "Trær");
                yield return new TestCaseData("Kne", "Knær");
                yield return new TestCaseData("Bonde", "Bønder");

                // _nonEndingWordsInPlural
                yield return new TestCaseData("Mus", "Mus");
                yield return new TestCaseData("Ski", "Ski");
                yield return new TestCaseData("Feil", "Feil");

                // _wordsChangingVowelsInPluralMale
                yield return new TestCaseData("Bot", "Bøter");
                yield return new TestCaseData("Rot", "Røter");

                // _wordsChangingVowelToÆ
                yield return new TestCaseData("Håndkle", "Håndkler");
                yield return new TestCaseData("Kne", "Knær");

                // _wordsForUnits (should not pluralize)
                yield return new TestCaseData("Meter", "Meter");
                yield return new TestCaseData("Gram", "Gram");
                yield return new TestCaseData("Dollar", "Dollar");

                // _wordChangingVowelsInPluralFemale
                yield return new TestCaseData("And", "Ender");
                yield return new TestCaseData("Hånd", "Hender");
                yield return new TestCaseData("Stang", "Stenger");
                yield return new TestCaseData("Strand", "Strender");
                yield return new TestCaseData("Tang", "Tenger");
                yield return new TestCaseData("Tann", "Tenner");

                // _wordsForRelatives (some already covered, but add missing)
                yield return new TestCaseData("Fetter", "Fettere");
                yield return new TestCaseData("Onkel", "Onkler");
                yield return new TestCaseData("Svigerbror", "Svigerbrødre");
                yield return new TestCaseData("Svigerfar", "Svigerfedre");
                yield return new TestCaseData("Svigermor", "Svigermødre");
                yield return new TestCaseData("Svigersøster", "Svigersøstre");

                // _wordsNoPluralizationForNeutralGenderOneSyllable
                yield return new TestCaseData("Hus", "Hus");
                yield return new TestCaseData("Blad", "Blad");

                // _wordsNeutralGenderEndingWithEumOrIum
                yield return new TestCaseData("Museum", "Museer");
                yield return new TestCaseData("Jubileum", "Jubileer");
                yield return new TestCaseData("Kjemikalium", "Kjemikalier");
            }
        }

        public static IEnumerable<TestCaseData> NorwegianSingularizationCases
        {
            get
            {
                yield return new TestCaseData("Biler", "Bil");
                yield return new TestCaseData("Bøker", "Bok");
                yield return new TestCaseData("Hunder", "Hund");
                yield return new TestCaseData("Stoler", "Stol");
                yield return new TestCaseData("Jenter", "Jente");
                yield return new TestCaseData("Gutter", "Gutt");
                yield return new TestCaseData("Lærere", "Lærer");
                yield return new TestCaseData("Barn", "Barn");
                yield return new TestCaseData("Fjell", "Fjell");
                yield return new TestCaseData("Sko", "Sko");
                yield return new TestCaseData("Ting", "Ting");
                yield return new TestCaseData("Menn", "Mann");
                yield return new TestCaseData("Kvinner", "Kvinne");
                yield return new TestCaseData("Brødre", "Bror");
                yield return new TestCaseData("Fedre", "Far");
                yield return new TestCaseData("Mødre", "Mor");
                yield return new TestCaseData("Døtre", "Datter");
                yield return new TestCaseData("Søstre", "Søster");
                yield return new TestCaseData("Øyne", "Øye");
                yield return new TestCaseData("Hender", "Hand");
                yield return new TestCaseData("Føtter", "Fot");
                yield return new TestCaseData("Tær", "Tå");
                yield return new TestCaseData("Tenner", "Tann");
                yield return new TestCaseData("Netter", "Natt");
                yield return new TestCaseData("Trær", "Tre");
                yield return new TestCaseData("Knær", "Kne");
                yield return new TestCaseData("Bønder", "Bonde");
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
        public void TryReusingOpenDbConnection()
        {
            DbConnection conn;
            using (var context = new ApplicationDbContext())
            {
                conn = context.Database.Connection;
                conn.Open();

                var users = context.Users.Take(12).ToList();
                users.Count().Should().Be(12);
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
