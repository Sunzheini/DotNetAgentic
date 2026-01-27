namespace DotNetAgentic.Agents;

public class OrchestrationResult
{
    public string OriginalTask { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public List<ExecutionStep> Steps { get; set; } = new();
    public string Summary { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
}