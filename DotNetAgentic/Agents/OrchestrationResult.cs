namespace DotNetAgentic.Agents;

/// <summary>
/// The result of an orchestration process, including the original task, plan, execution steps, summary, and completion time.
/// </summary>
public class OrchestrationResult
{
    /// <summary>
    /// The original task assigned to the agent.
    /// </summary>
    public string OriginalTask { get; set; } = string.Empty;
    
    /// <summary>
    /// The plan formulated by the planning agent.
    /// </summary>
    public string Plan { get; set; } = string.Empty;
    
    /// <summary>
    /// The list of execution steps taken by the execution agent.
    /// </summary>
    public List<ExecutionStep> Steps { get; set; } = new();
    
    /// <summary>
    /// The summary of the orchestration result.
    /// </summary>
    public string Summary { get; set; } = string.Empty;
    
    /// <summary>
    /// The timestamp when the orchestration was completed.
    /// </summary>
    public DateTime CompletedAt { get; set; }
}