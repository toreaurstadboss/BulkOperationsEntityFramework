using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration.Types;
using BulkOperationsEntityFramework.Lib.Services;

namespace BulkOperationsEntityFramework.Conventions
{

    public class NorwegianPluralizingTableNameConvention : Convention
    {
     
        public NorwegianPluralizingTableNameConvention()
        {
            var norwegianPluralizer = new NorwegianPluralizationService();
            Types().Configure(c =>
            {
                var pluralName = norwegianPluralizer.Pluralize(c.ClrType.Name);
                c.ToTable(pluralName, "");
            });
        }

    }

}