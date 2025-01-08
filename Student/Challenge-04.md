# Challenge 04 - Use Semantic Kernel as an Orchestrator

 [< Previous Challenge](./Challenge-03.md) - **[Home](../README.md)** - [Next Challenge >](./Challenge-05.md)
 
## Introduction

After completing the integration with Azure Open AI SDK, it's time to take it a step further and use an orchestrator for building intelligent applications.

Semantic Kernel is a lightweight, open-source development kit that lets you easily build AI agents and integrate the latest AI models into your C#, Python, or Java codebase. It serves as an efficient middleware that enables rapid delivery of enterprise-grade solutions. It achieves this by allowing you to define plugins that can be chained together in just a few lines of code.

It combines prompts with existing APIs to perform actions. By describing your existing code to AI models, they’ll be called to address requests. When a request is made the model calls a function, and Semantic Kernel is the middleware translating the model's request to a function call and passes the results back to the model.

![image](../Resources/Images/functioncalling.png)

## Description

This challenge will guide you through the process of developing your first intelligent app with Semantic Kernel.

In just a few steps, you can build your first AI agent with Semantic Kernel in either Python, .NET, or Java.

### Task 1: Light Bulb interaction plugin

As a starting point you can follow the steps below to start development with Semantic Kernel. In this .NET console application example, you will create a plugin, allowing the AI agent to interact with a light bulb.

If you are not familiar enough with .NET you can use the supported programming language (Python or Java) of your preference.

In a console window use the dotnet new command to create a new console app. 

```bash
dotnet new console -n azure-semantic-kernel-sdk-hackathon
```

**Install the SDK and add Logging package**
```bash
dotnet add package Microsoft.SemanticKernel
dotnet add package  Microsoft.Extensions.Logging.Console
```

**Import packages**
```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
```

**Add AI services**
```csharp
// Create kernel
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);
```

**Add enterprise services.** One of the main benefits of using Semantic Kernel is that it supports enterprise-grade services. In this sample, we added the logging service to the kernel to help debug the AI agent.

```csharp
//Disable the experimental warning
#pragma warning disable SKEXP0001

builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));
```

**Build the kernel and retrieve services.** Once the services have been added, we  build the kernel and retrieve the chat completion service for later use.

```csharp
Kernel kernel = builder.Build();

// Retrieve the chat completion service
var chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();
```
**Add plugins.** With plugins, you can give your AI agent the ability to run your code to retrieve information from external sources or to perform actions. In the above example, we added a plugin that allows the AI agent to interact with a light bulb. You should place your Plugins in a separate folder.
In your own code, you can create a plugin that interacts with any external service or API to achieve similar results.

A good practice to structure your Plugins in the project is like this:

```bash
\Plugins
    \LightsPlugin
        - LightsPlugin.cs
```

```csharp
using System.ComponentModel;
using Microsoft.SemanticKernel;
using System.Linq;

public class LightsPlugin
{
// Mock data for the lights
private readonly List<LightModel> lights = new()
{
    new LightModel { Id = 1, Name = "Table Lamp", IsOn = false },
    new LightModel { Id = 2, Name = "Porch light", IsOn = false },
    new LightModel { Id = 3, Name = "Chandelier", IsOn = true }
};

[KernelFunction("get_lights")]
[Description("Gets a list of lights and their current state")]
[return: Description("An array of lights")]
public async Task<List<LightModel>> GetLightsAsync()
{
    return lights;
}

[KernelFunction("change_state")]
[Description("Changes the state of the light")]
[return: Description("The updated state of the light; will return null if the light does not exist")]
public async Task<LightModel?> ChangeStateAsync(int id, bool isOn)
{
    var light = lights.FirstOrDefault(light => light.Id == id);

    if (light == null)
    {
        return null;
    }

    // Update the light with the new state
    light.IsOn = isOn;

    return light;
}
}

public class LightModel
{
[JsonPropertyName("id")]
public int Id { get; set; }

[JsonPropertyName("name")]
public string Name { get; set; }

[JsonPropertyName("is_on")]
public bool? IsOn { get; set; }
}
```
**Add the plugin to the kernel.**
Once you've created your plugin, you can add it to the kernel so the AI agent can access it. In the sample, we added the LightsPlugin class to the kernel.
```csharp
// Add the plugin to the kernel
kernel.Plugins.AddFromType<LightsPlugin>("Lights");
```
**Planning.** Semantic Kernel leverages function calling–a native feature of most LLMs–to provide planning. With function calling, LLMs can request (or call) a particular function to satisfy a user's request. Semantic Kernel then marshals the request to the appropriate function in your codebase and returns the results back to the LLM so the AI agent can generate a final response.

To enable automatic function calling, we first need to create the appropriate execution settings so that Semantic Kernel knows to automatically invoke the functions in the kernel when the AI agent requests them.
```csharp
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};
```
**Invoke the plugin.** Finally, we invoke the AI agent with the plugin. The sample code demonstrates how to generate a non-streaming response, but you can also generate a streaming response by using the GetStreamingChatMessageContentAsync method.

```csharp
// Create chat history
var history = new ChatHistory();

// Get the response from the AI
var result = await chatCompletionService.GetChatMessageContentAsync(
history,
executionSettings: openAIPromptExecutionSettings,
kernel: kernel
);
```

###  Task 2: Current time plugin

In this task, you will create a plugin that allows the AI agent to display the current time. Since large language models (LLMs) are trained on past data and do not have real-time capabilities, they cannot provide the current time on their own. 

By creating this plugin, you will enable the AI agent to call a function that retrieves and displays the current time.

###  Task 3: RAG pattern with Azure AI search plugin

After completing the above plugin you should create a [plugin to retrieve data from external source](https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/using-data-retrieval-functions-for-rag) such us Azure AI Search and generate grounded responses with semantic search. Use the AI Search data source created in previous challenges.

Semantic search utilizes vector databases to understand and retrieve information based on the meaning and context of the query rather than just matching keywords.

Semantic search excels in environments where user queries are complex, open-ended, or require a deeper understanding of the content. For example, searching for "best smartphones for photography" would yield results that consider the context of photography features in smartphones, rather than just matching the words "best," "smartphones," and "photography."

Add the Azure.Search.Documents package
```bash
dotnet add package Azure.Search.Documents
```

When providing an LLM with a semantic search function, you typically only need to define a function with a single search query. The LLM will then use this function to retrieve the necessary information. Below is an example of a semantic search function that uses Azure AI Search to find documents similar to a given query.

```csharp
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

public class ProductInfoPlugin
{
#pragma warning disable SKEXP0001
    private readonly ITextEmbeddingGenerationService _textEmbeddingGenerationService;
    private readonly SearchIndexClient _indexClient;
    private readonly string _indexName;
    public ProductInfoPlugin(ITextEmbeddingGenerationService textEmbeddingGenerationService, SearchIndexClient indexClient, string indexName)
    {
        _textEmbeddingGenerationService = textEmbeddingGenerationService;
        _indexClient = indexClient;
        _indexName = indexName;
    }

    [KernelFunction("Search")]
    [Description("Search for product information to the given query.")]
    public async Task<string> SearchAsync(string query)
    {
        // Convert string query to vector
        ReadOnlyMemory<float> embedding = await _textEmbeddingGenerationService.GenerateEmbeddingAsync(query);

        // Get client for search operations
        SearchClient searchClient = _indexClient.GetSearchClient(_indexName);

        // Configure request parameters
        VectorizedQuery vectorQuery = new(embedding);
        vectorQuery.Fields.Add("contentVector");

        SearchOptions searchOptions = new() { VectorSearch = new() { Queries = { vectorQuery } } };

        // Perform search request
        Response<SearchResults<IndexSchema>> response = await searchClient.SearchAsync<IndexSchema>(searchOptions);

        // Collect search results
        await foreach (SearchResult<IndexSchema> result in response.Value.GetResultsAsync())
        {
            return result.Document.Content; // Return text from first result
        }

        return string.Empty;
    }

    private sealed class IndexSchema
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("contentVector")]
        public ReadOnlyMemory<float> ContentVector { get; set; }
    }
}
```

After implementing the Product Info plugin,  you should add the plugin to your Kernel.
The AddFromObject method allows you to add an instance of the plugin class directly to the plugin collection in case you want to directly control how the plugin is constructed.
So you have to instantiate the plugin object and add it to the Kernel like the example below
```csharp
kernel.Plugins.AddFromObject(productInfoPlugin);
```

> [!NOTE]
> The ProductInfoPlugin constructor has 3 parameters with the following types. You should instantiate these objects before instantiating the plugin.
>  - ITextEmbeddingGenerationService
>  - SearchIndexClient
>  - string (index name)

## Success Criteria
- Ensure that your application is running and you are able to debug the application.
- Ensure that you can interact with the application and switch on or off the light bulbs.
- Ensure that you are able to request the current time and receive an accurate response.
- Set a break point in one of the plugins and hit the break point with a user prompt
- Debug and inspect the chat history object to see the sequence of function calls and results.
- Create a plugin to retrieve data from external source (Azure AI Search) created in previous challenge to generate grounded responses.
- Demonstrate that the user can ask questions on your own data within the application.

## Learning Resources
- [Introduction to Semantic Kernel | Microsoft Learn](https://learn.microsoft.com/en-us/semantic-kernel/overview/)
- [Plugins in Semantic Kernel | Microsoft Learn](https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/?pivots=programming-language-csharp)
- [What are Planners in Semantic Kernel | Microsoft Learn](https://learn.microsoft.com/en-us/semantic-kernel/concepts/planning?pivots=programming-language-csharp)
- [In-depth Semantic Kernel Demos | Microsoft Learn](https://learn.microsoft.com/en-us/semantic-kernel/get-started/detailed-samples?pivots=programming-language-csharp)
- [Semantic Kernel GitHub](https://github.com/microsoft/semantic-kernel)
- [Retrieve data from plugins for RAG | Microsoft Learn](https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/using-data-retrieval-functions-for-rag)
- [GitHub samples, Azure AI Search](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/Search/MyAzureAISearchPlugin.cs)