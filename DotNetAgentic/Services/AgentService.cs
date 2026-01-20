using Microsoft.SemanticKernel;

namespace DotNetAgentic.Services;

/// <summary>
/// Represents an AI agent service that processes user input using Semantic Kernel.
/// </summary>
public class AgentService
{
    private readonly Kernel _kernel;
    
    public AgentService()
    {
        // Initialize Semantic Kernel with OpenAI chat completion
        var kernelBuilder = Kernel.CreateBuilder();
        
        // Note: Replace
        kernelBuilder.AddOpenAIChatCompletion(
            modelId: "gpt-3.5-turbo",
            apiKey: "your-api-key-here");
        
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