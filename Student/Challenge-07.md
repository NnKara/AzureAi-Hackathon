# Challenge 07 - Basic NL to SQL with semantic kernel.

 [< Previous Challenge](./Challenge-06.md) - **[Home](../README.md)** - [Next Challenge >](./Challenge-08.md)
 
## Introduction
The CTO is impressed with the results and the speed of implementation.

You have been assigned by the CTO to convert natural language queries into SQL statements. The objective is to precisely translate the user's intent into SQL queries that accurately retrieve the necessary data from the database.

## Description
In this challenge, you will practice converting natural language queries into SQL statements by using Semantic Kernel plugin. This exercise will help you understand how to translate user requests into precise SQL queries that can be executed on a database.

For this challenge you should add the Database Schema to Azure Open Ai context window.  You can find the database schema [here](./Resources/Challenge-07/dbschema.txt)

Semantic Kernel SDK supports a prompt templating language with some simple syntax rules. You don't need to write code or import any external libraries, just use the curly braces {{...}} to embed expressions in your prompts.

To create a semantic plugin, you need a folder containing two files: a skprompt.txt file and a config.json file. The skprompt.txt file contains the prompt to the large language model (LLM), similar to all the prompts you wrote so far. The config.json file contains the configuration details for the prompt.

The config.json file supports the following parameters:
- type: The type of prompt. You typically use the chat completion prompt type.
- description: A description of what the prompt does. This description can be used by the kernel to automatically invoke the prompt.
- input_variables: Defines the variables that are used inside of the prompt.
- execution_settings: The settings for completion models. For OpenAI models, these settings include the max_tokens and temperature properties.

To create this plugin, you would first create a 'Prompts' folder in your project, then a subfolder called 'BasicNLtoSQL.' Afterwards, you add the 'skprompt.txt' and 'config.json' files to your 'BasicNLtoSQL' folder.

Example of 'skprompt.txt' file:

```code
Given the SQL schema and a query in natural language, you have to format the query into a single valid SQL statement.

{{$sqlSchema}}

User Input
{{$input}}
```

The config.json file shall include the following configuration. Based on the  prompt(in skprompt.txt) there is a missing part in the following configuration which you may add.
``` json
{
  "schema": 1,
  "name": "BasicNLtoSQL",
  "description": "Natural Language to SQL",
  "type": "completion",
  "execution_settings": {
    "default": {
      "max_tokens": 1024,
      "temperature": 0,
      "top_p": 0,
      "presence_penalty": 0,
      "frequency_penalty": 0
    }
  },
  "input_variables": [
    {
      "name": "input",
      "description": "The question to answer",
      "required": true
    }
  ]
}
```

You should add your plugin by importing from prompt directory like
```csharp
kernel.ImportPluginFromPromptDirectory("Prompts");
```
You should invoke the plugin with the following code
```csharp
var result =  kernel.InvokeStreamingAsync("Prompts", "BasicNLtoSQL", new() { { "input", userInput },{ "sqlSchema", sqlSchema } } );
```

In the next challenge you will investigate alternative ways to optimize the solution. Discuss with your coach potential optimizations.

## Success Criteria
- Demonstrate that you have created the "Natural language to SQL" Semantic Kernel Prompt Plugin
- Demonstrate that you add the database schema in Azure Open AI Context window and you set meaningful instructions to the bot.
- Demonstrate that you can ask questions in natural language and you get responses with SQL queries.
- Discuss with your coach the disadvantages of current solution and propose ways to optimize this.

## Learning Resources
- [Create an Agent from a Semantic Kernel Template (Experimental) | Microsoft Learn](https://learn.microsoft.com/en-us/semantic-kernel/frameworks/agent/agent-templates?pivots=programming-language-csharp#agent-definition-from-a-prompt-template)
- [PromptTemplateConfig Class (Microsoft.SemanticKernel) | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/microsoft.semantickernel.prompttemplateconfig?view=semantic-kernel-dotnet)
- [Save prompts to files - Training | Microsoft Learn](https://learn.microsoft.com/en-us/training/modules/create-plugins-semantic-kernel/7-save-prompts-files)
