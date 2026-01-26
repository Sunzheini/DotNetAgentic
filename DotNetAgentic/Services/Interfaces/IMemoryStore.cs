using DotNetAgentic.Models;

namespace DotNetAgentic.Services.Interfaces;

/// <summary>
/// The memory store interface for saving and retrieving memory records.
/// </summary>
public interface IMemoryStore
{
    /// <summary>
    /// Saves a memory record asynchronously.
    /// </summary>
    /// <param name="record">
    /// The memory record to save.
    /// </param>
    /// <returns>
    /// Returns a task that represents the asynchronous save operation.
    /// </returns>
    Task SaveAsync(MemoryRecord record);
    
    /// <summary>
    /// Gets the session history asynchronously.
    /// </summary>
    /// <param name="sessionId">
    /// The session identifier.
    /// </param>
    /// <param name="maxRecords">
    /// The maximum number of records to retrieve.
    /// </param>
    /// <returns>
    /// Returns a task that represents the asynchronous retrieval operation,
    /// </returns>
    Task<List<MemoryRecord>> GetSessionHistoryAsync(string sessionId, int maxRecords = 10);
    
    /// <summary>
    /// Gets the session summary asynchronously.
    /// </summary>
    /// <param name="sessionId">
    /// Session identifier.
    /// </param>
    /// <returns>
    /// Returns a task that represents the asynchronous retrieval operation,
    /// </returns>
    Task<string> GetSessionSummaryAsync(string sessionId);
    
    /// <summary>
    /// Clears the session asynchronously.
    /// </summary>
    /// <param name="sessionId">
    /// The session identifier.
    /// </param>
    /// <returns>
    /// Returns a task that represents the asynchronous clear operation,
    /// </returns>
    Task ClearSessionAsync(string sessionId);
}