namespace DotNetAgentic.Services;

/// <summary>
/// The metrics data for the AI agent.
/// </summary>
public class AgentMetrics
{
    /// <summary>
    /// Total number of requests processed by the agent.
    /// </summary>
    public int TotalRequests { get; set; }
    
    /// <summary>
    /// Successful requests count.
    /// </summary>
    public int SuccessfulRequests { get; set; }
    
    /// <summary>
    /// Failed requests count.
    /// </summary>
    public int FailedRequests { get; set; }
    
    /// <summary>
    /// Average response time in milliseconds.
    /// </summary>
    public long AverageResponseTimeMs { get; set; }
    
    /// <summary>
    /// Dictionary tracking tool usage counts.
    /// </summary>
    public Dictionary<string, int> ToolUsage { get; set; } = new();
    
    /// <summary>
    /// Dictionary tracking endpoint usage counts.
    /// </summary>
    public Dictionary<string, int> EndpointUsage { get; set; } = new();
    
    /// <summary>
    /// Timestamp of the last metrics update.
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}