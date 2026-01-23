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
    private readonly IMemoryStore _memoryStore;
    
    public AgentService(ToolRegistry toolRegistry, IMemoryStore memoryStore)
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
        _memoryStore = memoryStore;
    }
    
    public async Task<string> ProcessAsync(string input, string sessionId = "default")
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return "Error: Input cannot be empty";
        }
        
        // Get conversation history from memory
        var history = await _memoryStore.GetSessionSummaryAsync(sessionId);
        
        // Check if input is a tool command
        if (input.StartsWith("/tool "))
        {
            var parts = input[6..].Split(' ', 2);
            if (parts.Length == 2)
            {
                var toolResult = await _toolRegistry.ExecuteToolAsync(parts[0], parts[1]);
                
                // Save tool interaction to memory
                await _memoryStore.SaveAsync(new Models.MemoryRecord
                {
                    SessionId = sessionId,
                    UserMessage = input,
                    AgentResponse = toolResult,
                    ToolCalls = new List<Models.ToolCall>
                    {
                        new() { ToolName = parts[0], Input = parts[1], Output = toolResult }
                    }
                });
                
                return toolResult;
            }
        }
        
        // Enhanced prompt with tool awareness AND conversation history
        var toolsDescription = _toolRegistry.GetToolsDescription();
        var prompt = $"""
                      You are an AI assistant with access to these tools:
                      {toolsDescription}

                      {history}

                      User: {input}

                      If you need to use a tool, respond with: TOOL:tool_name:input
                      Otherwise, provide a helpful response.
                      """;
        
        var result = await _kernel.InvokePromptAsync(prompt);
        var response = ParseToolResponse(result.ToString());
        
        // Save conversation to memory
        await _memoryStore.SaveAsync(new Models.MemoryRecord
        {
            SessionId = sessionId,
            UserMessage = input,
            AgentResponse = response,
            ToolCalls = new List<Models.ToolCall>()
        });
        
        return response;
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
    
    public async Task<string> ProcessWithMemoryAsync(string input, string sessionId)
    {
        // This method is now the same as ProcessAsync since we integrated memory there
        return await ProcessAsync(input, sessionId);
    }
    
    public List<ITool> GetAvailableTools() => _toolRegistry.GetAllTools();
}