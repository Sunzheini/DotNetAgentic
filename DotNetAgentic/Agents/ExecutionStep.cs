namespace DotNetAgentic.Agents;

public class ExecutionStep
{
    public string Description { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}