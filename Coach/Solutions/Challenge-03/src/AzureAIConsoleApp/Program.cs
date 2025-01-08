// Install the .NET library via NuGet: dotnet add package Azure.AI.OpenAI --prerelease  
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using Azure.AI.OpenAI.Chat;
using AzureAIConsoleApp;
using OpenAI.Chat;
using static System.Environment;

// Disable the prerelease warning
#pragma warning disable AOAI001


//Get the configuration values
string endpoint = Config.AOAIEndpoint;
string deploymentName = Config.AOAIDeploymentId;
string openAiApiKey = Config.AOAIKey;

string searchEndpoint = Config.SearchEndpoint;
string searchIndex = Config.SearchIndex;
string searchApiKey = Config.SearchKey;

//Create an instance of the AzureOpenAIClient
AzureOpenAIClient azureClient = new(
    new Uri(endpoint),
    new ApiKeyCredential(openAiApiKey));
ChatClient chatClient = azureClient.GetChatClient(deploymentName);

//Add chat completion options with data source 
ChatCompletionOptions options = new ChatCompletionOptions();
options.AddDataSource(new AzureSearchChatDataSource()
{
    Endpoint = new Uri(searchEndpoint),
    IndexName = searchIndex,
    Authentication = DataSourceAuthentication.FromApiKey(searchApiKey),
});

//Add system message and user question
List<ChatMessage> messages = new List<ChatMessage>();
messages.Add(ChatMessage.CreateSystemMessage("You are an AI assistant that helps people find product information."));

Console.WriteLine("Type your question here: ");

while (true)
{
    Console.WriteLine();
    Console.Write("User: ");
    messages.Add(ChatMessage.CreateUserMessage(Console.ReadLine()));

    //Call the chat client to get the response
    ChatCompletion completion = chatClient.CompleteChat(messages, options);

    //Display the response and add it to chat history
    string response = completion.Content[0].Text;
    ChatMessage.CreateAssistantMessage(response);
    Console.WriteLine($"System: {response}");
}
