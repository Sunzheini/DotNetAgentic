namespace DotNetAgentic.Agents;

/// <summary>
/// Orchestrates multiple agents to work together.
/// </summary>
public class AgentOrchestrator
{
    private readonly PlanningAgent _planningAgent;
    private readonly ExecutionAgent _executionAgent;
    
    public AgentOrchestrator(PlanningAgent planningAgent, ExecutionAgent executionAgent)
    {
        _planningAgent = planningAgent;
        _executionAgent = executionAgent;
    }
    
    public async Task<OrchestrationResult> ExecuteComplexTaskAsync(string task)
    {
        var result = new OrchestrationResult
        {
            OriginalTask = task,
            Steps = new List<ExecutionStep>()
        };
        
        // Step 1: Planning
        var plan = await _planningAgent.CreatePlanAsync(task);
        result.Plan = plan;
        
        // Step 2: Parse and execute steps
        var steps = ParsePlan(plan);
        
        foreach (var step in steps)
        {
            var stepResult = await _executionAgent.ExecuteStepAsync(step);
            
            result.Steps.Add(new ExecutionStep
            {
                Description = step,
                Result = stepResult,
                Timestamp = DateTime.UtcNow
            });
            
            // Add delay between steps (simulate real processing)
            await Task.Delay(500);
        }
        
        // Step 3: Summarize results
        result.Summary = await GenerateSummaryAsync(result);
        result.CompletedAt = DateTime.UtcNow;
        
        return result;
    }
    
    private List<string> ParsePlan(string plan)
    {
        // Simple parsing - split by numbered lines
        return plan.Split('\n')
            .Where(line => line.Trim().StartsWith("1.") || 
                          line.Trim().StartsWith("2.") || 
                          line.Trim().StartsWith("3.") ||
                          line.Trim().StartsWith("4.") ||
                          line.Trim().StartsWith("5."))
            .Select(line => line.Substring(line.IndexOf('.') + 1).Trim())
            .ToList();
    }
    
    private Task<string> GenerateSummaryAsync(OrchestrationResult result)
    {
        return Task.FromResult($"Task completed in {result.Steps.Count} steps. " +
                               $"First step: {result.Steps.FirstOrDefault()?.Result?.Substring(0, Math.Min(50, result.Steps.FirstOrDefault()?.Result?.Length ?? 0))}...");
    }
}

public class OrchestrationResult
{
    public string OriginalTask { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public List<ExecutionStep> Steps { get; set; } = new();
    public string Summary { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
}

public class ExecutionStep
{
    public string Description { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}