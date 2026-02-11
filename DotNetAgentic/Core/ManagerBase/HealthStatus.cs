namespace DotNetAgentic.Core.ManagerBase;

/// <summary>
/// Represents the health status of a manager.
/// </summary>
public class HealthStatus
{
    /// <summary>
    /// Gets or sets whether the manager is healthy.
    /// </summary>
    public bool IsHealthy { get; set; }
    
    /// <summary>
    /// Gets or sets the status message.
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets additional health details.
    /// </summary>
    public IDictionary<string, object> Details { get; set; } = new Dictionary<string, object>();
    
    /// <summary>
    /// Gets or sets the timestamp of the health check.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Creates a healthy status.
    /// </summary>
    public static HealthStatus Healthy(string message = "Manager is healthy")
    {
        return new HealthStatus
        {
            IsHealthy = true,
            Message = message
        };
    }
    
    /// <summary>
    /// Creates an unhealthy status.
    /// </summary>
    public static HealthStatus Unhealthy(string message, Exception? exception = null)
    {
        var status = new HealthStatus
        {
            IsHealthy = false,
            Message = message
        };
        
        if (exception != null)
        {
            status.Details["Exception"] = exception.Message;
            status.Details["StackTrace"] = exception.StackTrace ?? string.Empty;
        }
        
        return status;
    }
}

