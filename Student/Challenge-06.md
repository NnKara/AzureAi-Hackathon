# Challenge 06 - Use Cosmos DB as a Vector DB

 [< Previous Challenge](./Challenge-05.md) - **[Home](../README.md)** - [Next Challenge >](./Challenge-07.md)
 
## Introduction

After using the Azure AI search as a vector store, you figure out that you can use a Vector Database like CosmosDB for grounding your own data.

A vector database is a database designed to store and manage vector embeddings for your records. These vector embeddings are used in similarity search and used as RAG to ground your db data with large languages models (LLMs).

In a vector database, embeddings are indexed and queried through vector search algorithms based on their vector distance or similarity.

## Description

With Cosmos DB, you can store vectors directly in the documents alongside your data. Each document in your database can contain not only traditional schema-free data, but also high-dimensional vectors as other properties of the documents. This colocation of data and vectors allows for efficient indexing and searching, as the vectors are stored in the same logical unit as the data they represent. Keeping vectors and data together simplifies data management, AI application architectures, and the efficiency of vector-based operations

Here's a streamlined process for building a RAG application with Azure Cosmos DB:

- **Data Ingestion**: Store your documents, images, and other content types in Azure Cosmos DB. Utilize the database's support for vector search to index and retrieve vectorized content.

- **Query Execution**: When a user submits a query, Azure Cosmos DB can quickly retrieve the most relevant data using its vector search capabilities.

- **LLM Integration**: Pass the retrieved data to an LLM (e.g., Azure OpenAI) to generate a response. The well-structured data provided by Cosmos DB enhances the quality of the model's output.

- **Response Generation**: The LLM processes the data and generates a comprehensive response, which is then delivered to the user.

In this challenge you are given a partially completed console application demo showcasing the usage of the RAG pattern for integrating Azure Open AI services with custom data in Azure Cosmos DB NoSQL API.

The demo has the following high level tasks:
- Create Cosmos DB container with vector embedding policy (partially completed)
- Get the recipes stored in the database
- Upload data from a [sample recipe dataset](./Resources/Challenge-06/DataSet/Recipe)
- Vectorize the data uploaded (partially completed)
- Perform a vector search and use LLM to generate responses (partially completed)


You should complete the following tasks
1. Deploy CosmosDb by using this [script](./Resources/Challenge-06/DeployCosmosDb.ps1)

2. Ensure that you enable Vector Search in Cosmos DB
```bash
# Ensure that you enable NoSQL Vector Search capability
az cosmosdb update --resource-group $resourceGroup --name $cosmosDbAccountName  --capabilities EnableNoSQLVectorSearch
```
2. Open the solution in Visual Studio or Visual Studio code located [here](./Resources/Challenge-06/src) and make the following changes to meet the success criteria
    - Make the necessary changes in CreateCosmosContainerAsync method
    - Make the necessary changes in GetEmbeddingsAsync method
    - Make the necessary changes in SingleVectorSearch method

In case you are not familiar with .NET, you can use in the [Azure Data Retrieval Augmented Generation Samples](https://github.com/microsoft/AzureDataRetrievalAugmentedGenerationSamples) the CosmosDB-NoSQL_VectorSearch sample for [Java](https://github.com/microsoft/AzureDataRetrievalAugmentedGenerationSamples/tree/main/Java/CosmosDB-NoSQL-VectorSearch) or [Python](https://github.com/microsoft/AzureDataRetrievalAugmentedGenerationSamples/blob/main/Python/CosmosDB-NoSQL_VectorSearch/CosmosDB-NoSQL-Vector_AzureOpenAI_Tutorial.ipynb).

## Success Criteria
- Demonstrate that you have created the Cosmos DB database
- Demonstrate that you push the sample data into Cosmos DB
- Demonstrate that you vectorize the sample data into Cosmos DB.
- Demonstrate that you can ask questions on recipes and get responses on your own data stored in Cosmos DB
- Explain to your coach the main differences of Azure AI Search and CosmosDB used as vector database

## Learning Resources
- [Index and query vector data in .NET - Azure Cosmos DB for NoSQL | Microsoft Learn](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/how-to-dotnet-vector-index-query)
- [How to generate embeddings with Azure OpenAI Service - Azure OpenAI | Microsoft Learn](https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/embeddings?tabs=csharp#how-to-get-embeddings)
- [Retrieval Augmented Generation in Azure Cosmos DB | Microsoft Learn](https://learn.microsoft.com/en-us/azure/cosmos-db/gen-ai/rag)
- [Vector Search in Azure Cosmos DB for NoSQL | Microsoft Learn](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/vector-search)
- [AzureDataRetrievalAugmentedGenerationSamples | GitHub](https://github.com/microsoft/AzureDataRetrievalAugmentedGenerationSamples)
- [Configure role-based access control with Microsoft Entra ID - Azure Cosmos Db | Microsoft Learn](https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-setup-rbac#built-in-role-definitions)
