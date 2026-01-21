using DotNetAgentic.Tools;

namespace DotNetAgentic.Services;

/// <summary>
/// Tool registry that manages available tools for the AI agent.
/// </summary>
public class ToolRegistry
{
    private readonly Dictionary<string, ITool> _tools = new();
    
    public ToolRegistry()
    {
        RegisterTool(new CalculatorTool());
        RegisterTool(new WebSearchTool());
    }
    
    public void RegisterTool(ITool tool)
    {
        _tools[tool.Name] = tool;
    }
    
    public bool HasTool(string toolName) => _tools.ContainsKey(toolName);
    
    public async Task<string> ExecuteToolAsync(string toolName, string input)
    {
        if (_tools.TryGetValue(toolName, out var tool))
        {
            return await tool.ExecuteAsync(input);
        }
        return $"Error: Tool '{toolName}' not found";
    }
    
    public List<ITool> GetAllTools() => _tools.Values.ToList();
    
    public string GetToolsDescription()
    {
        return string.Join("\n", _tools.Values.Select(t => 
            $"{t.Name}: {t.Description}"));
    }
}