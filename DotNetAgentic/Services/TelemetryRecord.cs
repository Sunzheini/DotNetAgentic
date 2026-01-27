namespace DotNetAgentic.Services;

/// <summary>
/// A telemetry record representing a logged request.
/// </summary>
public class TelemetryRecord
{
    /// <summary>
    /// The unique identifier for the telemetry record.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Endpoint that was called.
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;
    
    /// <summary>
    /// Session identifier associated with the request.
    /// </summary>
    public string SessionId { get; set; } = string.Empty;
    
    /// <summary>
    /// Operation performed (e.g., "Chat", "ToolExecution").
    /// </summary>
    public string Operation { get; set; } = string.Empty;
    
    /// <summary>
    /// Input provided to the operation.
    /// </summary>
    public string Input { get; set; } = string.Empty;
    
    /// <summary>
    /// Output returned from the operation.
    /// </summary>
    public string Output { get; set; } = string.Empty;
    
    /// <summary>
    /// Duration of the operation in milliseconds.
    /// </summary>
    public long DurationMs { get; set; }
    
    /// <summary>
    /// Error message if the operation failed.
    /// </summary>
    public string? Error { get; set; }
    
    /// <summary>
    /// Metadata associated with the telemetry record.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
    
    /// <summary>
    /// Timestamp when the record was created.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}