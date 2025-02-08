using System.Net;
using FluentValidation.Results;
using FurryFriends.Web.Endpoints.Base;

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
      await HandleValidationErrorsToResponse(context.Response, ex.Errors);
    }
    catch (ValidationException ex) // Handle custom ValidationException from UseCase
    {
      await HandleValidationErrorsToResponse(context.Response, ex.Errors);
    }
    catch (Exception ex)
    {
      // Log the exception
      await HandleExceptionAsync(context, ex);
    }
  }

  private async Task HandleValidationErrorsToResponse(HttpResponse response, IEnumerable<ValidationFailure> errors)
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

  private Task HandleExceptionAsync(HttpContext context, Exception ex)
  {
    context.Response.ContentType = "application/json";
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

    var response = ResponseBase<object>.FromException(ex);
    return context.Response.WriteAsJsonAsync(response);
  }
}

