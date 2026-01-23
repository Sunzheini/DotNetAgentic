﻿﻿﻿﻿﻿using DotNetAgentic.Agents;
using DotNetAgentic.Controllers;
using DotNetAgentic.Models;
using DotNetAgentic.Services;
using DotNetAgentic.Tools;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace DotNetAgentic.Tests.Controllers;

[TestFixture]
public class AgentControllerTests
{
    private Mock<IAgentService> _mockAgentService = null!;
    private Mock<IMemoryStore> _mockMemoryStore = null!;
    private Mock<AgentOrchestrator> _mockAgentOrchestrator = null!;
    private Mock<ITelemetryService> _mockTelemetryService = null!;
    private Mock<IMetricsService> _mockMetricsService = null!;
    private ToolRegistry _toolRegistry = null!;
    private AgentController _controller = null!;
    private string? _originalTavilyApiKey;
    
    [SetUp]
    public void Setup()
    {
        // Save and clear TAVILY_API_KEY to prevent TavilySearchTool from registering
        _originalTavilyApiKey = Environment.GetEnvironmentVariable("TAVILY_API_KEY");
        Environment.SetEnvironmentVariable("TAVILY_API_KEY", null);
        
        // Set temporary OPENAI_API_KEY for PlanningAgent initialization
        var originalOpenAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(originalOpenAiKey))
        {
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-key-for-mock");
        }
        
        _mockAgentService = new Mock<IAgentService>();
        _mockMemoryStore = new Mock<IMemoryStore>();
        _mockTelemetryService = new Mock<ITelemetryService>();
        _mockMetricsService = new Mock<IMetricsService>();
        _toolRegistry = new ToolRegistry();
        
        // Mock AgentOrchestrator - we just need it to exist for the controller
        // The tests don't actually test orchestration functionality
        try
        {
            _mockAgentOrchestrator = new Mock<AgentOrchestrator>(
                new PlanningAgent(), 
                new ExecutionAgent(_toolRegistry));
        }
        catch
        {
            // If mocking fails, create a mock without calling the base constructor
            _mockAgentOrchestrator = new Mock<AgentOrchestrator>(
                MockBehavior.Loose,
                Mock.Of<PlanningAgent>(), 
                new ExecutionAgent(_toolRegistry));
        }
        
        // Restore original key if it was empty
        if (string.IsNullOrEmpty(originalOpenAiKey))
        {
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", originalOpenAiKey);
        }
        
        _controller = new AgentController(
            _mockAgentService.Object, 
            _toolRegistry, 
            _mockMemoryStore.Object,
            _mockAgentOrchestrator.Object,
            _mockTelemetryService.Object,
            _mockMetricsService.Object);
    }
    
    [TearDown]
    public void TearDown()
    {
        // Restore TAVILY_API_KEY
        if (_originalTavilyApiKey != null)
        {
            Environment.SetEnvironmentVariable("TAVILY_API_KEY", _originalTavilyApiKey);
        }
    }
    
    [Test]
    public async Task Chat_ValidRequest_ReturnsOkWithAgentResponse()
    {
        // Arrange
        _mockAgentService.Setup(x => x.ProcessAsync("Hello", "test-session"))
            .ReturnsAsync("Mocked AI response");
        
        var request = new AgentRequest 
        { 
            Message = "Hello", 
            SessionId = "test-session" 
        };
        
        // Act
        var result = await _controller.Chat(request);
        
        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        
        var okResult = (OkObjectResult)result.Result!;
        var response = (AgentResponse)okResult.Value!;
        
        Assert.That(response.Content, Is.EqualTo("Mocked AI response"));
        Assert.That(response.ToolCalls, Is.Empty);
    }
    
    [Test]
    public async Task Chat_ServiceThrowsException_ReturnsBadRequest()
    {
        // Arrange
        _mockAgentService.Setup(x => x.ProcessAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("API error"));
        
        var request = new AgentRequest 
        { 
            Message = "Test", 
            SessionId = "test" 
        };
        
        // Act
        var result = await _controller.Chat(request);
        
        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }
    
    [Test]
    public void HealthCheck_ReturnsOkWithStatus()
    {
        // Act
        var result = _controller.HealthCheck();
        
        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.Not.Null);
    }
    
    [Test]
    public void GetTools_ReturnsListOfAvailableTools()
    {
        // Arrange
        var mockTools = new List<ITool>
        {
            new CalculatorTool(),
            new WebSearchTool()
        };
        
        _mockAgentService.Setup(x => x.GetAvailableTools())
            .Returns(mockTools);
        
        // Act
        var result = _controller.GetTools();
        
        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.Not.Null);
        
        // Verify the tools are in the result
        var toolsList = okResult.Value as IEnumerable<object>;
        Assert.That(toolsList, Is.Not.Null);
        Assert.That(toolsList!.Count(), Is.EqualTo(2));
    }
    
    [Test]
    public async Task ExecuteTool_WithCalculator_ReturnsCorrectResult()
    {
        // Arrange - Request body format: {"toolName": "calculator", "input": "2+2"}
        var request = new AgentController.ToolExecutionRequest(
            ToolName: "calculator",
            Input: "2+2"
        );
        
        // Act
        var result = await _controller.ExecuteTool(request);
        
        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.Not.Null);
        
        // Verify the response contains tool and result
        var response = okResult.Value;
        var toolProperty = response!.GetType().GetProperty("tool");
        var resultProperty = response.GetType().GetProperty("result");
        
        Assert.That(toolProperty?.GetValue(response), Is.EqualTo("calculator"));
        
        var resultValue = resultProperty?.GetValue(response) as string;
        Assert.That(resultValue, Is.EqualTo("4"));
    }
    
    [Test]
    public async Task ExecuteTool_WithWebSearch_ReturnsSearchResults()
    {
        // Arrange - Request body format: {"toolName": "web_search", "input": "AI agentic patterns"}
        var request = new AgentController.ToolExecutionRequest(
            ToolName: "web_search",
            Input: "AI agentic patterns"
        );
        
        // Act
        var result = await _controller.ExecuteTool(request);
        
        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.Not.Null);
        
        // Verify the response contains tool and result
        var response = okResult.Value;
        var toolProperty = response!.GetType().GetProperty("tool");
        var resultProperty = response.GetType().GetProperty("result");
        
        Assert.That(toolProperty?.GetValue(response), Is.EqualTo("web_search"));
        
        var resultValue = resultProperty?.GetValue(response) as string;
        Assert.That(resultValue, Does.Contain("AI agentic patterns"));
    }
    
    [Test]
    public async Task ExecuteTool_WithInvalidTool_ReturnsErrorMessage()
    {
        // Arrange - Request body format: {"toolName": "invalid_tool", "input": "test"}
        var request = new AgentController.ToolExecutionRequest(
            ToolName: "invalid_tool",
            Input: "test"
        );
        
        // Act
        var result = await _controller.ExecuteTool(request);
        
        // Assert - Invalid tool returns OK with error message in result
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.Not.Null);
        
        // Verify error message in result field
        var response = okResult.Value;
        var resultProperty = response!.GetType().GetProperty("result");
        var resultValue = resultProperty?.GetValue(response) as string;
        
        Assert.That(resultValue, Does.Contain("Error"));
        Assert.That(resultValue, Does.Contain("invalid_tool"));
        Assert.That(resultValue, Does.Contain("not found"));
    }
    
    [Test]
    public async Task Chat_WithEmptyMessage_ReturnsBadRequest()
    {
        // Arrange
        _mockAgentService.Setup(x => x.ProcessAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new ArgumentException("Message cannot be empty"));
        
        var request = new AgentRequest 
        { 
            Message = "", 
            SessionId = "test-session" 
        };
        
        // Act
        var result = await _controller.Chat(request);
        
        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }
    
    // ==================== TELEMETRY TESTS ====================
    
    [Test]
    public async Task GetTelemetry_ReturnsRecentLogs()
    {
        // Arrange
        var mockLogs = new List<TelemetryRecord>
        {
            new() { Endpoint = "/api/agent/chat", SessionId = "session1", Operation = "chat" },
            new() { Endpoint = "/api/agent/tool", SessionId = "session2", Operation = "tool" }
        };
        
        _mockTelemetryService.Setup(x => x.GetRecentLogsAsync(50))
            .ReturnsAsync(mockLogs);
        
        // Act
        var result = await _controller.GetTelemetry(50);
        
        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        var logs = okResult.Value as List<TelemetryRecord>;
        
        Assert.That(logs, Is.Not.Null);
        Assert.That(logs!.Count, Is.EqualTo(2));
        Assert.That(logs[0].Endpoint, Is.EqualTo("/api/agent/chat"));
    }
    
    [Test]
    public async Task GetTelemetry_WithCustomCount_ReturnsSpecifiedNumber()
    {
        // Arrange
        var mockLogs = new List<TelemetryRecord>
        {
            new() { Endpoint = "/api/agent/chat", SessionId = "session1", Operation = "chat" }
        };
        
        _mockTelemetryService.Setup(x => x.GetRecentLogsAsync(10))
            .ReturnsAsync(mockLogs);
        
        // Act
        var result = await _controller.GetTelemetry(10);
        
        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _mockTelemetryService.Verify(x => x.GetRecentLogsAsync(10), Times.Once);
    }
    
    [Test]
    public async Task GetTelemetryBySession_ReturnsSessionLogs()
    {
        // Arrange
        var sessionId = "test-session-123";
        var mockLogs = new List<TelemetryRecord>
        {
            new() { Endpoint = "/api/agent/chat", SessionId = sessionId, Operation = "chat" },
            new() { Endpoint = "/api/agent/chat", SessionId = sessionId, Operation = "chat" }
        };
        
        _mockTelemetryService.Setup(x => x.GetLogsBySessionAsync(sessionId))
            .ReturnsAsync(mockLogs);
        
        // Act
        var result = await _controller.GetTelemetryBySession(sessionId);
        
        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        var logs = okResult.Value as List<TelemetryRecord>;
        
        Assert.That(logs, Is.Not.Null);
        Assert.That(logs!.Count, Is.EqualTo(2));
        Assert.That(logs.All(l => l.SessionId == sessionId), Is.True);
    }
    
    [Test]
    public async Task GetMetrics_ReturnsAgentMetrics()
    {
        // Arrange
        var mockMetrics = new AgentMetrics
        {
            TotalRequests = 100,
            SuccessfulRequests = 95,
            FailedRequests = 5,
            AverageResponseTimeMs = 250,
            ToolUsage = new Dictionary<string, int> { { "calculator", 30 }, { "web_search", 20 } },
            EndpointUsage = new Dictionary<string, int> { { "/api/agent/chat", 80 }, { "/api/agent/tool", 20 } }
        };
        
        _mockMetricsService.Setup(x => x.GetMetricsAsync())
            .ReturnsAsync(mockMetrics);
        
        // Act
        var result = await _controller.GetMetrics();
        
        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        var metrics = okResult.Value as AgentMetrics;
        
        Assert.That(metrics, Is.Not.Null);
        Assert.That(metrics!.TotalRequests, Is.EqualTo(100));
        Assert.That(metrics.SuccessfulRequests, Is.EqualTo(95));
        Assert.That(metrics.FailedRequests, Is.EqualTo(5));
        Assert.That(metrics.AverageResponseTimeMs, Is.EqualTo(250));
        Assert.That(metrics.ToolUsage.Count, Is.EqualTo(2));
        Assert.That(metrics.EndpointUsage.Count, Is.EqualTo(2));
    }
    
    [Test]
    public async Task ClearTelemetry_ClearsBothLogsAndMetrics()
    {
        // Arrange
        _mockTelemetryService.Setup(x => x.ClearLogsAsync())
            .Returns(Task.CompletedTask);
        
        _mockMetricsService.Setup(x => x.ResetMetricsAsync())
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _controller.ClearTelemetry();
        
        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.Value, Is.Not.Null);
        
        // Verify both services were called
        _mockTelemetryService.Verify(x => x.ClearLogsAsync(), Times.Once);
        _mockMetricsService.Verify(x => x.ResetMetricsAsync(), Times.Once);
    }
    
    [Test]
    public async Task OrchestrateTask_WithValidRequest_ReturnsOrchestrationResult()
    {
        // Arrange
        var task = "Calculate 5+3 and search for weather";
        var orchestrationResult = new OrchestrationResult
        {
            OriginalTask = task,
            Plan = "Step 1: Calculate 5+3\nStep 2: Search for weather",
            Steps = new List<ExecutionStep>
            {
                new() { Description = "Calculate 5+3", Result = "8", Timestamp = DateTime.UtcNow }
            },
            Summary = "Task completed successfully",
            CompletedAt = DateTime.UtcNow
        };
        
        _mockAgentOrchestrator.Setup(x => x.ExecuteComplexTaskAsync(task))
            .ReturnsAsync(orchestrationResult);
        
        var request = new AgentController.OrchestrationRequest(task);
        
        // Act
        var result = await _controller.OrchestrateTask(request);
        
        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        var resultValue = okResult.Value as OrchestrationResult;
        
        Assert.That(resultValue, Is.Not.Null);
        Assert.That(resultValue!.OriginalTask, Is.EqualTo(task));
        Assert.That(resultValue.Steps.Count, Is.EqualTo(1));
    }
    
    [Test]
    public async Task OrchestrateTask_WhenOrchestratorThrows_ReturnsBadRequest()
    {
        // Arrange
        var task = "Invalid task";
        _mockAgentOrchestrator.Setup(x => x.ExecuteComplexTaskAsync(task))
            .ThrowsAsync(new Exception("Orchestration failed"));
        
        var request = new AgentController.OrchestrationRequest(task);
        
        // Act
        var result = await _controller.OrchestrateTask(request);
        
        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
}

