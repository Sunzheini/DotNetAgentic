namespace DotNetAgentic.Core.ManagerBase;

/// <summary>
/// The base interface for all manager types.
/// Provides common functionality for resource management, health monitoring, and lifecycle management.
/// </summary>
public interface IManagerBase
{
    /// <summary>
    /// Gets the type of this manager.
    /// </summary>
    ManagerType ManagerType { get; }
    
    /// <summary>
    /// Gets the name/identifier of this manager instance.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets whether the manager has been initialized and is ready for use.
    /// </summary>
    bool IsInitialized { get; }
    
    /// <summary>
    /// Initializes the manager asynchronously.
    /// </summary>
    /// <returns>A task representing the initialization operation.</returns>
    Task InitializeAsync();
    
    /// <summary>
    /// Performs health check on the manager and its resources.
    /// </summary>
    /// <returns>A task containing the health status result.</returns>
    Task<HealthStatus> CheckHealthAsync();
    
    /// <summary>
    /// Gets metrics/statistics about the manager's operations.
    /// </summary>
    /// <returns>A dictionary of metric names to values.</returns>
    IDictionary<string, object> GetMetrics();
    
    /// <summary>
    /// Disposes resources used by the manager.
    /// </summary>
    /// <returns>A task representing the disposal operation.</returns>
    Task DisposeAsync();
}