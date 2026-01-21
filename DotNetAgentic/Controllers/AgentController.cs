using DotNetAgentic.Models;
using DotNetAgentic.Services;
using DotNetAgentic.Tools;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAgentic.Controllers;

[ApiController]                     // MARK this class as an API controller (enables auto features)
[Route("api/[controller]")]  // Maps to /api/agent
public class AgentController : ControllerBase
{
    private readonly IAgentService _agentService;
    private readonly ToolRegistry _toolRegistry;
    
    // ReSharper disable once ConvertToPrimaryConstructor
    public AgentController(IAgentService agentService, ToolRegistry toolRegistry)
    {
        _agentService = agentService;
        _toolRegistry = toolRegistry;
    }
    
    // --------------------------------------------------------------------------------------
    // POST api/agent/chat
    [HttpPost("chat")]
    public async Task<ActionResult<AgentResponse>> Chat([FromBody] AgentRequest request)
    {
        try
        {
            var response = await _agentService.ProcessAsync(request.Message);
            
            return Ok(new AgentResponse
            {
                Content = response,
                ToolCalls = new List<ToolCall>(),  // Empty for now
                Reasoning = "Processed by Semantic Kernel agent"
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

    public record ToolExecutionRequest(string ToolName, string Input);
}