namespace DotNetAgentic.Models;


/// <summary>
/// Used to represent the response from an agent, including content, tool calls, and reasoning.
/// </summary>
public class AgentResponse
{
    public string Content { get; set; } = string.Empty;
    public List<ToolCall> ToolCalls { get; set; } = new();
    public string? Reasoning { get; set; }
}