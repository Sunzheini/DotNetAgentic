using DotNetAgentic.Tools.Interfaces;

namespace DotNetAgentic.Tools;

/// <summary>
/// Tool that performs mathematical calculations.
/// </summary>
public class CalculatorTool : ITool
{
    /// <inheritdoc />
    public string Name => "calculator";
    
    /// <inheritdoc />
    public string Description => "Performs mathematical calculations";
    
    /// <inheritdoc />
    public Task<string> ExecuteAsync(string input)
    {
        try
        {
            var dataTable = new System.Data.DataTable();
            var result = dataTable.Compute(input, "");
            return Task.FromResult(result.ToString() ?? "Error: Could not calculate");
        }
        catch (Exception ex)
        {
            return Task.FromResult($"Error: {ex.Message}");
        }
    }
}