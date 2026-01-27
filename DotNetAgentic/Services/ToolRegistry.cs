using DotNetAgentic.Tools;
using DotNetAgentic.Tools.Interfaces;

namespace DotNetAgentic.Services;

/// <summary>
/// Tool registry that manages available tools for the AI agent.
/// </summary>
public class ToolRegistry
{
    /// <summary>
    /// The dictionary of registered tools.
    /// </summary>
    private readonly Dictionary<string, ITool> _tools = new();
    
    public ToolRegistry()
    {
        var tavilyApiKey = Environment.GetEnvironmentVariable("TAVILY_API_KEY");
        
        RegisterTool(new CalculatorTool());
        RegisterTool(new WebSearchTool());
        
        if (!string.IsNullOrEmpty(tavilyApiKey))
        {
            RegisterTool(new TavilySearchTool(tavilyApiKey));
        }
    }
    
    /// <summary>
    /// Registers a new tool in the registry.
    /// </summary>
    /// <param name="tool">
    /// The tool to register.
    /// </param>
    public void RegisterTool(ITool tool)
    {
        _tools[tool.Name] = tool;
    }
    
    /// <summary>
    /// Whether the registry has a tool by the given name.
    /// </summary>
    /// <param name="toolName">
    /// The name of the tool.
    /// </param>
    /// <returns>
    /// True if the tool exists, false otherwise.
    /// </returns>
    public bool HasTool(string toolName) => _tools.ContainsKey(toolName);
    
    /// <summary>
    /// Executes a tool by name with the given input.
    /// </summary>
    /// <param name="toolName">
    /// The name of the tool to execute.
    /// </param>
    /// <param name="input">
    /// Input to the tool.
    /// </param>
    /// <returns>
    /// Result of the tool execution.
    /// </returns>
    public async Task<string> ExecuteToolAsync(string toolName, string input)
    {
        if (_tools.TryGetValue(toolName, out var tool))
        {
            return await tool.ExecuteAsync(input);
        }
        return $"Error: Tool '{toolName}' not found";
    }
    
    /// <summary>
    /// Gets all registered tools.
    /// </summary>
    /// <returns>
    /// Returns a list of all registered tools.
    /// </returns>
    public List<ITool> GetAllTools() => _tools.Values.ToList();
    
    /// <summary>
    /// Gets a description of all registered tools.
    /// </summary>
    /// <returns>
    /// Returns a formatted string listing all tools and their descriptions.
    /// </returns>
    public string GetToolsDescription()
    {
        return string.Join("\n", _tools.Values.Select(t => 
            $"{t.Name}: {t.Description}"));
    }
}