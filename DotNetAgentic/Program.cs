using DotNetAgentic.Services;
using DotNetEnv;

Env.Load(); 

// 1. Creates the app builder with command line args
var builder = WebApplication.CreateBuilder(args);

// 2. Load environment variables
var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
if (string.IsNullOrEmpty(openAiKey))
{
    Console.WriteLine("⚠️ WARNING: OPENAI_API_KEY environment variable not set!");
    Console.WriteLine("Set it with: set OPENAI_API_KEY=your-key (Windows) or export OPENAI_API_KEY=your-key (Linux/Mac)");
}

// 3. Add MVC controllers (AgentController, etc.)
builder.Services.AddControllers();

// 4. Enable API explorer for Swagger documentation
builder.Services.AddEndpointsApiExplorer();

// 5. Generate Swagger JSON from controllers
builder.Services.AddSwaggerGen();

// 6. Register AgentService
builder.Services.AddScoped<AgentService>();

// 7. BUILD the application from configured services
var app = builder.Build();

// 8. Only in development: Show Swagger docs
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();      // Makes /swagger/v1/swagger.json available
    app.UseSwaggerUI();    // Shows UI at /swagger
}

// 9. Redirect HTTP → HTTPS (security)
app.UseHttpsRedirection();

// 10. Enable authorization (for protected endpoints)
app.UseAuthorization();

// 11. Map your controller routes (AgentController, etc.)
app.MapControllers();

// 12. Add a simple root endpoint
app.MapGet("/", () => "AI Agentic API - Go to /swagger for documentation");

// 13. Run the app (starts listening for requests)
app.Run();
