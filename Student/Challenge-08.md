# Challenge 08 - Advanced Natural Language to SQL with Semantic Kernel and RAG

 [< Previous Challenge](./Challenge-07.md) - **[Home](../README.md)** - [Next Challenge >](./Challenge-09.md)
 
## Introduction
After completing the initial task of converting natural language to SQL queries, you will now optimize your solution. In this challenge, you will implement the RAG pattern, avoiding the need to pass the entire SQL schema into the LLM context. You will utilize the Semantic Kernel's ability to determine which table schemas to include in the LLM context. Additionally, you will enable the Semantic Kernel to execute SQL queries and display the results to the user.

## Description
In the previous exercise, you practiced converting natural language queries into SQL statements by using the Semantic Kernel prompt plugin, passing the SQL schema into the LLM context.

You will be given a sample database (Adventure Works) and you should issue the SQL statements based on the database schema. You can find the scripts to deploy the database in Azure [here](./Resources/Challenge-08/deploy-sql.azcli)

Create a plugin with the following functions. Please provide the semantic descriptions of the functions and parameters so the AI agent can understand them. Once you describe the functions and parameters, you enable Semantic Kernel Auto Invocation to make the right decisions and call the functions automatically based on the semantic meaning of your intent.
- **Get database Info** -> Get the Database description
- **Get database Schema Info** -> Get the Database schemas with their descriptions
- **Get Database Schema Table Info** -> Get the Database tables with their descriptions for specific schema
- **Get Database Schema Table Columns Info** -> Get the Database columns with their schema information for specific schema and table
- **Execute Sql Command** -> Execute sql command and display results, show only the first 10 rows, use only read operations, never update or delete anything in the database

You can create your functions like the example below

```csharp
        //Initialize the database service
        DatabaseService db = new DatabaseService("<add you data source, i.e nl-to-sql.database.windows.net>", "<SQL user name>", "<SQL password>", "<database Name>");


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
```
You can find the database service and include it in your solution [here](./Resources/Challenge-08/DatabaseService.cs)

## Success Criteria
- Demonstrate that you have deployed the Azure SQL database and you successfully import the Adventure Works sample database.
- Demonstrate that you have created the plugin with all necessary functions. The functions shall use the methods provided in Database Service.
- Demonstrate that you can ask questions in natural language and you get the answers with the actual data.
- Explain to your coach how the Function calling with chat completion works.
- Explain to your coach how the Auto Function Invocation works.

## Learning Resources
- [Provide native code to your agents | Microsoft Learn](https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/adding-native-plugins?pivots=programming-language-csharp)
- [Inspection of telemetry data with Application Insights | Microsoft Learn](https://learn.microsoft.com/en-us/semantic-kernel/concepts/enterprise-readiness/observability/telemetry-with-app-insights?tabs=Powershell&pivots=programming-language-csharp)
- (Auto Function Invocation | Microsoft Learn)[https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/chat-completion/function-calling/function-invocation?pivots=programming-language-csharp#auto-function-invocation]
- (Function calling with chat completion | Microsoft Learn)[https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/chat-completion/function-calling/?pivots=programming-language-csharp]