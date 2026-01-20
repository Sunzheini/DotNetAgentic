namespace DotNetAgentic.Models;


/// <summary>
/// Used to represent a call to an external tool by the agent.
/// </summary>
public class ToolCall
{
    public string ToolName { get; set; } = string.Empty;
    public string Input { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
}
