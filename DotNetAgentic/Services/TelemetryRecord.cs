namespace DotNetAgentic.Services;

/// <summary>
/// A telemetry record representing a logged request.
/// </summary>
public class TelemetryRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Endpoint { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string Input { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public long DurationMs { get; set; }
    public string? Error { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}