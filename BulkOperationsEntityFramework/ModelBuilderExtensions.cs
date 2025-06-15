using BulkOperationsEntityFramework.Attributes;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace BulkOperationsEntityFramework
{

    public static class ModelBuilderExtensions
    {

        /// <summary>
        /// Applies custom code conventions to the specified <see cref="DbModelBuilder"/> instance based on the <see
        /// cref="DbSet{TEntity}"/> types defined in the provided <see cref="DbContext"/>.
        /// </summary>
        /// <remarks>This method inspects the <see cref="DbSet{TEntity}"/> properties of the provided <see
        /// cref="DbContext"/> and applies schema conventions to each entity type. It is typically used to enforce
        /// custom schema rules or configurations during model creation.</remarks>
        /// <param name="modelBuilder">The <see cref="DbModelBuilder"/> instance to which the conventions will be applied.</param>
        /// <param name="context">The <see cref="DbContext"/> containing the <see cref="DbSet{TEntity}"/> types to analyze.</param>
        public static void ApplyCustomCodeConventions(this DbModelBuilder modelBuilder, DbContext context)
        {
            var dbSetTypes = context
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(p => p.PropertyType.GetGenericArguments()[0]);

            foreach (var type in dbSetTypes)
            {
                ApplySchemaAttributeConvention(modelBuilder, type);
            }

        }

        /// <summary>
        /// Adds a convention to apply the Schema attribute to set the schema name of entities in the DbContext.
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="type"></param>
        private static void ApplySchemaAttributeConvention(DbModelBuilder modelBuilder, System.Type type)
        {
            var schema = type.GetCustomAttribute<SchemaAttribute>(false)?.SchemaName;
            if (schema != null)
            {
                var entityMethod = typeof(DbModelBuilder).GetMethod("Entity").MakeGenericMethod(type);
                var entityTypeConfiguration = entityMethod.Invoke(modelBuilder, null);
                var toTableMethod = entityTypeConfiguration.GetType().GetMethod("ToTable", new[] { typeof(string), typeof(string) });
                toTableMethod.Invoke(entityTypeConfiguration, new object[] { type.Name, schema });
            }
        }
    }

}
