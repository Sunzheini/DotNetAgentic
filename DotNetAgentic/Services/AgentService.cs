using DotNetAgentic.Tools;
using Microsoft.SemanticKernel;

namespace DotNetAgentic.Services;

/// <summary>
/// Represents an AI agent service that processes user input using Semantic Kernel.
/// </summary>
public class AgentService : IAgentService
{
    private readonly Kernel _kernel;
    private readonly ToolRegistry _toolRegistry;
    
    public AgentService(ToolRegistry toolRegistry)
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
        
        _toolRegistry = toolRegistry;
    }
    
    public async Task<string> ProcessAsync(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return "Error: Input cannot be empty";
        }
        
        // Check if input is a tool command
        if (input.StartsWith("/tool "))
        {
            var parts = input[6..].Split(' ', 2);
            if (parts.Length == 2)
            {
                return await _toolRegistry.ExecuteToolAsync(parts[0], parts[1]);
            }
        }
        
        // Enhanced prompt with tool awareness
        var toolsDescription = _toolRegistry.GetToolsDescription();
        var prompt = $"""
                      You are an AI assistant with access to these tools:
                      {toolsDescription}

                      User: {input}

                      If you need to use a tool, respond with: TOOL:tool_name:input
                      Otherwise, provide a helpful response.
                      """;
        
        var result = await _kernel.InvokePromptAsync(prompt);
        return ParseToolResponse(result.ToString());
    }
    
    private string ParseToolResponse(string response)
    {
        if (response.StartsWith("TOOL:"))
        {
            var parts = response[5..].Split(':', 2);
            if (parts.Length == 2)
            {
                var toolResult = _toolRegistry.ExecuteToolAsync(parts[0], parts[1]).Result;
                return $"Used {parts[0]} tool. Result: {toolResult}";
            }
        }
        return response;
    }
    
    public List<ITool> GetAvailableTools() => _toolRegistry.GetAllTools();
}