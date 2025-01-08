using Microsoft.Extensions.Configuration;
using Spectre.Console;
using Console = Spectre.Console.AnsiConsole;
using System.Net;
using System.Net.Quic;
using System.Diagnostics;
using Newtonsoft.Json;
using Spectre.Console.Extensions;
using Spectre.Console.Json;
using AIDevHackathon.ConsoleApp.VectorDB.Recipes.Services;
using AIDevHackathon.ConsoleApp.VectorDB.Recipes.Service;


namespace AIDevHackathon.ConsoleApp.VectorDB.Recipes
{
    internal class Program
    {

        static CosmosDbService cosmosService = null;
        static OpenAIService openAIEmbeddingService = null;

        static async Task Main(string[] args)
        {
            // Display the application title
            AnsiConsole.Write(
               new FigletText("Vector DB Recipes")
               .Color(Color.Red));

            Console.WriteLine("");

            // Load configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);

            var config = configuration.Build();

            //Initialize the Cosmos DB service
            cosmosService = initCosmosDBService(config);

            //Menu items available
            const string createContainer = "1.\tCreate container and index in Cosmos DB";
            const string cosmosDocumentsLoaded = "2.\tGet recipe(s) loaded to Cosmos DB";
            const string cosmosUpload = "3.\tUpload recipe(s) to Cosmos DB";
            const string vectorize = "4.\tVectorize the recipe(s) and store it in Cosmos DB";
            const string search = "5.\tAsk AI Assistant (search for a recipe by name or description, or ask a question)";
            const string exit = "6.\tExit this Application";


            while (true)
            {
                // Display the menu
                var selectedOption = AnsiConsole.Prompt(
                      new SelectionPrompt<string>()
                          .Title("Select an option to continue")
                          .PageSize(10)
                          .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                          .AddChoices(new[] {
                            createContainer,cosmosDocumentsLoaded,cosmosUpload,vectorize ,search, exit
                          }));

                // Process the selected option
                switch (selectedOption)
                {
                    case createContainer:
                        //Create Cosmos Container
                        createCosmosContainer(config);
                        break;
                    case cosmosDocumentsLoaded:
                        //Get all recipes
                        GetAllRecipes(config);
                        break;
                    case cosmosUpload:
                        //Upload recipes
                        UploadRecipes(config);
                        break;
                    case vectorize:
                        //Generate Embeddings
                        GenerateEmbeddings(config);
                        break;
                    case search:
                        //Perform Search
                        PerformSearch(config);
                        break;
                    case exit:
                        return;
                }
            }

        }


        /// <summary>
        /// Initializes the OpenAI service with the provided configuration.
        /// </summary>
        /// <param name="config">The configuration containing the OpenAI service settings.</param>
        /// <returns>An instance of <see cref="OpenAIService"/> initialized with the provided settings.</returns>
        private static OpenAIService initOpenAIService(IConfiguration config)
        {
            // Get the OpenAI service settings from the configuration
            string endpoint = config["OpenAIEndpoint"];
            string key = config["OpenAIKey"];
            string embeddingDeployment = config["OpenAIEmbeddingDeployment"];
            string completionsDeployment = config["OpenAIcompletionsDeployment"];
            string maxToken = config["OpenAIMaxToken"];

            // Initialize the OpenAI service
            return new OpenAIService(endpoint, key, embeddingDeployment, completionsDeployment, maxToken);
        }


        /// <summary>
        /// Initializes the Cosmos DB service with the provided configuration.
        /// </summary>
        /// <param name="config">The configuration containing the Cosmos DB service settings.</param>
        /// <returns>An instance of <see cref="CosmosDbService"/> initialized with the provided settings.</returns>
        private static CosmosDbService initCosmosDBService(IConfiguration config)
        {
            CosmosDbService cosmosService = null;

            // Get the Cosmos DB service settings from the configuration
            string endpoint = config["CosmosUri"];
            string databaseName = config["CosmosDatabase"];
            string containerName = config["CosmosContainer"];
            string key = config["CosmosKey"];

            int recipeWithEmbedding = 0;
            int recipeWithNoEmbedding = 0;
            bool containerExist = false;

            AnsiConsole.Status()
                .Start("Processing...", ctx =>
                {
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("green"));

                    ctx.Status("Creating Cosmos DB Client ..");

                    // Initialize the Cosmos DB service
                    cosmosService = new CosmosDbService(endpoint, databaseName, containerName, key);
                    ctx.Status("Getting Container Status");

                    // Check if the container exists
                    containerExist = cosmosService.CheckCollectionExistsAsync().GetAwaiter().GetResult();

                    // Get the recipe count
                    if (containerExist)
                    {
                        AnsiConsole.MarkupLine($"Container exists, querying recipe Stats");
                        ctx.Status("Getting Recipe Stats");
                        recipeWithEmbedding = cosmosService.GetRecipeCountAsync(true).GetAwaiter().GetResult();
                        recipeWithNoEmbedding = cosmosService.GetRecipeCountAsync(false).GetAwaiter().GetResult();
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"Container not found. Create container using first option below.");
                        Console.WriteLine("");
                    }
                });

            if (containerExist)
            {
                AnsiConsole.MarkupLine($"We have [green]{recipeWithEmbedding}[/] vectorized recipe(s) and [red]{recipeWithNoEmbedding}[/] non vectorized recipe(s).");
                Console.WriteLine("");
            }

            return cosmosService;
        }

        /// <summary>
        /// Creates a Cosmos DB container with the specified configuration.
        /// </summary>
        /// <param name="config">The configuration containing the Cosmos DB settings.</param>
        private static void createCosmosContainer(IConfiguration config)
        {
            // Get the Cosmos DB service settings from the configuration
            string databaseName = config["CosmosDatabase"];
            string containerName = config["CosmosContainer"];
            bool isSucess = false;

            AnsiConsole.Status()
               .Start("Creating Cosmos container...", ctx =>
               {
                   // Create the Cosmos DB container
                   isSucess = cosmosService.CreateCosmosContainerAsync(databaseName, containerName).GetAwaiter().GetResult();
               });

            // Display the result of the operation
            if (isSucess)
                AnsiConsole.MarkupLine($"Cosmos Container '{containerName}' created with 1000 RU autoscale.");
            else
                AnsiConsole.MarkupLine($"Cosmos Container '{containerName}' create failed");
            Console.WriteLine("");
        }

        /// <summary>
        /// Retrieves all recipes from the Cosmos DB and displays them.
        /// </summary>
        /// <param name="config">The configuration containing the Cosmos DB settings.</param>
        private static void GetAllRecipes(IConfiguration config)
        {
            AnsiConsole.Status()
               .Start("Processing...", ctx =>
               {
                   ctx.Spinner(Spinner.Known.Star);
                   ctx.SpinnerStyle(Style.Parse("green"));

                   int recipeWithEmbedding = 0;
                   int recipeWithNoEmbedding = 0;

                   ctx.Status("Getting recipe(s)..");

                   // Get all recipes from the Cosmos DB
                   var Recipes = cosmosService.GetRecipesAsync().GetAwaiter().GetResult();

                   foreach (var recipe in Recipes)
                   {
                       //Remove embeddings to reduce numbers during display
                       if (recipe.vectors != null && recipe.vectors.Count > 10)
                       {
                           recipe.vectors.RemoveRange(2, recipe.vectors.Count - 5);
                       }

                       //Display the recipe
                       AnsiConsole.Write(new JsonText(JsonConvert.SerializeObject(recipe))
                            .BracesColor(Color.Red)
                            .BracketColor(Color.Green)
                            .ColonColor(Color.Blue)
                            .CommaColor(Color.Red)
                            .StringColor(Color.Green)
                            .NumberColor(Color.Blue)
                            .BooleanColor(Color.Red)
                            .NullColor(Color.Green));

                       //AnsiConsole.WriteLine(JsonConvert.SerializeObject(recipe));
                   }

                   // Get the recipe count
                   ctx.Status("Getting Updated Recipe Stats");
                   recipeWithEmbedding = cosmosService.GetRecipeCountAsync(true).GetAwaiter().GetResult();
                   recipeWithNoEmbedding = cosmosService.GetRecipeCountAsync(false).GetAwaiter().GetResult();

                   AnsiConsole.MarkupLine($"We have [teal]{recipeWithEmbedding}[/] vectorized recipe(s) and [red]{recipeWithNoEmbedding}[/] non vectorized recipe(s).");
                   Console.WriteLine("");
               });
        }

        /// <summary>
        /// Uploads recipes from the specified local folder to the Cosmos DB.
        /// </summary>
        /// <param name="config">The configuration containing the local folder path and Cosmos DB settings.</param>
        private static void UploadRecipes(IConfiguration config)
        {
            // Get the local folder path from the configuration
            string folder = config["RecipeLocalFolder"];
            int recipeWithEmbedding = 0;
            int recipeWithNoEmbedding = 0;

            List<Recipe> recipes = null;

            AnsiConsole.Status()
               .Start("Processing...", ctx =>
               {
                   ctx.Spinner(Spinner.Known.Star);
                   ctx.SpinnerStyle(Style.Parse("green"));

                   ctx.Status("Parsing Recipe files..");

                   //Parse documents
                   recipes = Utility.ParseDocuments(folder);

                   ctx.Status($"Uploading Recipe(s)..");

                   // Upload the recipes to the Cosmos DB
                   cosmosService.AddRecipesAsync(recipes).GetAwaiter().GetResult();

                   // Get the recipe count
                   ctx.Status("Getting Updated Recipe Stats");
                   recipeWithEmbedding = cosmosService.GetRecipeCountAsync(true).GetAwaiter().GetResult();
                   recipeWithNoEmbedding = cosmosService.GetRecipeCountAsync(false).GetAwaiter().GetResult();

               });

            AnsiConsole.MarkupLine($"Uploaded [green]{recipes.Count}[/] recipe(s).We have [teal]{recipeWithEmbedding}[/] vectorized recipe(s) and [red]{recipeWithNoEmbedding}[/] non vectorized recipe(s).");
            Console.WriteLine("");

        }

        /// <summary>
        /// Performs a search for recipes based on the user's query using vector search and OpenAI service.
        /// </summary>
        /// <param name="config">The configuration containing the OpenAI and Cosmos DB settings.</param>
        private static void PerformSearch(IConfiguration config)
        {
            Dictionary<string, float[]> dictEmbeddings = new Dictionary<string, float[]>();

            string chatCompletion = string.Empty;

            // Get the user query
            string userQuery = Console.Prompt(
                new TextPrompt<string>("Type the recipe name or your question, hit enter when ready.")
                    .PromptStyle("teal")
            );

            AnsiConsole.Status()
               .Start("Processing...", ctx =>
               {
                   ctx.Spinner(Spinner.Known.Star);
                   ctx.SpinnerStyle(Style.Parse("green"));

                   if (openAIEmbeddingService == null)
                   {
                       ctx.Status("Connecting to Open AI Service..");
                       //Initialize the Azure Open Ai service
                       openAIEmbeddingService = initOpenAIService(config);
                   }

                   ctx.Status("Converting User Query to Vector..");
                   //Get the embedding vector for the user query
                   var embeddingVector = openAIEmbeddingService.GetEmbeddingsAsync(userQuery).GetAwaiter().GetResult();

                   ctx.Status("Performing Vector Search.. Retriving recipe(s) from Cosmos DB (RAG pattern)..");
                   //Perform vector search to get the recipes
                   var retrivedDocs = cosmosService.SingleVectorSearch(embeddingVector, 0.60).GetAwaiter().GetResult();

                   ctx.Status($"Processing {retrivedDocs.Count} to generate Chat Response using OpenAI Service..");

                   string retrivedReceipeNames = string.Empty;

                   foreach (var recipe in retrivedDocs)
                   {
                       recipe.vectors = null; //removing embedding to reduce tokens during chat completion
                       retrivedReceipeNames += ", " + recipe.name; //to dispay recipes submitted for Completion
                   }

                   ctx.Status($"Processing '{retrivedReceipeNames}' to generate Completion using OpenAI Service..");
                   //Get the chat completion for the user query and retrived recipes
                   (string completion, int promptTokens, int completionTokens) = openAIEmbeddingService.GetChatCompletionAsync(userQuery, JsonConvert.SerializeObject(retrivedDocs)).GetAwaiter().GetResult();
                   chatCompletion = completion;
               });

            // Display the chat completion
            Console.WriteLine("");
            Console.Write(new Rule($"[silver]AI Assistant Response[/]") { Justification = Justify.Center });
            AnsiConsole.MarkupLine(chatCompletion);
            Console.WriteLine("");
            Console.WriteLine("");
            Console.Write(new Rule($"[yellow]****[/]") { Justification = Justify.Center });
            Console.WriteLine("");
        }

        /// <summary>
        /// Generates embeddings for recipes that do not have embeddings and updates them in the Cosmos DB.
        /// </summary>
        /// <param name="config">The configuration containing the OpenAI and Cosmos DB settings.</param>
        private static void GenerateEmbeddings(IConfiguration config)
        {
            Dictionary<string, float[]> dictEmbeddings = new Dictionary<string, float[]>();
            int recipeWithEmbedding = 0;
            int recipeWithNoEmbedding = 0;
            int recipeCount = 0;

            AnsiConsole.Status()
               .Start("Processing...", ctx =>
               {
                   ctx.Spinner(Spinner.Known.Star);
                   ctx.SpinnerStyle(Style.Parse("green"));

                   if (openAIEmbeddingService == null)
                   {
                       ctx.Status("Connecting to Open AI Service..");
                       //Initialize Azure Open Ai Service
                       openAIEmbeddingService = initOpenAIService(config);
                   }

                   ctx.Status("Getting recipe(s) to vectorize..");
                   // Get the recipes to vectorize
                   var Recipes = cosmosService.GetRecipesToVectorizeAsync().GetAwaiter().GetResult();

                   foreach (var recipe in Recipes)
                   {
                       recipeCount++;
                       ctx.Status($"Vectorizing Recipe# {recipeCount}..");

                       // Get the embedding vector for the recipe
                       var embeddingVector = openAIEmbeddingService.GetEmbeddingsAsync(JsonConvert.SerializeObject(recipe)).GetAwaiter().GetResult();
                       dictEmbeddings.Add(recipe.id, embeddingVector);
                   }

                   ctx.Status($"Updating {Recipes.Count} recipe(s) in Cosmos DB for vectors..");

                   // Update the recipes with the embeddings
                   cosmosService.UpdateRecipesAsync(dictEmbeddings).GetAwaiter().GetResult();

                   // Get the recipe count
                   ctx.Status("Getting Updated Recipe Stats");
                   recipeWithEmbedding = cosmosService.GetRecipeCountAsync(true).GetAwaiter().GetResult();
                   recipeWithNoEmbedding = cosmosService.GetRecipeCountAsync(false).GetAwaiter().GetResult();

               });
            // Display the result of the operation
            AnsiConsole.MarkupLine($"Vectorized [teal]{recipeCount}[/] recipe(s). We have [green]{recipeWithEmbedding}[/] vectorized recipe(s) and [red]{recipeWithNoEmbedding}[/] non vectorized recipe(s).");
            Console.WriteLine("");

        }



    }
}