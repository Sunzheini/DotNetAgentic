using DotNetAgentic.Services.Interfaces;

namespace DotNetAgentic.Services;

/// <summary>
/// Implements a metrics service for collecting and reporting agent performance metrics.
/// </summary>
public class MetricsService : IMetricsService
{
    /// <summary>
    /// The list of recorded response times in milliseconds.
    /// </summary>
    private readonly List<long> _responseTimes = new();
    
    /// <summary>
    /// Tool usage counts.
    /// </summary>
    private readonly Dictionary<string, int> _toolUsage = new();
    
    /// <summary>
    /// The endpoint usage counts.
    /// </summary>
    private readonly Dictionary<string, int> _endpointUsage = new();
    
    /// <summary>
    /// The total number of requests recorded.
    /// </summary>
    private int _totalRequests = 0;
    
    /// <summary>
    /// Successful request count.
    /// </summary>
    private int _successfulRequests = 0;
    
    /// <summary>
    /// Failed request count.
    /// </summary>
    private int _failedRequests = 0;
    
    /// <summary>
    /// Lock object for thread safety.
    /// </summary>
    private readonly object _lock = new();

    /// <inheritdoc />
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
    
    /// <inheritdoc />
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
    
    /// <inheritdoc />
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