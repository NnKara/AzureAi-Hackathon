using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using SK.NLtoSQL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK.NLtoSQL.Services
{
    /// <summary>
    /// Executes a SQL command and returns the result as a JSON string.
    /// </summary>
    /// <param name="sqlCommand">The SQL command to execute.</param>
    /// <returns>A JSON string representing the result of the SQL command.</returns>
    public class DatabaseService
    {
        private string dataSource;
        private string userName;
        private string password;
        private string dbName;
        private SqlConnectionStringBuilder sqlConnectionStringBuilder;

        public DatabaseService(string dataSource, string userName, string password, string dbName)
        {
            this.dataSource = dataSource;
            this.userName = userName;
            this.password = password;
            this.dbName = dbName;
            this.sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = dataSource,
                UserID = userName,
                Password = password,
                InitialCatalog = dbName,
                TrustServerCertificate = true
            };
        }

        /// <summary>
        /// Retrieves database information including database names and descriptions.
        /// </summary>
        /// <returns>A list of DbInfo objects containing database names and descriptions.</returns>
        public List<DbInfo> GetDbInfo()
        {
            List<DbInfo> dbinfo = new List<DbInfo>();

            using (SqlConnection connection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                connection.Open();
                // Get database information sql query
                String sql = @"SELECT 
                                DISTINCT i_s.CATALOG_NAME as Db,
                                [Description] = s.value 
                            FROM 
                                INFORMATION_SCHEMA.SCHEMATA i_s 
                            LEFT OUTER JOIN 
                                sys.extended_properties s 
                            ON 
                                s.major_id = 0
                                AND s.name = 'MS_Description'";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dbinfo.Add(new DbInfo()
                            {
                                DbName = reader["Db"] as string,
                                DbDescription = reader["Description"] as string
                            });
                        }
                    }
                }
            }
            return dbinfo;
        }

        /// <summary>
        /// Retrieves schema information including schema names and descriptions.
        /// </summary>
        /// <returns>A list of SchemaInfo objects containing schema names and descriptions.</returns>
        public List<SchemaInfo> GetSchemaInfo()
        {

            List<SchemaInfo> schemaInfo = new List<SchemaInfo>();

            using (SqlConnection connection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                connection.Open();

                // Get schema information sql query
                String sql = @"(SELECT 
                                [Db] = i_s.CATALOG_NAME,
                                [Schema] = i_s.SCHEMA_NAME, 
                                [Description] = s.value 
                            FROM 
                                INFORMATION_SCHEMA.SCHEMATA i_s 
                            LEFT OUTER JOIN 
                                sys.extended_properties s 
                            ON 
                                s.major_id = SCHEMA_ID(i_s.SCHEMA_NAME)
                                AND s.name = 'MS_Description' )
                            UNION
                            (SELECT 
                                DISTINCT i_s.CATALOG_NAME as Db,
                                [Schema] = NULL,
                                [Description] = s.value 
                            FROM 
                                INFORMATION_SCHEMA.SCHEMATA i_s 
                            LEFT OUTER JOIN 
                                sys.extended_properties s 
                            ON 
                                s.major_id = 0
                                AND s.name = 'MS_Description')
	                            order by Db, [Schema]";
                
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            schemaInfo.Add(new SchemaInfo()
                            {
                                DbName = reader["Db"] as string,
                                SchemaName = reader["Schema"] as string,
                                SchemaDescription = reader["Description"] as string
                            });
                        }
                    }
                }
            }
            return schemaInfo;
        }

        /// <summary>
        /// Retrieves table schema information including table names and descriptions.
        /// </summary>
        /// <param name="schemaName">Optional schema name to filter the results.</param>
        /// <returns>A list of TableInfo objects containing table names and descriptions.</returns>
        public List<TableInfo> GetTableSchemaInfo(string schemaName = "")
        {

            List<TableInfo> schemaTableInfo = new List<TableInfo>();

            using (SqlConnection connection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                connection.Open();

                // Get table information sql query
                String sql = $@"SELECT 
	                            [Db] = i_s.TABLE_CATALOG,
                                [Schema] = i_s.TABLE_SCHEMA,
                                [Table] = i_s.TABLE_NAME,
	                            [Description] = s.value 
                            FROM 
                                INFORMATION_SCHEMA.TABLES i_s 
                            LEFT OUTER JOIN 
                                sys.extended_properties s 
                            ON 
                                s.major_id = OBJECT_ID(i_s.TABLE_SCHEMA+'.'+i_s.TABLE_NAME) 
                                AND s.name = 'MS_Description' 
                            WHERE 
	                            i_s.TABLE_TYPE = 'BASE TABLE' 
                                AND s.minor_id = '0'
                            ORDER BY 
                                i_s.TABLE_NAME";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            schemaTableInfo.Add(new TableInfo()
                            {
                                DbName = reader["Db"] as string,
                                SchemaName = reader["Schema"] as string,
                                TableName = reader["Table"] as string,
                                TableDescription = reader["Description"] as string
                            });
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(schemaName))
            {
                schemaTableInfo = schemaTableInfo.Where(x => x.SchemaName == schemaName).ToList();
            }

            return schemaTableInfo;
        }

        /// <summary>
        /// Retrieves column schema information including column names, data types, and descriptions.
        /// </summary>
        /// <param name="schemaName">Optional schema name to filter the results.</param>
        /// <param name="tableName">Optional table name to filter the results.</param>
        /// <returns>A list of ColumnsInfo objects containing column names, data types, and descriptions.</returns>
        public List<ColumnsInfo> GetColumnSchemaInfo(string schemaName = "", string tableName = "")
        {

            List<ColumnsInfo> schemaColumnInfo = new List<ColumnsInfo>();

            using (SqlConnection connection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                connection.Open();

                // Get column information sql query
                String sql = @"SELECT 
	                            [Db] = i_s.TABLE_CATALOG,
                                [Schema] = i_s.TABLE_SCHEMA,
                                [Table] = i_s.TABLE_NAME, 
                                [Column] = i_s.COLUMN_NAME, 
	                            [DataType] = i_s.DATA_TYPE, 
                                [Description] = s.value 
                            FROM 
                                INFORMATION_SCHEMA.COLUMNS i_s 
                            LEFT OUTER JOIN 
                                sys.extended_properties s 
                            ON 
                                s.major_id = OBJECT_ID(i_s.TABLE_SCHEMA+'.'+i_s.TABLE_NAME) 
                                AND s.minor_id = i_s.ORDINAL_POSITION 
                                AND s.name = 'MS_Description' 
                            WHERE 
                                OBJECTPROPERTY(OBJECT_ID(i_s.TABLE_SCHEMA+'.'+i_s.TABLE_NAME), 'IsMsShipped')=0 
                                -- AND i_s.TABLE_NAME = 'table_name' 
                            ORDER BY 
                                i_s.TABLE_NAME, i_s.ORDINAL_POSITION";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            schemaColumnInfo.Add(new ColumnsInfo()
                            {
                                DbName = reader["Db"] as string,
                                SchemaName = reader["Schema"] as string,
                                TableName = reader["Table"] as string,
                                ColumnName = reader["Column"] as string,
                                ColumnDataType = reader["DataType"] as string,
                                ColumnDescription = reader["Description"] as string
                            });
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(schemaName))
            {
                schemaColumnInfo = schemaColumnInfo.Where(x => x.SchemaName == schemaName).ToList();
            }

            if (!string.IsNullOrEmpty(tableName))
            {
                schemaColumnInfo = schemaColumnInfo.Where(x => x.TableName == tableName).ToList();
            }


            return schemaColumnInfo;
        }

        /// <summary>
        /// Executes a SQL command and returns the result as a JSON string.
        /// </summary>
        /// <param name="sqlCommand">The SQL command to execute.</param>
        /// <returns>A JSON string representing the result of the SQL command.</returns>
        public string ExecuteSqlCommand(string sqlCommand)
        {

            List<ColumnsInfo> schemaColumnInfo = new List<ColumnsInfo>();

            using (SqlConnection connection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sqlCommand, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        return JsonConvert.SerializeObject(dataTable);
                    }
                }
            }
        }
    }
}
