using Tavily;

namespace DotNetAgentic.Tools;

/// <summary>
/// Tool that performs web searches using the Tavily AI service.
/// </summary>
public class TavilySearchTool : ITool
{
    private readonly TavilyClient _client;
    private readonly string _apiKey;
    
    public TavilySearchTool(string apiKey)
    {
        _apiKey = apiKey;
        _client = new TavilyClient(new HttpClient());
    }
    
    public string Name => "tavily_search";
    public string Description => "Search the web for current information using Tavily AI";
    
    public async Task<string> ExecuteAsync(string query)
    {
        try
        {
            var request = new SearchRequest
            {
                ApiKey = _apiKey,
                Query = query,
                MaxResults = 3
            };
            
            var response = await _client.SearchAsync(request);
            
            var formatted = new List<string>();
            
            // Add answer if available
            if (!string.IsNullOrEmpty(response.Answer))
            {
                formatted.Add($"🤖 Answer: {response.Answer}");
                formatted.Add("");
            }
            
            // Add search results
            if (response.Results.Any())
            {
                formatted.Add("🔍 Top results:");
                
                for (int i = 0; i < Math.Min(response.Results.Count, 3); i++)
                {
                    var result = response.Results[i];
                    formatted.Add($"{i + 1}. {result.Title}");
                    
                    // Use Content from the result
                    if (!string.IsNullOrEmpty(result.Content))
                    {
                        formatted.Add($"   {result.Content}");
                    }
                    
                    formatted.Add($"   📎 Source: {result.Url}");
                    formatted.Add(""); // Empty line between results
                }
            }
            else
            {
                formatted.Add("No results found.");
            }
            
            return string.Join("\n", formatted);
        }
        catch (Exception ex)
        {
            return $"Search error: {ex.Message}";
        }
    }
}