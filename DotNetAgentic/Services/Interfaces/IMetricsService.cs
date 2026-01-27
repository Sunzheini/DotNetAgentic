namespace DotNetAgentic.Services.Interfaces;

/// <summary>
/// The metrics service interface for tracking and retrieving agent metrics.
/// </summary>
public interface IMetricsService
{
    /// <summary>
    /// Tracks a request made to the agent.
    /// </summary>
    /// <param name="endpoint">
    /// The API endpoint being called.
    /// </param>
    /// <param name="toolName">
    /// The name of the tool being used, if any.
    /// </param>
    /// <param name="durationMs">
    /// The duration of the request in milliseconds.
    /// </param>
    /// <param name="success">
    /// True if the request was successful, false otherwise.
    /// </param>
    /// <returns>
    /// Returns a task that represents the asynchronous operation.
    /// </returns>
    Task RecordRequestAsync(string endpoint, string toolName, long durationMs, bool success);
    
    /// <summary>
    /// Gets the current metrics for the agent.
    /// </summary>
    /// <returns>
    /// Returns a task that represents the asynchronous operation.
    /// </returns>
    Task<AgentMetrics> GetMetricsAsync();
    
    /// <summary>
    /// Resets all collected metrics.
    /// </summary>
    /// <returns>
    /// Returns a task that represents the asynchronous operation.
    /// </returns>
    Task ResetMetricsAsync();
}