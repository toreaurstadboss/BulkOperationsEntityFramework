using System;

namespace BulkOperationsEntityFramework.Lib.Attributes
{

    /// <summary>
    /// Indicates that a property should be ignored during bulk insert operations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class BulkIgnoreAttribute : Attribute
    {
    }

}
