namespace DotNetAgentic.Services;

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

public interface IMetricsService
{
    Task RecordRequestAsync(string endpoint, string toolName, long durationMs, bool success);
    Task<AgentMetrics> GetMetricsAsync();
    Task ResetMetricsAsync();
}

public class MetricsService : IMetricsService
{
    private readonly List<long> _responseTimes = new();
    private readonly Dictionary<string, int> _toolUsage = new();
    private readonly Dictionary<string, int> _endpointUsage = new();
    private int _totalRequests = 0;
    private int _successfulRequests = 0;
    private int _failedRequests = 0;
    private readonly object _lock = new();
    
    public Task RecordRequestAsync(string endpoint, string toolName, long durationMs, bool success)
    {
        lock (_lock)
        {
            _totalRequests++;
            
            if (success)
                _successfulRequests++;
            else
                _failedRequests++;
            
            _responseTimes.Add(durationMs);
            
            if (!string.IsNullOrEmpty(toolName))
            {
                _toolUsage[toolName] = _toolUsage.GetValueOrDefault(toolName) + 1;
            }
            
            _endpointUsage[endpoint] = _endpointUsage.GetValueOrDefault(endpoint) + 1;
            
            // Keep only last 1000 response times
            if (_responseTimes.Count > 1000)
            {
                _responseTimes.RemoveRange(0, _responseTimes.Count - 1000);
            }
        }
        
        return Task.CompletedTask;
    }
    
    public Task<AgentMetrics> GetMetricsAsync()
    {
        lock (_lock)
        {
            var metrics = new AgentMetrics
            {
                TotalRequests = _totalRequests,
                SuccessfulRequests = _successfulRequests,
                FailedRequests = _failedRequests,
                AverageResponseTimeMs = _responseTimes.Any() ? (long)_responseTimes.Average() : 0,
                ToolUsage = new Dictionary<string, int>(_toolUsage),
                EndpointUsage = new Dictionary<string, int>(_endpointUsage),
                LastUpdated = DateTime.UtcNow
            };
            
            return Task.FromResult(metrics);
        }
    }
    
    public Task ResetMetricsAsync()
    {
        lock (_lock)
        {
            _totalRequests = 0;
            _successfulRequests = 0;
            _failedRequests = 0;
            _responseTimes.Clear();
            _toolUsage.Clear();
            _endpointUsage.Clear();
        }
        
        return Task.CompletedTask;
    }
}