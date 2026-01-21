namespace DotNetAgentic.Tools;

/// <summary>
/// Tool that performs mathematical calculations.
/// </summary>
public class CalculatorTool : ITool
{
    public string Name => "calculator";
    public string Description => "Performs mathematical calculations";
    
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