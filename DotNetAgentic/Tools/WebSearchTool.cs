using DotNetAgentic.Tools.Interfaces;

namespace DotNetAgentic.Tools;

/// <summary>
/// Tool that performs web searches to retrieve information.
/// </summary>
public class WebSearchTool : ITool
{
    /// <inheritdoc />
    public string Name => "web_search";
    
    /// <inheritdoc />
    public string Description => "Searches the web for information";
    
    /// <inheritdoc />
    public async Task<string> ExecuteAsync(string input)
    {
        // Mock implementation for showcase
        await Task.Delay(100); // Simulate API call
        
        return $"Search results for '{input}':\n" +
               "1. AI agents are autonomous systems...\n" +
               "2. Agentic AI can solve complex tasks...";
    }
}