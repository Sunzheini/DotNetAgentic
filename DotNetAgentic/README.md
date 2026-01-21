# DotNetAgentic
A .Net app to showcase AI agentic development.


## Status
Continue with step8


## Structure
Controller-focused API (not full MVC)

### Connected Services
Connected Services is a Visual Studio feature that helps integrate external services (APIs, databases, cloud services) 
into your project with minimal setup.

### Properties/launchSettings.json
Visual Studio's run configuration file.
- Defines how your app runs (F5 behavior)
- Sets URLs and ports for web apps
- Configures development environment settings

### Controllers/WeatherForecastController.cs
This is a sample Weather API controller that Visual Studio auto-generates.

### appsettings.json
- Settings/configuration for your app
- JSON-based config file
- Loaded automatically at startup

### DotNetAgentic.http
HTTP Request File - For testing APIs
- A test script for your API endpoints
- Used by VS Code REST Client or JetBrains HTTP Client
- Like Postman/Insomnia but as a text file


## Creation
1. Repo in GitHub with Readme and .gitignore, clone
2. Create a ASP.NET Core Web API project, .NET 8.0
3. Use the .gitignore here to replace the GitHub one
4. Install Microsoft.SemanticKernel, DotNetEnv, Tavily via NuGet
5. Create a .env file in the project root
6. Use the .env like it is shown in Program.cs and AgentService.cs
7. Create test project:
dotnet new nunit -n DotNetAgentic.Tests
cd DotNetAgentic.Tests
dotnet add reference ../DotNetAgentic/DotNetAgentic.csproj
cd..
dotnet sln add DotNetAgentic.Tests/DotNetAgentic.Tests.csproj
dotnet remove package NUnit
dotnet add package NUnit --version 4.4.0
dotnet restore
dotnet add package Moq
dotnet restore
8. Test /api/Agent/tool with:
{
   "toolName": "tavily_search",
   "input": "How much is the temperature in Tokyo now?"
}

{
   "toolName": "calculator",
   "input": "1+3"
}





















