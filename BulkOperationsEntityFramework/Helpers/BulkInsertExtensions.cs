using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace BulkOperationsEntityFramework.Helpers
{
  
    /// <summary>
    /// Provides extension methods for performing bulk insert operations using SqlBulkCopy.
    /// </summary>
    public static class BulkInsertExtensions
    {
        /// <summary>
        /// Performs a bulk insert of the specified entities into the database.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="context">The DbContext instance.</param>
        /// <param name="entities">The collection of entities to insert.</param>
        /// <param name="tableName">
        /// Optional: The name of the destination table. If null, the entity type name is used.
        /// </param>
        /// <param name="columnMappings">
        /// Optional: Custom column mappings (propertyName → columnName).
        /// 
        /// <example>
        /// Example 1: Rename columns
        /// <code>
        /// var mappings = new Dictionary&lt;string, string&gt;
        /// {
        ///     { "Name", "ProductName" },
        ///     { "Price", "UnitPrice" }
        /// };
        /// await context.BulkInsertAsync(products, "Products", mappings);
        /// </code>
        /// </example>
        /// 
        /// <example>
        /// Example 2: Ignore properties using attributes
        /// <code>
        /// public class Customer
        /// {
        ///     public int Id { get; set; }
        ///     public string FullName { get; set; }
        /// 
        ///     [BulkIgnore]
        ///     public string TempNotes { get; set; }
        /// 
        ///     [NotMapped]
        ///     public string CalculatedField { get; set; }
        /// }
        /// 
        /// var mappings = new Dictionary&lt;string, string&gt;
        /// {
        ///     { "FullName", "CustomerName" }
        /// };
        /// await context.BulkInsertAsync(customers, "Customers", mappings);
        /// </code>
        /// </example>
        /// 
        /// <example>
        /// Example 3: Snake_case mapping
        /// <code>
        /// var mappings = new Dictionary&lt;string, string&gt;
        /// {
        ///     { "FirstName", "first_name" },
        ///     { "LastName", "last_name" },
        ///     { "Email", "email_address" }
        /// };
        /// await context.BulkInsertAsync(users, "user_accounts", mappings);
        /// </code>
        /// </example>
        /// </param>
        public static void BulkInsert<T>(this DbContext context, IEnumerable<T> entities, string tableName = null, Dictionary<string, string> columnMappings = null)
            where T : class
        {
            var dataTable = entities.ToBulkDataTable(columnMappings, out var mappings);
            BulkCopy(context, dataTable, tableName ?? typeof(T).Name, mappings);
        }

        /// <summary>
        /// Asynchronously performs a bulk insert of the specified entities into the database.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="context">The DbContext instance.</param>
        /// <param name="entities">The collection of entities to insert.</param>
        /// <param name="tableName">
        /// Optional: The name of the destination table. If null, the entity type name is used.
        /// </param>
        /// <param name="columnMappings">
        /// Optional: Custom column mappings (propertyName → columnName).
        /// 
        /// <example>
        /// Example 1: Rename columns
        /// <code>
        /// var mappings = new Dictionary&lt;string, string&gt;
        /// {
        ///     { "Name", "ProductName" },
        ///     { "Price", "UnitPrice" }
        /// };
        /// await context.BulkInsertAsync(products, "Products", mappings);
        /// </code>
        /// </example>
        /// 
        /// <example>
        /// Example 2: Ignore properties using attributes
        /// <code>
        /// public class Customer
        /// {
        ///     public int Id { get; set; }
        ///     public string FullName { get; set; }
        /// 
        ///     [BulkIgnore]
        ///     public string TempNotes { get; set; }
        /// 
        ///     [NotMapped]
        ///     public string CalculatedField { get; set; }
        /// }
        /// 
        /// var mappings = new Dictionary&lt;string, string&gt;
        /// {
        ///     { "FullName", "CustomerName" }
        /// };
        /// await context.BulkInsertAsync(customers, "Customers", mappings);
        /// </code>
        /// </example>
        /// 
        /// <example>
        /// Example 3: Snake_case mapping
        /// <code>
        /// var mappings = new Dictionary&lt;string, string&gt;
        /// {
        ///     { "FirstName", "first_name" },
        ///     { "LastName", "last_name" },
        ///     { "Email", "email_address" }
        /// };
        /// await context.BulkInsertAsync(users, "user_accounts", mappings);
        /// </code>
        /// </example>
        /// </param>
        public static async Task BulkInsertAsync<T>(this DbContext context, IEnumerable<T> entities, string tableName = null, Dictionary<string, string> columnMappings = null)
            where T : class
        {
            var dataTable = entities.ToBulkDataTable(columnMappings, out var mappings);
            await BulkCopyAsync(context, dataTable, tableName ?? typeof(T).Name, mappings);
        }

        private static void BulkCopy(DbContext context, DataTable table, string tableName, Dictionary<string, string> mappings)
        {
            var connection = (SqlConnection)context.Database.Connection;
            var wasClosed = connection.State == ConnectionState.Closed;

            if (wasClosed)
                connection.Open();

            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = tableName;
                foreach (var map in mappings)
                {
                    bulkCopy.ColumnMappings.Add(map.Key, map.Value);
                }
                bulkCopy.WriteToServer(table);
            }

            if (wasClosed)
                connection.Close();
        }

        private static async Task BulkCopyAsync(DbContext context, DataTable table, string tableName, Dictionary<string, string> mappings)
        {
            var connection = (SqlConnection)context.Database.Connection;
            var wasClosed = connection.State == ConnectionState.Closed;

            if (wasClosed)
                await connection.OpenAsync();

            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = tableName;
                foreach (var map in mappings)
                {
                    bulkCopy.ColumnMappings.Add(map.Key, map.Value);
                }
                await bulkCopy.WriteToServerAsync(table);
            }

            if (wasClosed)
                connection.Close();
        }
    }
}
