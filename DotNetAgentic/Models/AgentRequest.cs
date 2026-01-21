namespace DotNetAgentic.Models;

/// <summary>
/// Used to represent a request sent to an agent, including the message and optional session ID.
/// </summary>
public class AgentRequest
{
    public string Message { get; set; } = string.Empty;
    public string? SessionId { get; set; }
}
