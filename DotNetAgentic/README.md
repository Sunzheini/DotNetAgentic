# DotNetAgentic
A .Net app to showcase AI agentic development.

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