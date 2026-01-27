using DotNetAgentic.Models;
using DotNetAgentic.Services.Interfaces;

namespace DotNetAgentic.Services;

/// <summary>
/// Temporary in-memory store for session memories.
/// </summary>
public class InMemoryStore : IMemoryStore
{
    /// <summary>
    /// The in-memory dictionary storing session memories.
    /// </summary>
    private readonly Dictionary<string, List<MemoryRecord>> _sessionMemories = new();
    
    /// <summary>
    /// The lock object for thread-safe operations.
    /// </summary>
    private readonly object _lock = new();

    /// <inheritdoc />
    public Task SaveAsync(MemoryRecord record)
    {
        lock (_lock)
        {
            if (!_sessionMemories.ContainsKey(record.SessionId))
            {
                _sessionMemories[record.SessionId] = new List<MemoryRecord>();
            }
            
            _sessionMemories[record.SessionId].Add(record);
        }
        
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<List<MemoryRecord>> GetSessionHistoryAsync(string sessionId, int maxRecords = 10)
    {
        lock (_lock)
        {
            if (_sessionMemories.TryGetValue(sessionId, out var records))
            {
                return Task.FromResult(records
                    .OrderByDescending(r => r.Timestamp)
                    .Take(maxRecords)
                    .ToList());
            }
        }
        
        return Task.FromResult(new List<MemoryRecord>());
    }
    
    /// <inheritdoc />
    public Task<string> GetSessionSummaryAsync(string sessionId)
    {
        lock (_lock)
        {
            if (!_sessionMemories.TryGetValue(sessionId, out var records) || !records.Any())
            {
                return Task.FromResult("No conversation history.");
            }
            
            var summary = $"Conversation history ({records.Count} messages):\n";
            foreach (var record in records.OrderBy(r => r.Timestamp).Take(5))
            {
                summary += $"[{record.Timestamp:HH:mm}] User: {record.UserMessage}\n";
                summary += $"[{record.Timestamp:HH:mm}] Agent: {record.AgentResponse.Substring(0, Math.Min(50, record.AgentResponse.Length))}...\n";
            }
            
            return Task.FromResult(summary);
        }
    }
    
    /// <inheritdoc />
    public Task ClearSessionAsync(string sessionId)
    {
        lock (_lock)
        {
            _sessionMemories.Remove(sessionId);
        }
        
        return Task.CompletedTask;
    }
}