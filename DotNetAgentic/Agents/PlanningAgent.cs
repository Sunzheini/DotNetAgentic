using Microsoft.SemanticKernel;

namespace DotNetAgentic.Agents;

/// <summary>
/// Planning agent that breaks down complex tasks into steps.
/// </summary>
public class PlanningAgent
{
    private readonly Kernel _kernel;
    
    public PlanningAgent()
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new ArgumentException("OPENAI_API_KEY environment variable is not set");
        }
        
        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddOpenAIChatCompletion(
            modelId: "gpt-3.5-turbo",
            apiKey: apiKey);
        
        _kernel = kernelBuilder.Build();
    }
    
    public async Task<string> CreatePlanAsync(string goal)
    {
        var prompt = $"""
                      You are a planning AI. Break this goal into steps:
                      GOAL: {goal}

                      Create a step-by-step plan. Format:
                      1. [Step 1]
                      2. [Step 2]
                      3. [Step 3]
                      """;
        
        var result = await _kernel.InvokePromptAsync(prompt);
        return result.ToString();
    }
}