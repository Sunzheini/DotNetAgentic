using DotNetAgentic.Models;
using DotNetAgentic.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAgentic.Controllers;

[ApiController]                     // MARK this class as an API controller (enables auto features)
[Route("api/[controller]")]  // Maps to /api/agent
public class AgentController : ControllerBase
{
    private readonly AgentService _agentService;
    
    public AgentController(AgentService agentService)
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
}