using FluentValidation;
using FurryFriends.UseCases.Users.CreateUser;

namespace FurryFriends.Web.Endpoints.UserEndpoints.Create;

public class CreateUser(IMediator _mediator)
  : Endpoint<CreateUserRequest, Result<CreateUserResponse>>
{
  public override void Configure()
  {
    Post(CreateUserRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("CreateUser_" + Guid.NewGuid().ToString())); // Ensure unique name

    Summary(s =>
    {
      s.Summary = "Create a new User";
      s.Description = "Creates a new user by providing a username and email. This endpoint allows anonymous access.";

      s.ExampleRequest = new CreateUserRequest
      {
        FirstName = "John",
        LastName = "Smith",
        Email = "john.smith@example.com",
        CountryCode = "1",
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
    var userCommand = new CreateUserCommand(
      request.FirstName,
      request.LastName,
      request.Email,
      request.CountryCode,
      request.Number,
      request.Street,
      request.City,
      request.State,
      request.Country, 
      request.PostalCode);

    var result = await _mediator.Send(userCommand, cancellationToken);

    if(result == null)
    {
      await SendErrorsAsync(StatusCodes.Status500InternalServerError, cancellationToken);
      return;
    }
    if(!result.IsSuccess)
    {
      AddError(result.ValidationErrors.First().ErrorMessage);
      await SendErrorsAsync(StatusCodes.Status400BadRequest,cancellationToken);
      return;
    }

    Response = new CreateUserResponse(result.Value);
  }
}
