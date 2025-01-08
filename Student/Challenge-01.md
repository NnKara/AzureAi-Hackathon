# Challenge 01 - Implement Retrieval Augmented Generation (RAG) with Azure OpenAI

 [< Previous Challenge](./Challenge-00.md) - **[Home](../README.md)** - [Next Challenge >](./Challenge-02.md)
 
## Introduction

You are a Cloud Solution Architect at Contoso. Recently, the CTO requested you to start investigating how to integrate Azure AI services into the existing applications. 

As part of this initiative, you will be focusing on implementing Retrieval Augmented Generation (RAG) with Azure OpenAI. 

Since Contoso is a proud partner with Microsoft, you have the opportunity to leverage cutting-edge AI technologies and figure out how to add intelligence into your applications.


## Description

As an architect, you have conducted a thorough research and developed a comprehensive plan to achieve your  goals by leveraging Azure AI Studio and deploying an Enterprise Chat web app. 

To begin, you will create a project in Azure AI Studio and deploy an Azure OpenAI model. This will allow you to chat in the playground without using your own data, ensuring a smooth initial setup. 
Once confirmed, you will add your own data to the model and conduct testing to ensure it performs accurately with your specific information. 

Finally, you will deploy the web app, providing a robust and interactive chat solution tailored to your needs.
This structured approach ensures that each step is carefully designed, leading to a successful implementation.

You should complete the following steps:

  - Create an Azure AI Studio project.
  - Deploy an Azure OpenAI model (version 0613 or later).
  - Chat in the playground without your data.
  - Add your data, create an Azure AI Search index that will use hybrid queries.
  - Chat in the playground with your data.
  - Deploy your web app  

You can find the sample product information data for grounding your own data [here](./Resources/Challenge-01/Data/product-info)

## Success Criteria

- Demonstrate that you can chat with your own data in AI Studio Chat Playground with Hybrid Search.
- Demonstrate that you get an answer (on product information data) by using the prompt "How much are the TrailWalker hiking shoes".
- Deploy the web app and demonstrate that you can chat on your own data in the deployed app.
  
## Learning Resources
- [Create a project and use the chat playground in Azure AI Studio - Azure AI Studio | Microsoft Learn](https://learn.microsoft.com/en-us/azure/ai-studio/quickstarts/get-started-playground)
- [Deploy an Enterprise Chat web app in the Azure AI Studio playground - Azure AI Studio | Microsoft Learn](https://learn.microsoft.com/en-us/azure/ai-studio/tutorials/deploy-chat-web-app)
- [Azure AI Studio | Microsoft Learn](https://learn.microsoft.com/en-us/azure/ai-studio/what-is-ai-studio)
- [RAG and generative AI - Azure AI Search | Microsoft Learn](https://learn.microsoft.com/en-us/azure/search/retrieval-augmented-generation-overview)
- [Hybrid search - Azure AI Search | Microsoft Learn](https://learn.microsoft.com/en-us/azure/search/hybrid-search-overview)
- [Azure OpenAI Service models - Azure OpenAI | Microsoft Learn](https://learn.microsoft.com/en-us/azure/ai-services/openai/concepts/models?tabs=python-secure%2Cglobal-standard%2Cstandard-chat-completions)
- [Supported regions - Azure AI Search | Microsoft Learn](https://learn.microsoft.com/en-us/azure/search/search-region-support)