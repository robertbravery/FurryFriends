using System.Net;
using FluentValidation.Results;

namespace FurryFriends.Web.Middleware;

public class GlobalExceptionHandlerMiddleware
{
  private readonly RequestDelegate _next;

  public GlobalExceptionHandlerMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task Invoke(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (ValidationException ex) when (ex.Errors.All(e => e.GetType() == typeof(ValidationFailure))) // Filter for standard Fluent Validation failures
    {
      context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
      await WriteValidationErrorsToResponse(context.Response, ex.Errors);
    }
    catch (ValidationException ex) // Handle custom ValidationException from UseCase
    {
      // Convert to desired format (see approach 2)
      await WriteValidationErrorsToResponse(context.Response, ex.Errors);
    }
    catch (Exception ex)
    {
      // Log the exception
      context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
      await context.Response.WriteAsJsonAsync(new { Message = ex.Message });
    }
  }

  private async Task WriteValidationErrorsToResponse(HttpResponse response, IEnumerable<ValidationFailure> errors)
  {
    var apiErrors = errors.Select(f => new
    {
      name = f.PropertyName,
      reason = f.ErrorMessage,
      code = f.ErrorCode // Include if available from Fluent Validation
                         // severity = "Error" // Can be omitted if not relevant
    });

    response.ContentType = "application/json";
    await response.WriteAsJsonAsync(new
    {
      type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1",
      title = "Validation Error",
      status = 400,
      instance = "/Clients", // Adjust if needed
      traceId = Guid.NewGuid().ToString(), // Or use a generated trace ID
      errors = apiErrors
    });
  }

  // Implement WriteCustomValidationErrorsToResponse to convert UseCase errors to the desired format
}

