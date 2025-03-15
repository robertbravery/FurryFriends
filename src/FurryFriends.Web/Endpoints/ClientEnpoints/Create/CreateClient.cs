using FurryFriends.UseCases.Domain.Clients.Command.CreateClient;
using Ardalis.GuardClauses;

namespace FurryFriends.Web.Endpoints.ClientEnpoints.Create;

public class CreateClient(IMediator mediator, ILogger<CreateClient> logger) 
: Endpoint<CreateClientRequest, Result<CreateClientReponse>>
{
  private readonly IMediator _mediator = mediator;
  private readonly ILogger<CreateClient> _logger = logger;

  public override void Configure()
  {
    Post(CreateClientRequest.Route);
    AllowAnonymous();
    Options(x => x.WithName("CreateClient_" + Guid.NewGuid().ToString()));
    Description(d => d
        .Produces<Result<int>>(201)
        .Produces(400)
        .WithTags("Clients"));
  }

  public override async Task HandleAsync(CreateClientRequest request, CancellationToken cancellationToken)
  {
    Guard.Against.Null(request, nameof(request));
    LogInformation(request);

    var command = CreateCommand(request);

    var result = await _mediator.Send(command, cancellationToken);

    if (!result.IsSuccess)
    {
      foreach (var error in result.Errors)
      {
        AddError(error);
      }
      LogError(request, result);
      Response = Result.Error();
      await SendErrorsAsync(result!.IsSuccess ? StatusCodes.Status500InternalServerError : StatusCodes.Status400BadRequest, cancellationToken);
      return;
    }

    Response = new CreateClientReponse(result.Value.ToString());
  }

  private void LogError(CreateClientRequest request, Result<Guid> result)
  {
    _logger.LogError(
              "Failed to create client. Email: {Email}, Errors: {Errors}",
              request.Email,
              string.Join(", ", result.Errors));
  }

  private void LogInformation(CreateClientRequest request)
  {
    _logger.LogInformation(
                "Creating new client. FirstName: {FirstName}, LastName: {LastName}, Email: {Email}",
                request.FirstName,
                request.LastName,
                request.Email);
  }

  private static CreateClientCommand CreateCommand(CreateClientRequest request)
  {
    return new CreateClientCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneCountryCode,
            request.PhoneNumber,
            request.Street,
            request.City,
            request.State,
            request.Country,
            request.ZipCode,
            request.ClientType,
            request.PreferredContactTime,
            request.ReferralSource);
  }
}

