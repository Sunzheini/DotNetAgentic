namespace DotNetAgentic.Models;


/// <summary>
/// Used to represent the response from an agent, including content, tool calls, and reasoning.
/// </summary>
public class AgentResponse
{
    /// <summary>
    /// The main content of the agent's response.
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// The list of tool calls made by the agent during its response.
    /// </summary>
    public List<ToolCall> ToolCalls { get; set; } = new();
    
    /// <summary>
    /// The reasoning behind the agent's response.
    /// </summary>
    public string? Reasoning { get; set; }
}