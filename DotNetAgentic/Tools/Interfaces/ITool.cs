namespace DotNetAgentic.Tools.Interfaces;

/// <summary>
/// The ITool interface defines the contract for tools that can be used by agents within the DotNetAgentic framework.
/// </summary>
public interface ITool
{
    /// <summary>
    /// The name of the tool.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// The description of the tool.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// The method to execute the tool's functionality.
    /// </summary>
    /// <param name="input">
    /// A string input that the tool will process.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing the tool's output as a string.
    /// </returns>
    Task<string> ExecuteAsync(string input);
}