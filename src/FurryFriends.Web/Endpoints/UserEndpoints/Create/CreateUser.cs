using Ardalis.GuardClauses;
using FluentValidation;
using FurryFriends.UseCases.Users.Create;

namespace FurryFriends.Web.Endpoints.UserEndpoints.Create;

public class CreateUser(IMediator _mediator, IValidator<CreateUserRequest> _validator)
  : Endpoint<CreateUserRequest, Result<CreateUserResponse>>
{
  public override void Configure()
  {
    Post(CreateUserRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Create a new User";
      s.Description = "Creates a new user by providing a username and email. This endpoint allows anonymous access.";

      s.ExampleRequest = new CreateUserRequest
      {
        Name = "John Smith",
        Email = "john.smith@example.com",
        CountryCode = "1",
        AreaCode = "555",
        Number = "1234567",
        Street = "123 Main St",
        City = "Springfield",
        State = "IL",
        PostalCode = "62701"
      };
    });
  }
  public override async Task HandleAsync(CreateUserRequest request, CancellationToken cancellationToken)
  {
    Guard(request);
    var result = await _validator.ValidateAsync(request);

    if (!result.IsValid)
    {
      Response = Result.Error(result.ToString());


      return;
    }

    var userCommand = new CreateUserCommand(
      request.Name,
      request.Email,
      request.CountryCode,
      request.AreaCode,
      request.Number,
      request.Street,
      request.City,
      request.State,
      request.PostalCode);

    await _mediator.Send(userCommand, cancellationToken);

    if (result.IsValid)
    {
      Response = new CreateUserResponse();
      return;
    }


  }

  private static void Guard(CreateUserRequest request)
  {
    Ardalis.GuardClauses.Guard.Against.NullOrEmpty(request.Name, nameof(request.Name), "Name cannot be null or empty");
    Ardalis.GuardClauses.Guard.Against.NullOrEmpty(request.Email, nameof(request.Email), "Email cannot be null or empty");
    Ardalis.GuardClauses.Guard.Against.NullOrEmpty(request.CountryCode, nameof(request.CountryCode), "Country code cannot be null or empty");
    Ardalis.GuardClauses.Guard.Against.NullOrEmpty(request.AreaCode, nameof(request.AreaCode), "Area code cannot be null or empty");
    Ardalis.GuardClauses.Guard.Against.NullOrEmpty(request.Number, nameof(request.Number), "Number cannot be null or empty");
    Ardalis.GuardClauses.Guard.Against.NullOrEmpty(request.Street, nameof(request.Street), "Street cannot be null or empty");
    Ardalis.GuardClauses.Guard.Against.NullOrEmpty(request.City, nameof(request.City), "City cannot be null or empty");
    Ardalis.GuardClauses.Guard.Against.NullOrEmpty(request.State, nameof(request.State), "State cannot be null or empty");
    Ardalis.GuardClauses.Guard.Against.NullOrEmpty(request.PostalCode, nameof(request.PostalCode), "Zip code cannot be null or empty");
  }
}
