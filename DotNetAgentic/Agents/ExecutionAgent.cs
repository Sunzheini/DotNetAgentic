using DotNetAgentic.Services;

namespace DotNetAgentic.Agents;

/// <summary>
/// Execution agent that carries out plans using tools.
/// </summary>
public class ExecutionAgent
{
    private readonly ToolRegistry _toolRegistry;
    
    public ExecutionAgent(ToolRegistry toolRegistry)
    {
        _toolRegistry = toolRegistry;
    }
    
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