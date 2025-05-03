# Logging Troubleshooting Guide

## Common Issues and Solutions

### No Logs Being Generated

#### Symptoms
- No log files are created in the Logs directory
- No log messages appear in the console
- Application runs without errors but logging is silent

#### Possible Causes and Solutions

1. **Missing Logs Directory**
   - **Cause**: The Logs directory doesn't exist and the application doesn't have permission to create it
   - **Solution**: Ensure the Logs directory exists in the application root or configure a different path
   ```csharp
   // Ensure Logs directory exists
   var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
   if (!Directory.Exists(logsDirectory))
   {
       Directory.CreateDirectory(logsDirectory);
   }
   ```

2. **Incorrect Log Level Configuration**
   - **Cause**: Log level is set too high (e.g., Error) and lower-level messages (e.g., Information) are filtered out
   - **Solution**: Check the log level configuration in appsettings.json
   ```json
   "Serilog": {
     "MinimumLevel": {
       "Default": "Information",
       "Override": {
         "Microsoft": "Warning",
         "System": "Warning"
       }
     }
   }
   ```

3. **Missing Serilog Configuration**
   - **Cause**: Serilog is not properly configured or initialized
   - **Solution**: Ensure Serilog is configured in Program.cs
   ```csharp
   Log.Logger = new LoggerConfiguration()
       .ReadFrom.Configuration(builder.Configuration)
       .CreateLogger();
   
   builder.Host.UseSerilog();
   ```

4. **DI Registration Issues**
   - **Cause**: Logging services are not properly registered in the DI container
   - **Solution**: Check service registration in Program.cs
   ```csharp
   // Client-side
   builder.Services.AddScoped<IClientLoggingService, ClientLoggingServiceImpl>();
   
   // Server-side
   builder.Services.AddScoped<IClientLoggingService, ServerClientLoggingService>();
   ```

### Logs Not Appearing in Expected Location

#### Symptoms
- Logs are being generated but not in the expected location
- Some components log correctly while others don't

#### Possible Causes and Solutions

1. **Multiple Configuration Sources**
   - **Cause**: Different parts of the application use different logging configurations
   - **Solution**: Consolidate logging configuration and ensure all components use the same configuration

2. **Incorrect Path Configuration**
   - **Cause**: Log file path is configured incorrectly
   - **Solution**: Check the path configuration in appsettings.json and ensure it's correct for the environment
   ```json
   "WriteTo": [
     {
       "Name": "File",
       "Args": {
         "path": "Logs/log-.txt",
         "rollingInterval": "Day"
       }
     }
   ]
   ```

3. **Permissions Issues**
   - **Cause**: The application doesn't have permission to write to the configured location
   - **Solution**: Check file system permissions and ensure the application has write access to the Logs directory

### Client-Side Logs Not Reaching the Server

#### Symptoms
- Client-side logs appear in the browser console but not in the server logs
- No errors are reported in the client or server

#### Possible Causes and Solutions

1. **Incorrect Implementation Registration**
   - **Cause**: The wrong implementation of `IClientLoggingService` is registered in the client
   - **Solution**: Ensure the client registers `ClientLoggingServiceImpl` and the server registers `ServerClientLoggingService`

2. **API Endpoint Configuration**
   - **Cause**: The logging API endpoint is not properly configured or is unreachable
   - **Solution**: Check the API endpoint configuration and ensure it's accessible from the server

3. **CORS Issues**
   - **Cause**: CORS policies are preventing the server from reaching the API
   - **Solution**: Check CORS configuration in the API project
   ```csharp
   builder.Services.AddCors(options =>
   {
     options.AddPolicy("AllowBlazorClient",
       builder => builder
         .WithOrigins("https://localhost:7214")
         .AllowAnyMethod()
         .AllowAnyHeader());
   });
   ```

### Excessive Logging

#### Symptoms
- Log files grow very large very quickly
- Performance degradation due to logging overhead
- Difficulty finding relevant log entries

#### Possible Causes and Solutions

1. **Inappropriate Log Levels**
   - **Cause**: Too many messages are logged at high levels (e.g., Information instead of Debug)
   - **Solution**: Review logging strategy and ensure appropriate log levels are used

2. **Missing Log Filtering**
   - **Cause**: No filtering is applied to logs from noisy components
   - **Solution**: Configure log filtering in appsettings.json
   ```json
   "Serilog": {
     "MinimumLevel": {
       "Default": "Information",
       "Override": {
         "Microsoft": "Warning",
         "System": "Warning",
         "Microsoft.AspNetCore": "Warning"
       }
     }
   }
   ```

3. **Missing Log Rotation**
   - **Cause**: Log files are not rotated or limited in size
   - **Solution**: Configure log rotation and size limits
   ```json
   "WriteTo": [
     {
       "Name": "File",
       "Args": {
         "path": "Logs/log-.txt",
         "rollingInterval": "Day",
         "fileSizeLimitBytes": 10485760,
         "retainedFileCountLimit": 31,
         "rollOnFileSizeLimit": true
       }
     }
   ]
   ```

## Interpreting Log Messages

### Common Log Patterns

#### HTTP Request Logs
```
[Information] HTTP GET Request: https://api.example.com/clients
[Information] HTTP GET Response: 200 from https://api.example.com/clients took 123ms
```
- Indicates a successful HTTP GET request that took 123ms to complete

#### Error Logs
```
[Error] Failed to send log to server: Client log message
System.Net.Http.HttpRequestException: Connection refused
```
- Indicates a failure to send a log message to the server due to a connection issue

#### Warning Logs
```
[Warning] Rate limit approaching: 80% of limit reached
```
- Indicates a potential issue that might become a problem if not addressed

### Structured Logging

FurryFriends uses structured logging with Serilog, which means log messages contain structured data that can be queried and analyzed. For example:

```
[Information] {Message: "User logged in", UserId: "123", IPAddress: "192.168.1.1"}
```

This structured format makes it easier to filter and search logs based on specific properties.

## Advanced Troubleshooting

### Enabling Debug Logging Temporarily

To enable more detailed logging for troubleshooting, you can temporarily lower the log level to Debug:

```json
"Serilog": {
  "MinimumLevel": {
    "Default": "Debug",
    "Override": {
      "Microsoft": "Information",
      "System": "Information"
    }
  }
}
```

Remember to revert this change after troubleshooting to avoid excessive logging in production.

### Analyzing Log Files

For large log files, you can use tools like:

1. **grep** (or PowerShell's `Select-String`) to search for specific patterns
   ```
   grep "Error" Logs/log-2023-05-01.txt
   ```

2. **Serilog.Expressions** to filter logs programmatically
   ```csharp
   Log.Logger = new LoggerConfiguration()
       .Filter.ByExcluding("RequestPath like '/health%'")
       .WriteTo.Console()
       .CreateLogger();
   ```

3. **Log aggregation tools** like ELK Stack (Elasticsearch, Logstash, Kibana) or Application Insights for more advanced analysis

### Debugging the Logging Pipeline

If you need to debug the logging pipeline itself, you can add diagnostic logging:

```csharp
Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
```

This will output internal Serilog diagnostic messages to the Debug output window.
