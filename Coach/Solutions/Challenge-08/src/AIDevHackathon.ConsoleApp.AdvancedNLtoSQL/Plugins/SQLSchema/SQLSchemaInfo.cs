using Microsoft.SemanticKernel;
using SK.NLtoSQL.Models;
using SK.NLtoSQL.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK.NLtoSQL.Plugins.SQLSchema
{
    /// <summary>
    /// Provides information about the SQL schema.
    /// </summary>
    public sealed class SQLSchemaInfo
    {
        AzureConfiguration azureConfig = new AzureConfiguration();

        //Initialize the database service
        DatabaseService dbService = new DatabaseService("tcp:codegeeks-db.database.windows.net,1433", "sqladmin", "AI@hackathon1234", "AdventureWorks");

        /// <summary>
        /// Get the database information.
        /// </summary>
        /// <returns>A list of <see cref="DbInfo"/> objects representing the database information.</returns>
        [KernelFunction, Description("Get the Database description ")]
        public List<DbInfo> GetDatabaseInfo()
        {
            var info = dbService.GetDbInfo();
            return info;
        }

        /// <summary>
        /// Get the database schemas with their descriptions.
        /// </summary>
        /// <returns>A list of <see cref="SchemaInfo"/> objects representing the database schema information.</returns>
        [KernelFunction, Description("Get the Database schemas with their descriptions ")]
        public List<SchemaInfo> GetDatabaseSchemaInfo()
        {
            var info = dbService.GetSchemaInfo();
            return info;
        }

        /// <summary>
        /// Get the database tables with their descriptions for a specific schema.
        /// </summary>
        /// <param name="schemaName">The name of the schema.</param>
        /// <returns>A list of <see cref="TableInfo"/> objects representing the database table information.</returns>
        [KernelFunction, Description("Get the Database tables with their descriptions for specific schema")]
        public List<TableInfo> GetDatabaseSchemaTableInfo([Description("The schema name")] string schemaName)
        {
            var info = dbService.GetTableSchemaInfo(schemaName);
            return info;
        }

        /// <summary>
        /// Get the database columns with their schema information for a specific schema and table.
        /// </summary>
        /// <param name="schemaName">The name of the schema.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>A list of <see cref="ColumnsInfo"/> objects representing the database column information.</returns>
        [KernelFunction]
        [Description("Get the Database columns with their schema information for specific schema and table")]
        [return: Description("The table columns schema")]
        public List<ColumnsInfo> GetDatabaseSchemaTableColumnsInfo(
            [Description("The schema name")] string schemaName,
            [Description("The table name")] string tableName
            )
        {
            var info = dbService.GetColumnSchemaInfo(schemaName, tableName);
            return info;
        }

        [KernelFunction]
        [Description("Execute sql command and display results, show only the first 10 rows use only read operations, never update or delete anything from database")]
        [return: Description("The result of the sql command, return maximum of 10 rows")]
        public string ExecuteSqlCommand(
           [Description("Sql command")] string sqlCommand
           )
        {
            var info = dbService.ExecuteSqlCommand(sqlCommand);
            return info;
        }
    }
}
