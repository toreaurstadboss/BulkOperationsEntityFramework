using Serilog;
using System;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;

namespace BulkOperationsEntityFramework.Test
{

    /// <summary>
    /// Intercepts Entity Framework database commands and logs them using Serilog.
    /// 
    /// This interceptor captures and logs SQL command text for NonQuery, Reader, and Scalar operations.
    /// Logging is configured via Serilog and can be customized using <c>App.config</c> or <c>Web.config</c> settings.
    /// 
    /// <para>
    /// <b>Configuration via AppSettings:</b>
    /// </para>
    /// <list type="table">
    ///   <item>
    ///     <term>serilog:write-to:File.path</term>
    ///     <description>Path to the log file. Default: <c>databaselogs\log.txt</c></description>
    ///   </item>
    ///   <item>
    ///     <term>serilog:write-to:File.rollingInterval</term>
    ///     <description>Rolling interval for log files (e.g., <c>Day</c>, <c>Hour</c>). Default: <c>Day</c></description>
    ///   </item>
    ///   <item>
    ///     <term>serilog:minimum-level</term>
    ///     <description>Minimum log level (e.g., <c>Information</c>, <c>Warning</c>). Default: <c>Information</c></description>
    ///   </item>
    ///   <item>
    ///     <term>serilog:write-to:File.retainedFileCountLimit</term>
    ///     <description>Number of log files to retain. Default: <c>21</c></description>
    ///   </item>
    /// </list>
    /// 
    /// <para>
    /// <b>Example App.config override:</b>
    /// </para>
    /// <code language="xml">
    /// &lt;appSettings&gt;
    ///   &lt;add key="serilog:write-to:File.path" value="C:\Logs\mydb.log" /&gt;
    ///   &lt;add key="serilog:write-to:File.rollingInterval" value="Hour" /&gt;
    ///   &lt;add key="serilog:minimum-level" value="Warning" /&gt;
    ///   &lt;add key="serilog:write-to:File.retainedFileCountLimit" value="10" /&gt;
    /// &lt;/appSettings&gt;
    /// </code>
    /// </summary>
    public class SerilogCommandInterceptor : IDbCommandInterceptor
    {

        private static bool _isInitialized = false;

        public SerilogCommandInterceptor()
        {
            if (_isInitialized)
            {
                return;
            }

            var logPath = ConfigurationManager.AppSettings["serilog:write-to:File.path"] ?? "databaselogs\\log.txt";
            var logIntervalRaw = ConfigurationManager.AppSettings["serilog:write-to:File.rollingInterval"];
            var logInterval = Enum.TryParse(logIntervalRaw, true, out RollingInterval interval) ? interval : RollingInterval.Day;
            var minLevelRaw = ConfigurationManager.AppSettings["serilog:minimum-level"];
            var logLevel = Enum.TryParse(minLevelRaw, true, out Serilog.Events.LogEventLevel level) ? level : Serilog.Events.LogEventLevel.Information;
            var retainedCountRaw = ConfigurationManager.AppSettings["serilog:write-to:File.retainedFileCountLimit"];
            var retainedCount = int.TryParse(retainedCountRaw, out int count) ? count : 21;

            //Set up Serilog logging for the database logging interceptor - set up the minimum level to Information and
            //write to a file with rolling intervals. Set up the file size limit to 500 MB per file
            //in case the log file grows too large on a given day, it will roll over to a new file with the with a running number suffixed to it 
            //the logs will be stored in the "databaselogs" subfolder, or configured path in config file
            //logs will be kept or a maximum number of 21 days or specified number of days
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(logLevel)
                .WriteTo.File(
                    logPath, 
                    rollingInterval: logInterval, 
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 500 * 1000 * 1000,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] [SQL] {Message:lj}{NewLine}",
                    retainedFileCountLimit: retainedCount
                )
                .CreateLogger();

            _isInitialized = true;
        }

        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) { }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) =>
            Log.Information("{Tag} {Sql}", GetSqlTag(command.CommandText), CompactAndInterpolateSql(command));

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) { }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) =>
            Log.Information("{Tag} {Sql}", GetSqlTag(command.CommandText), CompactAndInterpolateSql(command));

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) { }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) =>
            Log.Information("{Tag} {Sql}", GetSqlTag(command.CommandText), CompactAndInterpolateSql(command));

        private string CompactAndInterpolateSql(DbCommand dbCommand)
        {
            string sql = InterpolateSql(dbCommand);
            return CompactSql(sql);
        }

        private string CompactSql(string sql) =>
            sql.Replace(Environment.NewLine, " ").Replace("\n", "").Replace("\r", " ").Trim();

        private string GetSqlTag(string sql)
        {
            if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase)) return "[SELECT]";
            if (sql.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase)) return "[INSERT]";
            if (sql.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase)) return "[UPDATE]";
            if (sql.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase)) return "[DELETE]";
            return "[SQL]";
        }

        private string InterpolateSql(DbCommand dbCommand)
        {
            string sql = dbCommand.CommandText;
            foreach (DbParameter parameter in dbCommand.Parameters)
            {
                string value = FormatParameterValue(parameter.Value);
                sql = sql.Replace(parameter.ParameterName, value);
            }
            return sql;
        }

        private string FormatParameterValue(object value)
        { 
            if (value == null || value == DBNull.Value)
            {
                return "NULL";
            }
            if (value is string || value is DateTime || value is Guid)
            {
                return $"'{value}'"; // Wrap strings, DateTime, and Guid in single quotes
            }
            if (value is bool b)
            {
                return b ? "1" : "0";
            }

            return value.ToString();
        }        

    }
}
