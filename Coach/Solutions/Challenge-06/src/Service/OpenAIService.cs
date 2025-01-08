using Azure;
using Azure.AI.OpenAI;
using Azure.Core;
using Microsoft.Azure.Cosmos;
using System.Text.RegularExpressions;

namespace AIDevHackathon.ConsoleApp.VectorDB.Recipes.Services;

/// <summary>
/// Service to access Azure OpenAI.
/// </summary>
public class OpenAIService
{

    private readonly string _openAIEmbeddingDeployment = string.Empty;
    private readonly string _openAICompletionDeployment = string.Empty;
    private readonly int _openAIMaxTokens = default;

    private readonly OpenAIClient? _openAIClient;

    //System prompts to send with user prompts to instruct the model for chat session
    private readonly string _systemPromptRecipeAssistant = @"
        You are an intelligent assistant for Contoso Recipes. 
        You are designed to provide helpful answers to user questions about using
        recipes, cooking instructions only using the provided JSON strings.

        Instructions:
        - In case a recipe is not provided in the prompt politely refuse to answer all queries regarding it. 
        - Never refer to a recipe not provided as input to you.
        - If you're unsure of an answer, you can say ""I don't know"" or ""I'm not sure"" and recommend users search themselves.        
        - Your response  should be complete. 
        - List the Name of the Recipe at the start of your response folowed by step by step cooking instructions
        - Assume the user is not an expert in cooking.
        - Format the content so that it can be printed to the Command Line 
        - In case there are more than one recipes you find let the user pick the most appropiate recipe.";

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIService"/> class.
    /// </summary>
    /// <param name="endpoint">The endpoint URI for the OpenAI service.</param>
    /// <param name="key">The API key for the OpenAI service.</param>
    /// <param name="embeddingsDeployment">The deployment name for embeddings.</param>
    /// <param name="CompletionDeployment">The deployment name for completions.</param>
    /// <param name="maxTokens">The maximum number of tokens for the OpenAI service.</param>

    public OpenAIService(string endpoint, string key, string embeddingsDeployment, string CompletionDeployment, string maxTokens)
    {

        _openAIEmbeddingDeployment = embeddingsDeployment;
        _openAICompletionDeployment = CompletionDeployment;
        _openAIMaxTokens = int.TryParse(maxTokens, out _openAIMaxTokens) ? _openAIMaxTokens : 8191;

        //Retry policy for OpenAI client
        OpenAIClientOptions clientOptions = new OpenAIClientOptions()
        {
            Retry =
        {
            Delay = TimeSpan.FromSeconds(2),
            MaxRetries = 10,
            Mode = RetryMode.Exponential
        }
        };

        try
        {

            //Use this as endpoint in configuration to use non-Azure Open AI endpoint and OpenAI model names
            if (endpoint.Contains("api.openai.com"))
                _openAIClient = new OpenAIClient(key, clientOptions);
            else
                _openAIClient = new(new Uri(endpoint), new AzureKeyCredential(key), clientOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"OpenAIService Constructor failure: {ex.Message}");
        }
    }

    /// <summary>
    /// Asynchronously gets the embeddings for the provided data.
    /// </summary>
    /// <param name="data">The data to get embeddings for.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the embeddings as a float array, or null if an error occurs.</returns>
    public async Task<float[]?> GetEmbeddingsAsync(dynamic data)
    {
        try
        {
            // Create embeddings options
            EmbeddingsOptions embeddingsOptions = new()
            {
                DeploymentName = _openAIEmbeddingDeployment,
                Input = { data },
            };

            // Get embeddings
            var response = await _openAIClient.GetEmbeddingsAsync(embeddingsOptions);

            Embeddings embeddings = response.Value;

            // Return embeddings as float array
            float[] embedding = embeddings.Data[0].Embedding.ToArray();

            return embedding;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetEmbeddingsAsync Exception: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Asynchronously gets the chat completion for the provided user prompt and documents.
    /// </summary>
    /// <param name="userPrompt">The user prompt to send to the OpenAI service.</param>
    /// <param name="documents">The documents to provide context for the chat completion.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a tuple with the response,
    /// the number of prompt tokens used, and the number of response tokens used.
    /// </returns>
    public async Task<(string response, int promptTokens, int responseTokens)> GetChatCompletionAsync(string userPrompt, string documents)
    {

        try
        {
            // Create system and user messages
            var systemMessage = new ChatRequestSystemMessage(_systemPromptRecipeAssistant + documents);
            var userMessage = new ChatRequestUserMessage(userPrompt);

            // Create chat completion options
            ChatCompletionsOptions options = new()
            {
                DeploymentName = _openAICompletionDeployment,
                Messages =
            {
                systemMessage,
                userMessage
            },
                MaxTokens = _openAIMaxTokens,
                Temperature = 0.5f, //0.3f,
                NucleusSamplingFactor = 0.95f,
                FrequencyPenalty = 0,
                PresencePenalty = 0
            };

            // Get chat completions
            Azure.Response<ChatCompletions> completionsResponse = await _openAIClient.GetChatCompletionsAsync(options);

            ChatCompletions completions = completionsResponse.Value;

            // Return response, prompt tokens, and response tokens
            return (
                response: completions.Choices[0].Message.Content,
                promptTokens: completions.Usage.PromptTokens,
                responseTokens: completions.Usage.CompletionTokens
            );

        }
        catch (Exception ex)
        {

            string message = $"OpenAIService.GetChatCompletionAsync(): {ex.Message}";
            Console.WriteLine(message);
            throw;

        }
    }

}