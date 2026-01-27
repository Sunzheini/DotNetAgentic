namespace DotNetAgentic.Services.Interfaces;

/// <summary>
/// Telemetry service interface for logging and retrieving telemetry data.
/// </summary>
public interface ITelemetryService
{
    /// <summary>
    /// Logs a request with relevant telemetry data.
    /// </summary>
    /// <param name="endpoint">
    /// The API endpoint being accessed.
    /// </param>
    /// <param name="sessionId">
    /// Session identifier for correlating requests.
    /// </param>
    /// <param name="operation">
    /// Operation name or type being performed.
    /// </param>
    /// <param name="input">
    /// Input data for the operation.
    /// </param>
    /// <param name="output">
    /// Output data from the operation.
    /// </param>
    /// <param name="durationMs">
    /// Duration of the operation in milliseconds.
    /// </param>
    /// <param name="error">
    /// Error message if the operation failed.
    /// </param>
    /// <param name="metadata">
    /// Transitional metadata associated with the request.
    /// </param>
    /// <returns>
    /// Returns a task that represents the asynchronous logging operation.
    /// </returns>
    Task LogRequestAsync(string endpoint, string sessionId, string operation, string input, 
        string output, long durationMs, string? error = null, Dictionary<string, object>? metadata = null);
    
    /// <summary>
    /// Gets recent telemetry logs.
    /// </summary>
    /// <param name="count">
    /// Component of logs to retrieve.
    /// </param>
    /// <returns>
    /// Returns a task that represents the asynchronous retrieval operation,
    /// </returns>
    Task<List<TelemetryRecord>> GetRecentLogsAsync(int count = 50);
    
    /// <summary>
    /// Gets telemetry logs for a specific session.
    /// </summary>
    /// <param name="sessionId">
    /// Session identifier to filter logs.
    /// </param>
    /// <returns>
    /// Returns a task that represents the asynchronous retrieval operation,
    /// </returns>
    Task<List<TelemetryRecord>> GetLogsBySessionAsync(string sessionId);
    
    /// <summary>
    /// Clears all telemetry logs.
    /// </summary>
    /// <returns>
    /// Returns a task that represents the asynchronous clear operation.
    /// </returns>
    Task ClearLogsAsync();
}