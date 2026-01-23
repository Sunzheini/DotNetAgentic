using DotNetAgentic.Models;
using DotNetAgentic.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAgentic.Controllers;

[ApiController]                     // MARK this class as an API controller (enables auto features)
[Route("api/[controller]")]  // Maps to /api/agent
public class AgentController : ControllerBase
{
    private readonly IAgentService _agentService;
    private readonly ToolRegistry _toolRegistry;
    private readonly IMemoryStore _memoryStore;
    
    // ReSharper disable once ConvertToPrimaryConstructor
    public AgentController(IAgentService agentService, ToolRegistry toolRegistry, IMemoryStore memoryStore)
    {
        _agentService = agentService;
        _toolRegistry = toolRegistry;
        _memoryStore = memoryStore;
    }
    
    // --------------------------------------------------------------------------------------
    // POST api/agent/chat
    [HttpPost("chat")]
    public async Task<ActionResult<AgentResponse>> Chat([FromBody] AgentRequest request)
    {
        try
        {
            // Use provided sessionId or default to "default"
            var sessionId = request.SessionId ?? "default";
            var response = await _agentService.ProcessAsync(request.Message, sessionId);
            
            return Ok(new AgentResponse
            {
                Content = response,
                ToolCalls = new List<ToolCall>(),  // Empty for now
                Reasoning = $"Processed by Semantic Kernel agent (Session: {sessionId})"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    // GET api/agent/health
    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new { status = "AI Agent API is running", timestamp = DateTime.UtcNow });
    }
    
    [HttpGet("tools")]
    public IActionResult GetTools()
    {
        var tools = _agentService.GetAvailableTools();
        var toolList = tools.Select(t => new 
        { 
            t.Name, 
            t.Description 
        });
    
        return Ok(toolList);
    }

    [HttpPost("tool")]
    public async Task<IActionResult> ExecuteTool([FromBody] ToolExecutionRequest request)
    {
        try
        {
            var result = await _toolRegistry.ExecuteToolAsync(request.ToolName, request.Input);
            return Ok(new { tool = request.ToolName, result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET api/agent/memory/{sessionId}
    [HttpGet("memory/{sessionId}")]
    public async Task<IActionResult> GetSessionHistory(string sessionId, [FromQuery] int maxRecords = 10)
    {
        try
        {
            var history = await _memoryStore.GetSessionHistoryAsync(sessionId, maxRecords);
            return Ok(new { sessionId, recordCount = history.Count, history });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET api/agent/memory/{sessionId}/summary
    [HttpGet("memory/{sessionId}/summary")]
    public async Task<IActionResult> GetSessionSummary(string sessionId)
    {
        try
        {
            var summary = await _memoryStore.GetSessionSummaryAsync(sessionId);
            return Ok(new { sessionId, summary });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE api/agent/memory/{sessionId}
    [HttpDelete("memory/{sessionId}")]
    public async Task<IActionResult> ClearSession(string sessionId)
    {
        try
        {
            await _memoryStore.ClearSessionAsync(sessionId);
            return Ok(new { message = $"Session '{sessionId}' cleared successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    public record ToolExecutionRequest(string ToolName, string Input);
}