namespace DotNetAgentic.Agents;

/// <summary>
/// The ExecutionStep class represents a single step in the execution process of an agent.
/// </summary>
public class ExecutionStep
{
    /// <summary>
    /// Description of the execution step.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Result of the execution step.
    /// </summary>
    public string Result { get; set; } = string.Empty;
    
    /// <summary>
    /// Timestamp of when the execution step was performed.
    /// </summary>
    public DateTime Timestamp { get; set; }
}