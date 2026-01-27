﻿namespace DotNetAgentic.Agents;

/// <summary>
/// Orchestrates multiple agents to work together.
/// </summary>
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class AgentOrchestrator
{
    /// <summary>
    /// The planning agent responsible for creating task plans.
    /// </summary>
    private readonly PlanningAgent _planningAgent;
    
    /// <summary>
    /// The execution agent responsible for executing planned steps.
    /// </summary>
    private readonly ExecutionAgent _executionAgent;
    
    // ReSharper disable once ConvertToPrimaryConstructor
    public AgentOrchestrator(PlanningAgent planningAgent, ExecutionAgent executionAgent)
    {
        _planningAgent = planningAgent;
        _executionAgent = executionAgent;
    }
    
    /// <summary>
    /// Executes a complex task by coordinating planning and execution agents.
    /// </summary>
    /// <param name="task">
    /// The complex task description to be executed.
    /// </param>
    /// <returns>
    /// Returns an OrchestrationResult containing details of the execution.
    /// </returns>
    public virtual async Task<OrchestrationResult> ExecuteComplexTaskAsync(string task)
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
    
    /// <summary>
    /// Parses the plan into individual steps.
    /// </summary>
    /// <param name="plan">
    /// The plan string containing numbered steps.
    /// </param>
    /// <returns>
    /// Returns a list of individual step descriptions.
    /// </returns>
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
    
    /// <summary>
    /// Generates a summary of the orchestration result.
    /// </summary>
    /// <param name="result">
    /// The orchestration result containing executed steps.
    /// </param>
    /// <returns>
    /// Returns a summary string.
    /// </returns>
    private Task<string> GenerateSummaryAsync(OrchestrationResult result)
    {
        return Task.FromResult($"Task completed in {result.Steps.Count} steps. " +
                               $"First step: {result.Steps.FirstOrDefault()?.Result?.Substring(0, Math.Min(50, result.Steps.FirstOrDefault()?.Result?.Length ?? 0))}...");
    }
}