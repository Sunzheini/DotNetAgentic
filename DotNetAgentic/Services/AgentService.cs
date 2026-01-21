using Microsoft.SemanticKernel;

namespace DotNetAgentic.Services;

/// <summary>
/// Represents an AI agent service that processes user input using Semantic Kernel.
/// </summary>
public class AgentService : IAgentService
{
    private readonly Kernel _kernel;
    
    public AgentService()
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        const string openAiModel = "gpt-4.1-mini";
    
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new ArgumentException("OPENAI_API_KEY environment variable is not set");
        }
        
        // Initialize Semantic Kernel with OpenAI chat completion
        var kernelBuilder = Kernel.CreateBuilder();
        
        // Note: Replace
        kernelBuilder.AddOpenAIChatCompletion(
            modelId: openAiModel,
            apiKey: apiKey);
        
        _kernel = kernelBuilder.Build();
    }
    
    public async Task<string> ProcessAsync(string input)
    {
        // Simple AI agent
        var prompt = $"You are a helpful AI assistant. User says: {input}";
        var result = await _kernel.InvokePromptAsync(prompt);
        return result.ToString();
    }
}