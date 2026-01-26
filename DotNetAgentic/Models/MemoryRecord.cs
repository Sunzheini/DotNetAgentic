namespace DotNetAgentic.Models;

/// <summary>
/// The MemoryRecord class represents a record of an interaction between a user and the AI agent,
/// including the user's message, the agent's response, any tool calls made, and associated metadata.
/// This class is used for logging and tracking conversations.
/// </summary>
public class MemoryRecord
{
    /// <summary>
    /// The unique identifier for the memory record.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// The session identifier to which this memory record belongs.
    /// </summary>
    public string SessionId { get; set; } = string.Empty;
    
    /// <summary>
    /// The user's message that initiated the interaction.
    /// </summary>
    public string UserMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// The AI agent's response to the user's message.
    /// </summary>
    public string AgentResponse { get; set; } = string.Empty;
    
    /// <summary>
    /// A list of tool calls made by the agent during the interaction.
    /// </summary>
    public List<ToolCall> ToolCalls { get; set; } = new();
    
    /// <summary>
    /// The timestamp when the memory record was created.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// The metadata associated with the memory record.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}