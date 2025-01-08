using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIDevHackathon.ConsoleApp.BasicNLtoSQL.Services
{
    public class PromptRenderFilter : IPromptRenderFilter
    {
        public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
        {
            // Example: get function information
            var functionName = context.Function.Name;

            await next(context);

            // Example: override rendered prompt before sending it to AI
            //context.RenderedPrompt = "Safe prompt";
        }
    }
}
