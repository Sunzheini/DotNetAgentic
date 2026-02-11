namespace DotNetAgentic.Core.ManagerBase;

/// <summary>
/// Abstract base class for all managers, providing common functionality.
/// Implements lifecycle management, health monitoring, and metrics collection.
/// </summary>
/// <typeparam name="TRegistry">The type of registry this manager uses.</typeparam>
public abstract class ManagerBase<TRegistry> : IManagerBase where TRegistry : class
{
    private readonly Dictionary<string, object> _metrics = new();
    private bool _isDisposed;
    
    /// <summary>
    /// Gets the registry instance.
    /// </summary>
    protected TRegistry Registry { get; }
    
    /// <summary>
    /// Gets the type of this manager.
    /// </summary>
    public abstract ManagerType ManagerType { get; }
    
    /// <summary>
    /// Gets the name of this manager.
    /// </summary>
    public virtual string Name => ManagerType.ToString();
    
    /// <summary>
    /// Gets whether the manager has been initialized.
    /// </summary>
    public bool IsInitialized { get; protected set; }
    
    /// <summary>
    /// Initializes a new instance of the ManagerBase class.
    /// </summary>
    /// <param name="registry">The registry instance.</param>
    protected ManagerBase(TRegistry registry)
    {
        Registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }
    
    /// <summary>
    /// Initializes the manager asynchronously.
    /// </summary>
    public virtual async Task InitializeAsync()
    {
        if (IsInitialized)
            return;
            
        await OnInitializeAsync();
        IsInitialized = true;
        UpdateMetric("InitializedAt", DateTime.UtcNow);
    }
    
    /// <summary>
    /// Override this method to provide custom initialization logic.
    /// </summary>
    protected virtual Task OnInitializeAsync()
    {
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Performs health check on the manager.
    /// </summary>
    public virtual async Task<HealthStatus> CheckHealthAsync()
    {
        try
        {
            if (!IsInitialized)
            {
                return HealthStatus.Unhealthy($"{Name} is not initialized");
            }
            
            var customHealthCheck = await OnCheckHealthAsync();
            if (customHealthCheck != null)
            {
                return customHealthCheck;
            }
            
            return HealthStatus.Healthy($"{Name} is operational");
        }
        catch (Exception ex)
        {
            return HealthStatus.Unhealthy($"{Name} health check failed", ex);
        }
    }
    
    /// <summary>
    /// Override this method to provide custom health check logic.
    /// </summary>
    /// <returns>A HealthStatus object, or null to use the default healthy status.</returns>
    protected virtual Task<HealthStatus?> OnCheckHealthAsync()
    {
        return Task.FromResult<HealthStatus?>(null);
    }
    
    /// <summary>
    /// Gets metrics about the manager's operations.
    /// </summary>
    public virtual IDictionary<string, object> GetMetrics()
    {
        return new Dictionary<string, object>(_metrics)
        {
            ["ManagerType"] = ManagerType.ToString(),
            ["IsInitialized"] = IsInitialized,
            ["Name"] = Name
        };
    }
    
    /// <summary>
    /// Updates a metric value.
    /// </summary>
    protected void UpdateMetric(string key, object value)
    {
        _metrics[key] = value;
    }
    
    /// <summary>
    /// Increments a counter metric.
    /// </summary>
    protected void IncrementMetric(string key, long amount = 1)
    {
        if (_metrics.TryGetValue(key, out var existing) && existing is long counter)
        {
            _metrics[key] = counter + amount;
        }
        else
        {
            _metrics[key] = amount;
        }
    }
    
    /// <summary>
    /// Disposes resources used by the manager.
    /// </summary>
    public virtual async Task DisposeAsync()
    {
        if (_isDisposed)
            return;
            
        await OnDisposeAsync();
        _isDisposed = true;
    }
    
    /// <summary>
    /// Override this method to provide custom disposal logic.
    /// </summary>
    protected virtual Task OnDisposeAsync()
    {
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Ensures the manager has been initialized.
    /// </summary>
    protected void EnsureInitialized()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException($"{Name} must be initialized before use. Call InitializeAsync() first.");
        }
    }
}

