namespace DotNetAgentic.Services.Interfaces;

/// <summary>
/// Telemetry service interface for logging and retrieving telemetry data.
/// </summary>
public interface ITelemetryService
{
    Task LogRequestAsync(string endpoint, string sessionId, string operation, string input, 
        string output, long durationMs, string? error = null, Dictionary<string, object>? metadata = null);
    
    Task<List<TelemetryRecord>> GetRecentLogsAsync(int count = 50);
    Task<List<TelemetryRecord>> GetLogsBySessionAsync(string sessionId);
    Task ClearLogsAsync();
}