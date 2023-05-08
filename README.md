# Minimal Blazor app with Backend API

![Todos](/Screenshots/Todos.png)

## Features

Hosted Blazor WebAssembly app - on ASP.NET Core backend.

Contains code from MudBlazor Docs.

### Frontend
* Blazor - MudBlazor UI
* Web API clients - Generated from Open API
* SignalR client - Generated with Source Generator
* Localization

### Backend
* Minimal API endpoints
  * Versioned API
  * Open API with Swagger UI
* SignalR
* Entity Framework Core - Sqlite database
* Integration tests

## Run

To run the app, execute this in the ``Server`` directory:

```csharp
dotnet run
```

or

```csharp
dotnet watch
```

## Debugging

The project has been set up to run in VS Code.

Attach using the defined launch configurations.

### Debug Client (Frontend)

To debug the Client, start the Server project.
Then, attach the debugger to the browser with the "Client Attach (WASM)" launch configuration

A debug instance of your selected browser (Chrome or Edge) will then launch.

![Debug Client](/Screenshots/DebugClient.png)

