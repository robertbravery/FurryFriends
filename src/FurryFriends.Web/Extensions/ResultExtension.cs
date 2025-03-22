// Extensions/ResultExtensions.cs
namespace FurryFriends.Web.Extensions;

public static class ResultExtensions
{
  public static ProblemDetails ToProblemDetails(this Result result, string? title = null)
  {
    int statusCode;
    string detail;
    string problemTitle;

    switch (result.Status)
    {
      case ResultStatus.NotFound:
        statusCode = StatusCodes.Status404NotFound;
        detail = "Resource not found.";
        problemTitle = title ?? "Not Found";
        break;
      case ResultStatus.Invalid:
        statusCode = StatusCodes.Status400BadRequest;
        detail = string.Join(Environment.NewLine, result.ValidationErrors.Select(e => $"{e.Identifier}: {e.ErrorMessage}")); // Combine validation errors
        problemTitle = title ?? "Invalid Request";
        break;
      case ResultStatus.Unauthorized:
        statusCode = StatusCodes.Status401Unauthorized;
        detail = "Unauthorized access.";
        problemTitle = title ?? "Unauthorized";
        break;
      case ResultStatus.Forbidden:
        statusCode = StatusCodes.Status403Forbidden;
        detail = "Forbidden access.";
        problemTitle = title ?? "Forbidden";
        break;
      case ResultStatus.Error:
      default:
        statusCode = StatusCodes.Status500InternalServerError;
        detail = "An unexpected error occurred.";
        problemTitle = title ?? "Server Error";
        break;
    }

    return new ProblemDetails
    {
      Detail = detail,
      Status = statusCode,
      //Title
    };
  }

  public static ProblemDetails ToProblemDetails<T>(this Result<T> result, string? title = null)
  {
    int statusCode;
    string detail;
    string problemTitle;

    switch (result.Status)
    {
      case ResultStatus.NotFound:
        statusCode = StatusCodes.Status404NotFound;
        detail = "Resource not found.";
        problemTitle = title ?? "Not Found";
        break;
      case ResultStatus.Invalid:
        statusCode = StatusCodes.Status400BadRequest;
        detail = string.Join(Environment.NewLine, result.ValidationErrors.Select(e => $"{e.Identifier}: {e.ErrorMessage}")); // Combine validation errors
        problemTitle = title ?? "Invalid Request";
        break;
      case ResultStatus.Unauthorized:
        statusCode = StatusCodes.Status401Unauthorized;
        detail = "Unauthorized access.";
        problemTitle = title ?? "Unauthorized";
        break;
      case ResultStatus.Forbidden:
        statusCode = StatusCodes.Status403Forbidden;
        detail = "Forbidden access.";
        problemTitle = title ?? "Forbidden";
        break;
      case ResultStatus.Error:
      default:
        statusCode = StatusCodes.Status500InternalServerError;
        detail = "An unexpected error occurred.";
        problemTitle = title ?? "Server Error";
        break;
    }

    return new ProblemDetails
    {
      Detail = detail,
      Status = statusCode,
      //Title = problemTitle
    };
  }

  //Overloaded with specific message
  public static ProblemDetails ToProblemDetails(this Result result, string detailMessage, string? title = null)
  {
    int statusCode;
    string problemTitle;

    switch (result.Status)
    {
      case ResultStatus.NotFound:
        statusCode = StatusCodes.Status404NotFound;
        problemTitle = title ?? "Not Found";
        break;
      case ResultStatus.Invalid:
        statusCode = StatusCodes.Status400BadRequest;
        problemTitle = title ?? "Invalid Request";
        break;
      case ResultStatus.Unauthorized:
        statusCode = StatusCodes.Status401Unauthorized;
        problemTitle = title ?? "Unauthorized";
        break;
      case ResultStatus.Forbidden:
        statusCode = StatusCodes.Status403Forbidden;
        problemTitle = title ?? "Forbidden";
        break;
      case ResultStatus.Error:
      default:
        statusCode = StatusCodes.Status500InternalServerError;
        problemTitle = title ?? "Server Error";
        break;
    }

    return new ProblemDetails()
    {
      Detail = detailMessage,
      Status = statusCode,
      //Title = problemTitle
    };
  }
}
