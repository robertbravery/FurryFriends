using FluentValidation;
using FurryFriends.UseCases.Users.UpdateUser;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Update;

public class UpdatePetWalkerHourlyRate(IMediator _mediator)
  : Endpoint<UpdateUserHourlyRateRequest, Result>
{
  public override void Configure()
  {
    Put(UpdateUserHourlyRateRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("UpdateUserHourlyRate_" + Guid.NewGuid().ToString())); // Ensure unique name

    Summary(s =>
    {
      s.Summary = "Update User Hourly Rate";
      s.Description = "Updates the hourly rate of an existing user by providing the user ID, hourly rate, and currency.";

      s.ExampleRequest = new UpdateUserHourlyRateRequest
      {
        UserId = Guid.NewGuid(),
        HourlyRate = 50.0m,
        Currency = "USD"
      };
    });
  }

  public override async Task HandleAsync(UpdateUserHourlyRateRequest request, CancellationToken cancellationToken)
  {
    var updateCommand = new UpdatePetWalkerHourlyRateCommand(
      request.UserId,
      request.HourlyRate,
      request.Currency);

    var result = await _mediator.Send(updateCommand, cancellationToken);

    if (result == null)
    {
      await SendErrorsAsync(StatusCodes.Status500InternalServerError, cancellationToken);
      return;
    }
    if (!result.IsSuccess)
    {
      if (result.ValidationErrors != null && result.ValidationErrors.Any())
      {
        AddError(result.ValidationErrors.First().ErrorMessage);
      }
      else if (result.Errors != null && result.Errors.Any())
      {
        AddError(result.Errors.First());
      }
      await SendErrorsAsync(StatusCodes.Status400BadRequest, cancellationToken);
      return;
    }

    await SendOkAsync(cancellationToken);
  }
}
