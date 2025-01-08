using AIDevHackathon.ConsoleApp.BasicNLtoSQL.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Xml.Schema;

namespace AIDevHackathon.ConsoleApp.BasicNLtoSQL
{
#pragma warning disable SKEXP0001
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                //Initialize confguration from appsettings.json
                var azureConfig = new AzureConfiguration();

                //Create the kernel builder and add the Azure OpenAI chat completion plugin
                var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(azureConfig.AOAIDeploymentId, azureConfig.AOAIEndpoint, azureConfig.AOAIKey);

                // Add logging
                var logger = new LoggerFactory().CreateLogger("Program");
                builder.Services.AddSingleton<ILogger>(logger);

                // Build the kernel
                Kernel kernel = builder.Build();

                // Import the prompts plugin from the prompt directory
                kernel.ImportPluginFromPromptDirectory("Prompts");

                // Retrieve the chat completion service from the kernel
                IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

                // Create the chat history
                string systemPrompt = @"
                        You are an sql query assistant, you transform natural language to sql statements.
                        ";

                ChatHistory chatMessages = new ChatHistory(systemPrompt);

                //Load the database schema
                string sqlSchema = File.ReadAllText("Data\\dbschema.txt");

                // Start the conversation
                while (true)
                {
                    try
                    {
                        // Get user input
                        System.Console.Write("User > ");
                        string userInput = Console.ReadLine();

                        // Add the user message to the chat history
                        chatMessages.AddUserMessage(userInput);

                        // Invoke the chat completion service with prompt function BasicNLToSQL to get the response from the agent
                        var result =  kernel.InvokeStreamingAsync("Prompts", "BasicNLtoSQL", new() { { "input", userInput },{ "sqlSchema", sqlSchema } } );
                                      
                        // Stream the results
                        string fullMessage = "";
                        await foreach (var content in result)
                        {
                            System.Console.Write(content);
                            fullMessage += content;
                        }
                        System.Console.WriteLine();

                        // Add the message from the agent to the chat history
                        chatMessages.AddAssistantMessage(fullMessage);
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}