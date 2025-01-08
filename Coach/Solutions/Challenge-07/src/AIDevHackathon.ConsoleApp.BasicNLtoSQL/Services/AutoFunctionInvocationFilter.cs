using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIDevHackathon.ConsoleApp.BasicNLtoSQL.Services
{
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    public class AutoFunctionInvocationFilter(ILogger logger) : IAutoFunctionInvocationFilter
    {
        private readonly ILogger _logger = logger;

        public async Task OnAutoFunctionInvocationAsync(AutoFunctionInvocationContext context, Func<AutoFunctionInvocationContext, Task> next)
        {
            // Example: get function information
            var functionName = context.Function.Name;

            // Example: get chat history
            var chatHistory = context.ChatHistory;

            // Example: get information about all functions which will be invoked
            var functionCalls = FunctionCallContent.GetFunctionCalls(context.ChatHistory.Last());

            // Example: get request sequence index
            this._logger.LogDebug("Request sequence index: {RequestSequenceIndex}", context.RequestSequenceIndex);

            // Example: get function sequence index
            this._logger.LogDebug("Function sequence index: {FunctionSequenceIndex}", context.FunctionSequenceIndex);

            // Example: get total number of functions which will be called
            this._logger.LogDebug("Total number of functions: {FunctionCount}", context.FunctionCount);

            // Calling next filter in pipeline or function itself.
            // By skipping this call, next filters and function won't be invoked, and function call loop will proceed to the next function.
            await next(context);

            // Example: get function result
            var result = context.Result;

            // Example: override function result value
            context.Result = new FunctionResult(context.Result, "Result from auto function invocation filter");

            // Example: Terminate function invocation
            context.Terminate = true;
        }
    }
}
