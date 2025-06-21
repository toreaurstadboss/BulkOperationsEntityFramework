using System;

namespace BulkOperationsEntityFramework.Attributes
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SchemaAttribute : Attribute
    {
        private readonly string _schemaName;

        public SchemaAttribute(string schemaName)
        {
            _schemaName = schemaName;
        }

        public string SchemaName => _schemaName;

    }

}
