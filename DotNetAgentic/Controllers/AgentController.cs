using DotNetAgentic.Agents;
using DotNetAgentic.Models;
using DotNetAgentic.Services;
using DotNetAgentic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAgentic.Controllers;

/// <summary>
/// The AgentController handles API requests related to the AI agent functionalities,
/// including chat interactions, tool executions, session memory management, and monitoring.
/// </summary>
[ApiController]                     // MARK this class as an API controller (enables auto features)
[Route("api/[controller]")]  // Maps to /api/agent
public class AgentController : ControllerBase
{
    /// <summary>
    /// The agent service for processing messages and managing tools.
    /// </summary>
    private readonly IAgentService _agentService;
    
    /// <summary>
    /// The tool registry for managing and executing tools.
    /// </summary>
    private readonly ToolRegistry _toolRegistry;
    
    /// <summary>
    /// The memory store for session history and summaries.
    /// </summary>
    private readonly IMemoryStore _memoryStore;
    
    /// <summary>
    /// The agent orchestrator for handling complex task orchestration.
    /// </summary>
    private readonly AgentOrchestrator _agentOrchestrator;
    
    /// <summary>
    /// The telemetry service for logging and monitoring.
    /// </summary>
    private readonly ITelemetryService _telemetryService;
    
    /// <summary>
    /// The metrics service for collecting performance metrics.
    /// </summary>
    private readonly IMetricsService _metricsService;
    
    // ReSharper disable once ConvertToPrimaryConstructor
    public AgentController(IAgentService agentService, ToolRegistry toolRegistry, 
        IMemoryStore memoryStore, AgentOrchestrator agentOrchestrator, 
        ITelemetryService telemetryService, IMetricsService metricsService)
    {
        _agentService = agentService;
        _toolRegistry = toolRegistry;
        _memoryStore = memoryStore;
        _agentOrchestrator = agentOrchestrator;
        _telemetryService = telemetryService;
        _metricsService = metricsService;
    }
    
    public record ToolExecutionRequest(string ToolName, string Input);
    
    #region API Endpoints
    [HttpPost("chat")]      // POST api/agent/chat
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
    
    [HttpPost("orchestrate")]
    public async Task<IActionResult> OrchestrateTask([FromBody] OrchestrationRequest request)
    {
        try
        {
            var result = await _agentOrchestrator.ExecuteComplexTaskAsync(request.Task);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    public record OrchestrationRequest(string Task);
    
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
    
    // Add monitoring endpoints
    [HttpGet("telemetry")]
    public async Task<IActionResult> GetTelemetry([FromQuery] int count = 50)
    {
        var logs = await _telemetryService.GetRecentLogsAsync(count);
        return Ok(logs);
    }

    [HttpGet("telemetry/{sessionId}")]
    public async Task<IActionResult> GetTelemetryBySession(string sessionId)
    {
        var logs = await _telemetryService.GetLogsBySessionAsync(sessionId);
        return Ok(logs);
    }

    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics()
    {
        var metrics = await _metricsService.GetMetricsAsync();
        return Ok(metrics);
    }

    [HttpDelete("telemetry")]
    public async Task<IActionResult> ClearTelemetry()
    {
        await _telemetryService.ClearLogsAsync();
        await _metricsService.ResetMetricsAsync();
        return Ok(new { message = "Telemetry cleared" });
    }
    #endregion
}