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
                var user = context.Bruker.FirstOrDefault();
                //Assert.Inconclusive("Check test output");
                Assert.That(user, Is.Not.Null, "User should not be null");

                var userById = context.Bruker.Find(2);
                Assert.That(userById, Is.Not.Null, "User by id should not be null");

                userById.PhoneNumber = Faker.Phone.PhoneNumber();

                var moreBruker = GetBruker(4);
                context.Bruker.AddRange(moreBruker);

                context.SaveChanges();

                var lastAddedUser = moreBruker.Last();
                context.Bruker.Remove(lastAddedUser);

                context.SaveChanges();
            }
        }

        [Test]
        public void CanUseGeneratedStoredProcedures()
        {
            using (var context = new ApplicationDbContext())
            {
                var jubileum = new Jubileum
                {
                    Date = DateTime.Now,
                    Description = "Liberation Day " + Guid.NewGuid().ToString()
                };
                context.Jubileum.Add(jubileum);
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
            //Updated english nouns
            //Sadly some english nouns overlap with norwegian nouns, so the test cases had to be updated
            get
            {
                yield return new TestCaseData("Bil", "Biler");
                yield return new TestCaseData("Bok", "Bøker");
                yield return new TestCaseData("Hund", "Hunder");
                yield return new TestCaseData("Stol", "Stoler");
                yield return new TestCaseData("Jente", "Jenter");
                yield return new TestCaseData("Gutt", "Gutter");
                yield return new TestCaseData("Lærer", "Lærere");
                yield return new TestCaseData("Barn", "Barns"); //english noun 'barn'
                yield return new TestCaseData("Fjell", "Fjell");
                yield return new TestCaseData("Sko", "Sko");
                yield return new TestCaseData("Ting", "Tings"); //english noun 'Ting'
                yield return new TestCaseData("Mann", "Menns"); //english noun 'mann'
                yield return new TestCaseData("Kvinne", "Kvinner");
                yield return new TestCaseData("Bror", "Brødre");
                yield return new TestCaseData("Far", "Fedre");
                yield return new TestCaseData("Mor", "Mødre");
                yield return new TestCaseData("Datter", "Døtre");
                yield return new TestCaseData("Søster", "Søstre");
                yield return new TestCaseData("Øye", "Øyne");
                //yield return new TestCaseData("Hand", "Hender");
                yield return new TestCaseData("Fot", "Føtter");
                yield return new TestCaseData("Tå", "Tær");
                yield return new TestCaseData("Tann", "Tenner");
                yield return new TestCaseData("Natt", "Netter");
                yield return new TestCaseData("Tre", "Trær");
                yield return new TestCaseData("Kne", "Knær");
                yield return new TestCaseData("Bonde", "Bønder");

                // _nonEndingWordsInPlural
                yield return new TestCaseData("Mus", "Mus");
                yield return new TestCaseData("Ski", "Skis"); //english noun 'Skis'
                yield return new TestCaseData("Feil", "Feil");

                // _wordsChangingVowelsInPluralMale
                //yield return new TestCaseData("Bot", "Bøter");
                yield return new TestCaseData("Rot", "Røter");

                // _wordsChangingVowelToÆ
                yield return new TestCaseData("Håndkle", "Håndklær");
                yield return new TestCaseData("Kne", "Knær");

                // _wordsForUnits (should not pluralize)
                yield return new TestCaseData("Meter", "Meters"); //english noun 'Meter'
                yield return new TestCaseData("Gram", "Grams"); //english noun 'Grams'
                yield return new TestCaseData("Dollar", "Dollars"); //english noun 'dollars'

                // _wordChangingVowelsInPluralFemale
                yield return new TestCaseData("And", "Ender");
                yield return new TestCaseData("Hånd", "Hender");
                yield return new TestCaseData("Stang", "Stenger");
                yield return new TestCaseData("Strand", "Strands"); //english noun 'Strand'
                yield return new TestCaseData("Tang", "Tangs"); //english noun 'tangs'
                yield return new TestCaseData("Tann", "Tenner");

                // _wordsForRelatives (some already covered, but add missing)
                yield return new TestCaseData("Fetter", "Fetters"); //english nouns
                yield return new TestCaseData("Onkel", "Onkler");
                yield return new TestCaseData("Svigerbror", "Svigerbrødre");
                yield return new TestCaseData("Svigerfar", "Svigerfedre");
                yield return new TestCaseData("Svigermor", "Svigermødre");
                yield return new TestCaseData("Svigersøster", "Svigersøstre");

                // _wordsNoPluralizationForNeutralGenderOneSyllable
                yield return new TestCaseData("Hus", "Hus");
                yield return new TestCaseData("Blad", "Blad");

                // _wordsNeutralGenderEndingWithEumOrIum
                yield return new TestCaseData("Museum", "Museums"); //english noun 'Museum'
                yield return new TestCaseData("Jubileum", "Jubileer");
                yield return new TestCaseData("Kjemikalium", "Kjemikalier");

                //also test out some english words

                yield return new TestCaseData("Car", "Cars");
                yield return new TestCaseData("Horse", "Horses");
                yield return new TestCaseData("Surgeon", "Surgeons");
                yield return new TestCaseData("Operation", "Operations");
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
                yield return new TestCaseData("Barn", "Barns"); //english noun 'Barn'
                yield return new TestCaseData("Fjell", "Fjell");
                yield return new TestCaseData("Sko", "Sko");
                yield return new TestCaseData("Ting", "Ting");
                yield return new TestCaseData("Menn", "Menns"); //english noun 'Menn'
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
            mockContext.Setup(x => x.Bruker.Find(It.IsAny<int>())).Returns(new Bruker
            {
                Id = 1,
                FirstName = "Test",
                LastName = "MyUser"

            });

            var user = mockContext.Object.Bruker.Find(12);
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
                context.Bruker.Add(new Bruker
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "MyUser"
                });
                context.SaveChanges();

                // Act  
                var user = context.Bruker.Find(1);

                // Assert  
                Assert.That(user, Is.Not.Null, "Mocked user should not be null");
                Assert.That(user.FirstName, Is.EqualTo("Test"), "First name should be 'Test'");
                Assert.That(user.LastName, Is.EqualTo("MyUser"), "Last name should be 'MyUser'");

                Console.WriteLine($"Number of users in in-memory db: {context.Bruker.Count()}");
            }
        }

        [Test]
        public void CanLogDatabaseModificationsEfGeneratedSql()
        {
            using (var context = new ApplicationDbContext())
            {
                context.Database.Log = Console.Write;
                var firstUser = context.Bruker.FirstOrDefault();
                Console.WriteLine(JsonConvert.SerializeObject(firstUser));

                var userById = context.Bruker.Find(2);
                Assert.Pass("Check test output");
            }
        }

        [Test]
        [Ignore("This test should not be run in case of Benchmark has been run, as the target database table will contain many rows")]
        public async Task TestAsyncFetchOfBruker()
        {
            using (var context = new ApplicationDbContext())
            {
                foreach (var i in Enumerable.Range(1, 1))
                {
                    var stopWatch = Stopwatch.StartNew();

                    var user = await context.Bruker.FindAsync(i);
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

                var users = context.Bruker.Where(x => userIds.Contains(x.Id)).ToList();
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

                var users = context.Bruker.Take(12).ToList();
                users.Count().Should().Be(12);
            }
        }

        [Test]
        public async Task TestAsyncFetchOfListOfBruker()
        {
            using (var context = new ApplicationDbContext())
            {
                foreach (var i in Enumerable.Range(1, 1))
                {
                    var stopWatch = Stopwatch.StartNew();

                    var users = await context.Bruker.ToListAsync();
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
                var users = GetBruker(1);
                context.Bruker.AddRange(users);
                context.SaveChanges();
                var firstUser = context.Bruker.FirstOrDefault();
                Console.WriteLine(JsonConvert.SerializeObject(firstUser));
                Assert.Pass("Check test output");
            }
        }

        private static readonly Faker Faker = new Faker();

        private Bruker[] GetBruker(int size) =>
            Enumerable.Range(1, size).Select(i => new Bruker
            {
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.FirstName(),
                LastName = Faker.Name.LastName(),
                PhoneNumber = Faker.Phone.PhoneNumber()
            }).ToArray();

    }
}
