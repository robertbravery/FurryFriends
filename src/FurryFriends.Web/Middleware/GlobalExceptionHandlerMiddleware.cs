using System.Net;
using FluentValidation.Results;
using FurryFriends.Web.Endpoints.Base;

namespace FurryFriends.Web.Middleware;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
  private const string ValidationError = "Validation Error";
  private readonly RequestDelegate _next = next;
  private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (ValidationException ex)
    {
      context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
      if(ex.Errors.Any())
      {
        await HandleValidationErrorsToResponse(context.Response, ex.Errors);
        }
      else
      {
        await HandleExceptionAsync(context, ex, StatusCodes.Status400BadRequest);

      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, ex.Message);
      await HandleExceptionAsync(context, ex);
    }
  }

  private async Task HandleValidationErrorsToResponse(HttpResponse httpResponse, IEnumerable<ValidationFailure> errors)
  {
    httpResponse.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    var response = new ResponseBase<object>(
        data: null,
        success: false,
        message: ValidationError,
        errorCode: "DataValidationError",
        errors: errors.Select(e => e.ErrorMessage).ToList()
    );
    _logger.LogError("{ValidatonErrpr}: {Errors}", ValidationError, string.Join(", ", response!.Errors!));
    await httpResponse.HttpContext.Response.WriteAsJsonAsync(response);
  }

  private Task HandleExceptionAsync(HttpContext context, Exception ex, int statusCode = StatusCodes.Status500InternalServerError)
  {
    _logger.LogError(ex, ex.Message);
    context.Response.ContentType = "application/json";
    context.Response.StatusCode = statusCode;

    var response = ResponseBase<object>.FromException(ex);
    return context.Response.WriteAsJsonAsync(response);
  }
}

