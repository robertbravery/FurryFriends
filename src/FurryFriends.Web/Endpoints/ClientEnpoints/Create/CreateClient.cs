using FurryFriends.UseCases.Clients.CreateClient;

namespace FurryFriends.Web.Endpoints.ClientEnpoints.Create;

public class CreateClient : Endpoint<CreateClientRequest, Result<CreateClientReponse>>
{
  private readonly IMediator _mediator;
  private readonly ILogger<CreateClient> _logger;

  public CreateClient(IMediator mediator, ILogger<CreateClient> logger)
  {
    _mediator = mediator;
    _logger = logger;
  }

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
    _logger.LogInformation("Received create client request for {ClientName}", request);

    var command = new CreateClientCommand(
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

    var result = await _mediator.Send(command, cancellationToken);

    if (!result.IsSuccess)
    {
      foreach (var error in result.Errors)
      {
        AddError(error);
      }
      Response = Result.Error();
      await SendErrorsAsync(cancellation: cancellationToken);
      return;
    }

    Response = new CreateClientReponse(result.Value.ToString());
  }
}

