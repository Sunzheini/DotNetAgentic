namespace DotNetAgentic.Services;

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

public interface ITelemetryService
{
    Task LogRequestAsync(string endpoint, string sessionId, string operation, string input, 
        string output, long durationMs, string? error = null, Dictionary<string, object>? metadata = null);
    
    Task<List<TelemetryRecord>> GetRecentLogsAsync(int count = 50);
    Task<List<TelemetryRecord>> GetLogsBySessionAsync(string sessionId);
    Task ClearLogsAsync();
}

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