using DotNetAgentic.Models;

namespace DotNetAgentic.Services;

/// <summary>
/// The memory store interface for saving and retrieving memory records.
/// </summary>
public interface IMemoryStore
{
    Task SaveAsync(MemoryRecord record);
    Task<List<MemoryRecord>> GetSessionHistoryAsync(string sessionId, int maxRecords = 10);
    Task<string> GetSessionSummaryAsync(string sessionId);
    Task ClearSessionAsync(string sessionId);
}