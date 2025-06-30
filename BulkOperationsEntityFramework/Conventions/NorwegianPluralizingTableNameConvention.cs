using BulkOperationsEntityFramework.Lib.Services;
using System.Data.Entity.ModelConfiguration.Conventions;

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