using FurryFriends.UseCases.Domain.PetWalkers.Command.CreatePetWalker;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Create;

public class CreatePetWalker(IMediator _mediator)
  : Endpoint<CreatePetWalkerRequest, Result<CreatePetWalkerResponse>>
{
  public override void Configure()
  {
    Post(CreatePetWalkerRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("CreateUser_" + Guid.NewGuid().ToString())); // Ensure unique name

    Summary(s =>
    {
      s.Summary = "Create a new User";
      s.Description = "Creates a new user by providing a username and email. This endpoint allows anonymous access.";

      s.ExampleRequest = new CreatePetWalkerRequest
      {
        FirstName = "John",
        LastName = "Smith",
        Email = "john.smith@example.com",
        PhoneCountryCode = "1",
        PhoneNumber = "1234567",
        Street = "123 Main St",
        City = "Springfield",
        State = "IL",
        PostalCode = "62701"
      };
    });
  }
  public override async Task HandleAsync(CreatePetWalkerRequest request, CancellationToken cancellationToken)
  {
    var userCommand = CreateCommand(request);

    var result = await _mediator.Send(userCommand, cancellationToken);

    if (result == null || !result.IsSuccess)
    {
      await HandleResultErrorsAsync(result, cancellationToken);
      return;
    }

    Response = new CreatePetWalkerResponse(result.Value.ToString());
  }

  private static CreatePetWalkerCommand CreateCommand(CreatePetWalkerRequest request)
  {
    var userCommand = new CreatePetWalkerCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneCountryCode,
            request.PhoneNumber,
            request.Street,
            request.City,
            request.State,
            request.Country,
            request.PostalCode, // Add this line to fix the error
            request.Gender,
            request.Biography,
            request.DateOfBirth,
            request.HourlyRate,
            request.Currency,
            request.IsActive,
            request.IsVerified,
            request.YearsOfExperience,
            request.HasInsurance,
            request.HasFirstAidCertification,
        request.DailyPetWalkLimit);
    return userCommand;
  }

  private async Task HandleResultErrorsAsync(Result<Guid>? result, CancellationToken cancellationToken)
  {
    if (result?.ValidationErrors?.Any() == true)
    {
      foreach (var error in result.ValidationErrors)
      {
        AddError(error.ErrorMessage);
      }
    }

    if (result?.Errors?.Any() == true)
    {
      foreach (var error in result.Errors)
      {
        AddError(error);
      }
    }

    await SendErrorsAsync(result!.IsSuccess ? StatusCodes.Status500InternalServerError : StatusCodes.Status400BadRequest, cancellationToken);
  }
}
