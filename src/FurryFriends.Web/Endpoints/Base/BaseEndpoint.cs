namespace FurryFriends.Web.Endpoints.Base;

// For endpoints without request/response types
public abstract class BaseEndpoint : EndpointWithoutRequest
{
  protected readonly IHttpContextAccessor _httpContextAccessor = default!;

  protected BaseEndpoint()
  {
  }
}

/// <summary>
/// Base class for endpoints with request/response types providing centralized error handling
/// and request processing logic. Eliminates repetitive Result-pattern error handling across endpoints.
/// Handles both Errors (string) and ValidationErrors from Ardalis.Result.
/// </summary>
/// <typeparam name="TRequest">The request DTO type.</typeparam>
/// <typeparam name="TResponse">The response DTO type (without Result wrapper; the base class wraps it).</typeparam>
public abstract class BaseEndpoint<TRequest, TResponse> : Endpoint<TRequest, Result<TResponse>>
    where TRequest : class
    where TResponse : class
{
  protected readonly IMediator _mediator;
  protected readonly Microsoft.Extensions.Logging.ILogger _logger;

  protected BaseEndpoint(IMediator mediator, Microsoft.Extensions.Logging.ILogger logger)
  {
    _mediator = mediator;
    _logger = logger;
  }

  /// <summary>
  /// A descriptive name for the operation, used in logging.
  /// </summary>
  protected abstract string OperationName { get; }

  /// <summary>
  /// Adds all errors from a failed Result to the response.
  /// Handles both Errors (string) and ValidationErrors.
  /// </summary>
  protected void AddResultErrors<T>(Result<T> result)
  {
    if (result.ValidationErrors?.Any() == true)
    {
      foreach (var error in result.ValidationErrors)
        AddError(error.ErrorMessage);
    }

    foreach (var error in result.Errors)
      AddError(error);
  }

  /// <summary>
  /// Adds all errors from a failed non-generic Result to the response.
  /// </summary>
  protected void AddResultErrors(Result result)
  {
    if (result.ValidationErrors?.Any() == true)
    {
      foreach (var error in result.ValidationErrors)
        AddError(error.ErrorMessage);
    }

    foreach (var error in result.Errors)
      AddError(error);
  }

  /// <summary>
  /// Centralized result handler for command/query handlers returning Result{TValue}.
  /// Logs the operation, executes the handler, and on failure handles errors centrally.
  /// On success, maps the result value to the response via <paramref name="successMapper"/>.
  /// </summary>
  protected async Task HandleResultAsync<TValue>(
      Func<CancellationToken, Task<Result<TValue>>> handler,
      Func<TValue, CancellationToken, Task<TResponse>> successMapper,
      CancellationToken cancellationToken)
  {
    _logger.LogInformation("{OperationName} initiated", OperationName);

    var result = await handler(cancellationToken);

    if (!result.IsSuccess)
    {
      AddResultErrors(result);

      switch (result.Status)
      {
        case ResultStatus.NotFound:
          await SendNotFoundAsync(cancellationToken);
          return;
        case ResultStatus.Invalid:
          await SendErrorsAsync(StatusCodes.Status400BadRequest, cancellationToken);
          return;
        default:
          await SendErrorsAsync(StatusCodes.Status400BadRequest, cancellationToken);
          return;
      }
    }

    Response = Result<TResponse>.Success(await successMapper(result.Value, cancellationToken));
  }

  /// <summary>
  /// Overload for handlers returning non-generic Result (e.g., delete operations).
  /// Sends 204 No Content on success instead of mapping a response value.
  /// </summary>
  protected async Task HandleResultAsync(
      Func<CancellationToken, Task<Result>> handler,
      CancellationToken cancellationToken)
  {
    _logger.LogInformation("{OperationName} initiated", OperationName);

    var result = await handler(cancellationToken);

    if (!result.IsSuccess)
    {
      AddResultErrors(result);

      switch (result.Status)
      {
        case ResultStatus.NotFound:
          await SendNotFoundAsync(cancellationToken);
          return;
        case ResultStatus.Invalid:
          await SendErrorsAsync(StatusCodes.Status400BadRequest, cancellationToken);
          return;
        default:
          await SendErrorsAsync(StatusCodes.Status400BadRequest, cancellationToken);
          return;
      }
    }

    await SendNoContentAsync(cancellationToken);
  }
}
