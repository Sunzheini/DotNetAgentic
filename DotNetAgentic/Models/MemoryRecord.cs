namespace DotNetAgentic.Models;

/// <summary>
/// The MemoryRecord class represents a record of an interaction between a user and the AI agent,
/// including the user's message, the agent's response, any tool calls made, and associated metadata.
/// This class is used for logging and tracking conversations.
/// </summary>
public class MemoryRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SessionId { get; set; } = string.Empty;
    public string UserMessage { get; set; } = string.Empty;
    public string AgentResponse { get; set; } = string.Empty;
    public List<ToolCall> ToolCalls { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Metadata { get; set; } = new();
}