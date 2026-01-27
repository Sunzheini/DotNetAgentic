namespace DotNetAgentic.Services;

/// <summary>
/// The metrics data for the AI agent.
/// </summary>
public class AgentMetrics
{
    public int TotalRequests { get; set; }
    public int SuccessfulRequests { get; set; }
    public int FailedRequests { get; set; }
    public long AverageResponseTimeMs { get; set; }
    public Dictionary<string, int> ToolUsage { get; set; } = new();
    public Dictionary<string, int> EndpointUsage { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}