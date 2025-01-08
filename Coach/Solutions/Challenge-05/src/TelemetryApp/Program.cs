using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TelemetryApp.Plugins.CoffeePlugin;


#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0001

AzureConfiguration config = new AzureConfiguration();

string aoaiModelId = config.AOAIDeploymentId;
string aoaiEndpoint = config.AOAIEndpoint;
string aoaiApiKey = config.AOAIKey;
string aoaiEmbeddingsEndpoint = config.AOAIEmbeddingsEndpoint;
string aoaiEmbeddingsDeploymentId = config.AOAIEmbeddingsDeploymentId;
string searchEndpoint = config.SearchEndpoint;
string searchKey = config.SearchKey;
string searchIndexProducts = config.SearchIndexProducts;

#region Telemetry
var connectionString = config.AppInsightsConnectionString;

var resourceBuilder = ResourceBuilder
    .CreateDefault()
    .AddService("TelemetryApplicationInsightsQuickstart");

// Enable model diagnostics with sensitive data.
AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);

using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource("Microsoft.SemanticKernel*")
    .AddAzureMonitorTraceExporter(options => options.ConnectionString = connectionString)
    .Build();

using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddMeter("Microsoft.SemanticKernel*")
    .AddAzureMonitorMetricExporter(options => options.ConnectionString = connectionString)
    .Build();

using var loggerFactory = LoggerFactory.Create(builder =>
{
    // Add OpenTelemetry as a logging provider
    builder.AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(resourceBuilder);
        options.AddAzureMonitorLogExporter(options => options.ConnectionString = connectionString);
        // Format log messages. This is default to false.
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;
    });
    builder.SetMinimumLevel(LogLevel.Information);
});

IKernelBuilder builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
    deploymentName: "gpt-4o",
    endpoint: @"https://codegeekshub3046809766.openai.azure.com",
    apiKey: "1BWpGSgbTWyIh2pG6Jwopi1cKdPmrZubT5D1LO0FVAW919U0a6kCJQQJ99AKACHYHv6XJ3w3AAAAACOG6UfH"
    );

builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));
Kernel kernel = builder.Build();
#endregion

IChatCompletionService chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();

kernel.Plugins.AddFromType<CoffeePlugin>("CoffeePlugin");

var history = new ChatHistory();
history.AddSystemMessage("You are an AI assistant managing the lights and product information.");

while (true) {
    
    Console.Write("User: ");
    
    history.AddUserMessage(Console.ReadLine());

    // Get the response from the AI
    ChatMessageContent result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        },
        kernel: kernel
    );
    
    Console.WriteLine($"Bot: {result}");
}