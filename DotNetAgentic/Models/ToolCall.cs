namespace DotNetAgentic.Models;

/// <summary>
/// Used to represent a call to an external tool by the agent.
/// </summary>
public class ToolCall
{
    /// <summary>
    /// The name of the tool being called.
    /// </summary>
    public string ToolName { get; set; } = string.Empty;
    
    /// <summary>
    /// The input provided to the tool.
    /// </summary>
    public string Input { get; set; } = string.Empty;
    
    /// <summary>
    /// The output returned by the tool.
    /// </summary>
    public string Output { get; set; } = string.Empty;
}
