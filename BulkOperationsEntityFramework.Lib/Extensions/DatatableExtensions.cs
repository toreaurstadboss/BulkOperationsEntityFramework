using BulkOperationsEntityFramework.Lib.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace BulkOperationsEntityFramework.Lib.Extensions
{

    public static class DatatableExtensions
    {

        /// <summary>
        /// Converts an IEnumerable of type T to a DataTable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="columnMappings"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> data, Dictionary<string, string> columnMappings = null)
        {
            var dataTable = new DataTable();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var columnName = columnMappings != null && columnMappings.ContainsKey(prop.Name) ? columnMappings[prop.Name] : prop.Name;
                dataTable.Columns.Add(columnName, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (var item in data)
            {
                var values = properties.Select(p => p.GetValue(item) ?? DBNull.Value).ToArray();
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        /// <summary>
        /// Converts an IEnumerable of type T to a DataTable with specified column mappings. Tailored for Bulk operations.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="columnMappings"></param>
        /// <param name="finalMappings"></param>
        /// <returns></returns>
        public static DataTable ToBulkDataTable<T>(this IEnumerable<T> entities, Dictionary<string, string> columnMappings, out Dictionary<string, string> finalMappings)
        {
            var dataTable = new DataTable();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p =>
                    !Attribute.IsDefined(p, typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute)) &&
                    !Attribute.IsDefined(p, typeof(BulkIgnoreAttribute)))
                .ToArray();

            finalMappings = new Dictionary<string, string>();

            foreach (var prop in properties)
            {
                var columnName = columnMappings != null && columnMappings.ContainsKey(prop.Name)
                    ? columnMappings[prop.Name]
                    : prop.Name;

                dataTable.Columns.Add(columnName, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                finalMappings[prop.Name] = columnName;
            }

            foreach (var entity in entities)
            {
                var values = properties.Select(p => p.GetValue(entity) ?? DBNull.Value).ToArray();
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

    }
}
