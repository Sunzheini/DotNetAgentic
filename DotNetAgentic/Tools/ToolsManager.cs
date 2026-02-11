using DotNetAgentic.Core.ManagerBase;
using DotNetAgentic.Services;
using DotNetAgentic.Tools.Interfaces;

namespace DotNetAgentic.Tools;

/// <summary>
/// Manager responsible for tool registration, discovery, and execution.
/// </summary>
public class ToolsManager : ManagerBase<ToolRegistry>
{
    public override ManagerType ManagerType => ManagerType.ToolManager;

    public ToolsManager(ToolRegistry toolRegistry) : base(toolRegistry)
    {
    }

    /// <summary>
    /// Registers a new tool.
    /// </summary>
    public void RegisterTool(ITool tool)
    {
        Registry.RegisterTool(tool);
        IncrementMetric("TotalToolsRegistered");
    }

    /// <summary>
    /// Executes a tool by name with the given input.
    /// </summary>
    public async Task<string> ExecuteToolAsync(string toolName, string input)
    {
        EnsureInitialized();
        IncrementMetric("TotalToolExecutions");
        
        var result = await Registry.ExecuteToolAsync(toolName, input);
        
        if (!result.StartsWith("Error:"))
        {
            IncrementMetric("SuccessfulExecutions");
        }
        else
        {
            IncrementMetric("FailedExecutions");
        }
        
        return result;
    }

    /// <summary>
    /// Checks if a tool exists.
    /// </summary>
    public bool HasTool(string toolName)
    {
        return Registry.HasTool(toolName);
    }

    /// <summary>
    /// Gets all registered tools.
    /// </summary>
    public List<ITool> GetAllTools()
    {
        return Registry.GetAllTools();
    }

    /// <summary>
    /// Gets a description of all registered tools.
    /// </summary>
    public string GetToolsDescription()
    {
        return Registry.GetToolsDescription();
    }

    protected override Task OnInitializeAsync()
    {
        // Auto-initialize with current tool count
        UpdateMetric("TotalToolsRegistered", Registry.GetAllTools().Count);
        UpdateMetric("SuccessfulExecutions", 0L);
        UpdateMetric("FailedExecutions", 0L);
        UpdateMetric("TotalToolExecutions", 0L);
        return Task.CompletedTask;
    }

    protected override Task<HealthStatus?> OnCheckHealthAsync()
    {
        var toolCount = Registry.GetAllTools().Count;
        
        if (toolCount == 0)
        {
            return Task.FromResult<HealthStatus?>(
                HealthStatus.Unhealthy("No tools registered"));
        }
        
        var status = HealthStatus.Healthy($"ToolsManager operational with {toolCount} tools");
        status.Details["ToolCount"] = toolCount;
        status.Details["AvailableTools"] = string.Join(", ", Registry.GetAllTools().Select(t => t.Name));
        
        return Task.FromResult<HealthStatus?>(status);
    }
}