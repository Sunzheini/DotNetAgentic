using DotNetAgentic.Agents;
using DotNetAgentic.Middleware;
using DotNetAgentic.Services;
using DotNetAgentic.Services.Interfaces;
using DotNetAgentic.Tools;
using DotNetAgentic.Tools.Interfaces;
using DotNetEnv;

Env.Load(); 

// 1. Creates the app builder with command line args
var builder = WebApplication.CreateBuilder(args);

// 2. Load environment variables
var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
if (string.IsNullOrEmpty(openAiKey))
{
    Console.WriteLine("WARNING: OPENAI_API_KEY environment variable not set!");
    Console.WriteLine("Set it with: set OPENAI_API_KEY=your-key (Windows) or export OPENAI_API_KEY=your-key (Linux/Mac)");
}

// 3. Add MVC controllers (AgentController, etc.)
builder.Services.AddControllers();

// 4. Enable API explorer for Swagger documentation
builder.Services.AddEndpointsApiExplorer();

// 5. Generate Swagger JSON from controllers
builder.Services.AddSwaggerGen();

// 6. Register AgentService
builder.Services.AddScoped<IAgentService, AgentService>();

// 7. Register MemoryStore
builder.Services.AddSingleton<IMemoryStore, InMemoryStore>();

// 8. Register multi-agent services
builder.Services.AddSingleton<PlanningAgent>();
builder.Services.AddSingleton<ExecutionAgent>();
builder.Services.AddSingleton<AgentOrchestrator>();

// 9. Add telemetry services
builder.Services.AddSingleton<ITelemetryService, TelemetryService>();
builder.Services.AddSingleton<IMetricsService, MetricsService>();

// 10. Register tool system
builder.Services.AddSingleton<ToolRegistry>();

// 11. Register all tools
builder.Services.AddSingleton<ITool, CalculatorTool>();
builder.Services.AddSingleton<ITool, WebSearchTool>();

var tavilyApiKey = Environment.GetEnvironmentVariable("TAVILY_API_KEY");
if (!string.IsNullOrEmpty(tavilyApiKey))
{
    builder.Services.AddSingleton<ITool>(new TavilySearchTool(tavilyApiKey));
}

// 12. BUILD the application from configured services
var app = builder.Build();

// Add telemetry middleware (add this early in pipeline)
app.UseMiddleware<TelemetryMiddleware>();

// 13. Only in development: Show Swagger docs
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();      // Makes /swagger/v1/swagger.json available
    app.UseSwaggerUI();    // Shows UI at /swagger
}

// 14. Redirect HTTP → HTTPS (security)
app.UseHttpsRedirection();

// 15. Enable authorization (for protected endpoints)
app.UseAuthorization();

// 16. Map your controller routes (AgentController, etc.)
app.MapControllers();

// 17. Add a simple root endpoint
app.MapGet("/", () => "AI Agentic API - Go to /swagger for documentation");

// 18. Run the app (starts listening for requests)
app.Run();
