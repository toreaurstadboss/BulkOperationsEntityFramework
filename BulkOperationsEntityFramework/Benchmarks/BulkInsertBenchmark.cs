using BenchmarkDotNet.Attributes;
using Bogus;
using BulkOperationsEntityFramework.Helpers;
using BulkOperationsEntityFramework.Models;
using Dapper;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BulkOperationsEntityFramework.Benchmarks
{

    [MemoryDiagnoser]
    public class BulkInsertBenchmark
    {

        private static readonly Faker Faker = new Faker();

        [Params(100)]
        public int Size { get; set; }

        //First benchmark - Naive approach - add one and one entity and save changes everytime with round trip to database

        [Benchmark(Description = "EFAddOneAndSave - Add one user and then save. Then add another one. Results in many round-trips to database.")]
        public async Task EFAddOneAndSave()
        {
            using (var context = new ApplicationDbContext())
            {
                foreach (var user in GetUsers())
                {
                    context.Users.Add(user);
                    await context.SaveChangesAsync();
                }
            }
        }

        [Benchmark(Description = "EFAddOneByOneAndSave - Add one by one user, but save once after adding them. Results in one round-trip to database")]
        public async Task EFAddOneByOneAndSave()
        {
            using (var context = new ApplicationDbContext())
            {
                foreach (var user in GetUsers())
                {
                    context.Users.Add(user);

                }
                await context.SaveChangesAsync();
            }
        }

        [Benchmark(Description = "EFAddRange - Adds the users, then does the save. Results in one round-trip to database")]
        public async Task EFAddRange()
        {
            using (var context = new ApplicationDbContext())
            {
                var users = GetUsers();
                context.Users.AddRange(users);
                await context.SaveChangesAsync();
            }
        }

        [Benchmark(Description = "DapperInsertRange - Uses Dapper to insert the users. Results in one round-trip to database")]
        public async Task DapperInsertRange()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["App"].ConnectionString;

            string sql = @"
                INSERT INTO Users (Email, FirstName, LastName, PhoneNumber)
                VALUES (@Email, @FirstName, @LastName, @PhoneNumber)
            ".Trim();

            using (var connection = new SqlConnection(connectionString))
            {
                var users = GetUsers().Select(u => new
                {
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.PhoneNumber
                }).ToArray();

                await connection.ExecuteAsync(sql, users);
            }
        }

        [Benchmark(Description = "SqlBulkCopy - Uses SqlBulkCopy to insert the users. Results in one round-trip to database")]  
        public async Task SqlBulkCopy()
        {
            using (var context = new ApplicationDbContext())
            {
                await context.BulkInsertAsync(GetUsers(), "Users");
            }
        }



        private User[] GetUsers() =>
            Enumerable.Range(1, Size).Select(i => new User
            {
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.FirstName(),
                LastName = Faker.Name.LastName(),
                PhoneNumber = Faker.Phone.PhoneNumber()
            }).ToArray();

    }

}
