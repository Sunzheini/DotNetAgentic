namespace DotNetAgentic.Models;


public class AgentRequest
{
    public string Message { get; set; } = string.Empty;
    public string? SessionId { get; set; }
}
