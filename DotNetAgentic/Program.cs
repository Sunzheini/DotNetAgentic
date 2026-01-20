// 1. Creates the app builder with command line args
var builder = WebApplication.CreateBuilder(args);

// 2. Add MVC controllers (your AgentController, etc.)
builder.Services.AddControllers();

// 3. Enable API explorer for Swagger documentation
builder.Services.AddEndpointsApiExplorer();

// 4. Generate Swagger JSON from controllers
builder.Services.AddSwaggerGen();

// 5. BUILD the application from configured services
var app = builder.Build();

// 6. Only in development: Show Swagger docs
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();      // Makes /swagger/v1/swagger.json available
    app.UseSwaggerUI();    // Shows UI at /swagger
}

// 7. Redirect HTTP → HTTPS (security)
app.UseHttpsRedirection();

// 8. Enable authorization (for protected endpoints)
app.UseAuthorization();

// 9. Map your controller routes (AgentController, etc.)
app.MapControllers();

// 10. Run the app (starts listening for requests)
app.Run();
