using System;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;

namespace BulkOperationsEntityFramework.Conventions
{

    public class GuidKeyConvention : Convention
    {
        public GuidKeyConvention()
        {
            Types().Configure(t =>
            {
                var keyProperty = t.ClrType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.PropertyType == typeof(Guid)
                    && string.Equals(p.Name, "Key", StringComparison.OrdinalIgnoreCase));

                if (keyProperty != null)
                {
                    t.HasKey(keyProperty);
                }
            });            
        }
    }

}
