using DotNetAgentic.Services;

namespace DotNetAgentic.Agents;

/// <summary>
/// Execution agent that carries out plans using tools.
/// </summary>
public class ExecutionAgent
{
    /// <summary>
    /// The tool registry for accessing available tools.
    /// </summary>
    private readonly ToolRegistry _toolRegistry;
    
    public ExecutionAgent(ToolRegistry toolRegistry)
    {
        _toolRegistry = toolRegistry;
    }
    
    /// <summary>
    /// Executes a single step, utilizing tools as needed.
    /// </summary>
    /// <param name="step">
    /// The step description to execute.
    /// </param>
    /// <returns>
    /// Results of the step execution.
    /// </returns>
    public async Task<string> ExecuteStepAsync(string step)
    {
        // Check if step requires a tool
        if (step.Contains("calculate") || step.Contains("math"))
        {
            // Extract calculation
            var calculation = ExtractCalculation(step);
            if (!string.IsNullOrEmpty(calculation))
            {
                return await _toolRegistry.ExecuteToolAsync("calculator", calculation);
            }
        }
        
        if (step.Contains("search") || step.Contains("find") || step.Contains("look up"))
        {
            // Extract search query
            var query = ExtractSearchQuery(step);
            if (!string.IsNullOrEmpty(query))
            {
                return await _toolRegistry.ExecuteToolAsync("web_search", query);
            }
        }
        
        return $"Executed: {step}";
    }
    
    /// <summary>
    /// Extracts calculation expression from the step text.
    /// </summary>
    /// <param name="text">
    /// The step text.
    /// </param>
    /// <returns>
    /// Returns the extracted calculation expression.
    /// </returns>
    private string ExtractCalculation(string text)
    {
        // Simple extraction - in real app use better NLP
        if (text.Contains("calculate") && text.Length > 10)
        {
            var start = text.IndexOf("calculate", StringComparison.OrdinalIgnoreCase);
            return text.Substring(start + 9).Trim();
        }
        return "";
    }
    
    /// <summary>
    /// Extracts search query from the step text.
    /// </summary>
    /// <param name="text">
    /// The step text.
    /// </param>
    /// <returns>
    /// Returns the extracted search query.
    /// </returns>
    private string ExtractSearchQuery(string text)
    {
        if (text.Contains("search for") && text.Length > 11)
        {
            var start = text.IndexOf("search for", StringComparison.OrdinalIgnoreCase);
            return text.Substring(start + 10).Trim();
        }
        return text;
    }
}