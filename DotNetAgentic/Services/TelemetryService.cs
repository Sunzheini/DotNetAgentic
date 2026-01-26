using DotNetAgentic.Services.Interfaces;

namespace DotNetAgentic.Services;

public class TelemetryService : ITelemetryService
{
    private readonly List<TelemetryRecord> _logs = new();
    private readonly object _lock = new();
    
    public Task LogRequestAsync(string endpoint, string sessionId, string operation, string input,
        string output, long durationMs, string? error = null, Dictionary<string, object>? metadata = null)
    {
        var record = new TelemetryRecord
        {
            Endpoint = endpoint,
            SessionId = sessionId,
            Operation = operation,
            Input = input.Length > 500 ? input.Substring(0, 500) + "..." : input,
            Output = output.Length > 500 ? output.Substring(0, 500) + "..." : output,
            DurationMs = durationMs,
            Error = error,
            Metadata = metadata ?? new Dictionary<string, object>(),
            Timestamp = DateTime.UtcNow
        };
        
        lock (_lock)
        {
            _logs.Add(record);
            // Keep only last 1000 records
            if (_logs.Count > 1000)
            {
                _logs.RemoveRange(0, _logs.Count - 1000);
            }
        }
        
        Console.WriteLine($"[Telemetry] {operation} took {durationMs}ms - Session: {sessionId}");
        
        return Task.CompletedTask;
    }
    
    public Task<List<TelemetryRecord>> GetRecentLogsAsync(int count = 50)
    {
        lock (_lock)
        {
            return Task.FromResult(_logs
                .OrderByDescending(l => l.Timestamp)
                .Take(count)
                .ToList());
        }
    }
    
    public Task<List<TelemetryRecord>> GetLogsBySessionAsync(string sessionId)
    {
        lock (_lock)
        {
            return Task.FromResult(_logs
                .Where(l => l.SessionId == sessionId)
                .OrderByDescending(l => l.Timestamp)
                .ToList());
        }
    }
    
    public Task ClearLogsAsync()
    {
        lock (_lock)
        {
            _logs.Clear();
        }
        return Task.CompletedTask;
    }
}