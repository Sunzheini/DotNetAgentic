namespace DotNetAgentic.Services;

/// <summary>
/// Interface for AI agent service that processes user input.
/// </summary>
public interface IAgentService
{
    /// <summary>
    /// Processes user input and returns AI-generated response.
    /// </summary>
    /// <param name="input">The user's input message</param>
    /// <returns>AI-generated response text</returns>
    Task<string> ProcessAsync(string input, string sessionId = "default");
    
    /// <summary>
    /// Threads user input with memory for context-aware responses.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="sessionId"></param>
    /// <returns> A context-aware AI-generated response text</returns>
    Task<string> ProcessWithMemoryAsync(string input, string sessionId);
    
    /// <summary>
    /// Gets all available tools that the agent can use.
    /// </summary>
    /// <returns>List of available tools</returns>
    List<Tools.ITool> GetAvailableTools();
}
