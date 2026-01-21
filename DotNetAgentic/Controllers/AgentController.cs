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
    
    // ReSharper disable once ConvertToPrimaryConstructor
    public AgentController(IAgentService agentService)
    {
        _agentService = agentService;
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
            // Simple tool execution
            if (request.ToolName == "calculator")
            {
                var tool = new CalculatorTool();
                var result = await tool.ExecuteAsync(request.Input);
                return Ok(new { tool = request.ToolName, result });
            }
        
            return BadRequest(new { error = $"Tool '{request.ToolName}' not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    public record ToolExecutionRequest(string ToolName, string Input);
}