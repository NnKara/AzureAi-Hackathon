# Challenge 02 - Use prompt flow to query on own data with Search AI.

 [< Previous Challenge](./Challenge-01.md) - **[Home](../README.md)** - [Next Challenge >](./Challenge-03.md)
 
## Introduction

Once you've finished the initial setup in the chat playground, it's time to explore deploying a real-time endpoint. You discover that using Prompt flow can help you accomplish this.

## Description

You figure out that Prompt flow is a tool that simplifies the entire development cycle of AI applications using Large Language Models (LLMs). It streamlines prototyping, experimenting, iterating, and deploying your AI applications.

By utilizing prompt flow, you're able to:
- Create executable flows that link LLMs, prompts, and Python tools through a visualized graph.
- Debug, share, and iterate your flows with ease through team collaboration.
- Create prompt variants and evaluate their performance through large-scale testing.
- Deploy a real-time endpoint that unlocks the full power of LLMs for your application.

You will use generative AI and prompt flow UI to build, configure, and deploy a copilot.

The copilot should answer questions about your products and services. For example, the copilot can answer questions such as "How much do the TrailWalker hiking shoes cost?"

You should complete the following steps
- Create a prompt flow from the playground.
- Customize prompt flow and ground your data created in previous challenge.
- Evaluate the flow using a question and answer evaluation dataset.
- Deploy the flow for consumption.
- Evaluate the deployed flow with your prefered metrics

> [!NOTE]
> Create the prompt flow from the playground to avoid creating the flow from scratch. By doing this, the playground setup will be exported to prompt flow.

You can find the sample product information data used in previous challenge [here](./Resources/Challenge-01/Data/product-info)

## Success Criteria
- Demonstrate that you can chat within prompt flow with product info. Get answers to questions such as "How much are the TrailWalker hiking shoes?"
- Evaluate the flow using a question and answer evaluation dataset (here is the [eval_dataset](https://github.com/Azure-Samples/rag-data-openai-python-promptflow/blob/main/src/evaluation/evaluation_dataset.jsonl))
- Show the evaluation status and results 
- Demonstrate that you deploy the flow and you can use the REST endpoint or the SDK to use the deployed flow.
- The prompt flow steps were automatically generated when the prompt flow was created from Chat playground. Describe the prompt flow steps to your coach.

## Learning Resources

### Prompt flow
- [Prompt flow in Azure AI Studio - Azure AI Studio | Microsoft Learn](https://learn.microsoft.com/en-us/azure/ai-studio/how-to/prompt-flow)
- [How to build with prompt flow - Azure AI Studio | Microsoft Learn](https://learn.microsoft.com/en-us/azure/ai-studio/how-to/flow-develop)
- [Work with Azure AI Studio projects in VS Code - Azure AI Studio | Microsoft Learn](https://learn.microsoft.com/en-us/azure/ai-studio/how-to/develop/vscode)
- [Deploy a flow as a managed online endpoint for real-time inference - Azure AI Studio | Microsoft Learn](https://learn.microsoft.com/en-us/azure/ai-studio/how-to/flow-deploy)
### Evaluate models in Azure AI Studio
- [Evaluation of generative AI applications - Azure AI Studio | Microsoft Learn](https://learn.microsoft.com/en-us/azure/ai-studio/concepts/evaluation-approach-gen-ai)
- [How to evaluate generative AI apps with Azure AI Studio - Azure AI Studio | Microsoft Learn](https://learn.microsoft.com/en-us/azure/ai-studio/how-to/evaluate-generative-ai-app)
- [How to view evaluation results in Azure AI Studio - Azure AI Studio | Microsoft Learn](https://learn.microsoft.com/en-us/azure/ai-studio/how-to/evaluate-flow-results)
- [Monitoring evaluation metrics descriptions and use cases](https://learn.microsoft.com/en-us/azure/machine-learning/prompt-flow/concept-model-monitoring-generative-ai-evaluation-metrics?view=azureml-api-2)