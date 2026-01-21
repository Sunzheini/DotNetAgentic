namespace DotNetAgentic.Models;

/// <summary>
/// Tool execution record for tracking tool usage by the agent.
/// </summary>
public class ToolExecution
{
    public string ToolName { get; set; } = string.Empty;
    public string Input { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Agent context that maintains session information, tool history, and conversation history.
/// </summary>
public class AgentContext
{
    public string SessionId { get; set; } = string.Empty;
    public List<ToolExecution> ToolHistory { get; set; } = new();
    public string ConversationHistory { get; set; } = string.Empty;
}