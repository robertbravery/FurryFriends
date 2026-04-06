# AGENTS.md (Debug Mode)

## Log Locations

- **API logs**: `FurryFriends.Web/Logs/web-log-.txt` (rolling daily, 10MB max, 31 days)
- **BlazorUI logs**: `FurryFriends.BlazorUI/Logs/blazorui-log-.txt` (rolling daily, 10MB max, 31 days)
- Both use Serilog with structured logging and OpenTelemetry instrumentation

## Debugging Gotchas

- **Testing environment**: `CustomWebApplicationFactory` sets environment to `"Testing"` which switches email sender to `MimeKitEmailSender` and uses in-memory DB. Check `tests/FurryFriends.FunctionalTests/CustomWebApplicationFactory.cs:12`
- **Database seeding**: Test data includes hardcoded GUIDs (e.g., `locality1.Id = Guid.Parse("929ccaf2-8c74-49bb-b9a0-ce26db0611ab")`). See [`SeedData.cs`](src/FurryFriends.Infrastructure/Data/SeedData.cs:38)
- **Domain events**: Dispatched only after successful `SaveChangesAsync`. If you see missing events, check that `_dispatcher` is not null and `RegisterDomainEvent()` was called.
- **CORS errors**: Policy `"AllowBlazorClient"` must be configured before `UseRouting()` in [`Program.cs`](src/FurryFriends.Web/Program.cs:92). Origins: `https://localhost:7214` and `http://localhost:5185`.
- **Blazor service discovery**: If API calls fail, verify `ApiBaseUrl` configuration or fallback to `http://api/api`. See [`BlazorUI.Program.cs`](src/FurryFriends.BlazorUI/Program.cs:75-77)

## Special Flags

- `dotnet ef database update --project src/FurryFriends.Infrastructure --startup-project src/FurryFriends.Web` - migrations must be applied from Infrastructure project with Web as startup
- `"Testing"` environment disables external email sending and uses in-memory database
