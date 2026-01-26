namespace DotNetAgentic.Models;

/// <summary>
/// Used to represent a request sent to an agent, including the message and optional session ID.
/// </summary>
public class AgentRequest
{
    /// <summary>
    /// The message or prompt sent to the agent.
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// The optional session ID for maintaining context across multiple requests.
    /// </summary>
    public string? SessionId { get; set; }
}
