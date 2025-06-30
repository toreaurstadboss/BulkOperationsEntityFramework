using BulkOperationsEntityFramework.Attributes;
using BulkOperationsEntityFramework.Lib.Services;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Reflection;

namespace BulkOperationsEntityFramework.Conventions
{
    public class SchemaConvention : Convention
    {
        public SchemaConvention()
        {
            var pluralizer = new NorwegianPluralizationService();

            Types().Configure(c =>
            {
                var schemaAttr = c.ClrType.GetCustomAttribute<SchemaAttribute>(false);
                var tableName = pluralizer.Pluralize(c.ClrType.Name);

                if (schemaAttr != null && !string.IsNullOrEmpty(schemaAttr.SchemaName))
                {
                    c.ToTable(tableName, schemaAttr.SchemaName ?? "dbo");
                }
                else
                {
                    c.ToTable(tableName);
                }
            });
        }
    }
}