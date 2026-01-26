namespace DotNetAgentic.Services.Interfaces;

public interface IMetricsService
{
    Task RecordRequestAsync(string endpoint, string toolName, long durationMs, bool success);
    Task<AgentMetrics> GetMetricsAsync();
    Task ResetMetricsAsync();
}