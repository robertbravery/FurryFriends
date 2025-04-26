using FurryFriends.UseCases.Domain.PetWalkers.Command.UpdatePetWalker;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Update;

public class UpdatePetWalker(IMediator mediator) : Endpoint<UpdatePetWalkerRequest, UpdatePetWalkerResponse, PetWalkerResponseMapper>
{
  private readonly IMediator _mediator = mediator;

  public override void Configure()
  {
    Put(UpdatePetWalkerRequest.Route);
    AllowAnonymous();
    Options(x => x.WithName("UpdatePetWalker_" + Guid.NewGuid().ToString()));
    Description(d => d
        .Produces<UpdatePetWalkerResponse>(200)
        .Produces(400)
        .WithTags("Petwalker"));
  }

  public override async Task HandleAsync(UpdatePetWalkerRequest req, CancellationToken ct)
  {
    var command = new UpdatePetWalkerCommand
    {
      PetWalkerId = req.PetWalkerId,
      FirstName = req.FirstName,
      LastName = req.LastName,
      CountryCode = req.CountryCode,
      PhoneNumber = req.PhoneNumber,
      Street = req.Street,
      City = req.City,
      State = req.State,
      ZipCode = req.ZipCode,
      Country = req.Country,
      Biography = req.Biography,
      DateOfBirth = req.DateOfBirth,
      Gender = req.Gender,
      HourlyRate = req.HourlyRate,
      Currency = req.Currency,
      IsActive = req.IsActive,
      IsVerified = req.IsVerified,
      YearsOfExperience = req.YearsOfExperience,
      HasInsurance = req.HasInsurance,
      HasFirstAidCertification = req.HasFirstAidCertification,
      DailyPetWalkLimit = req.DailyPetWalkLimit,
      ServiceLocation = req.ServiceLocation,
      //BioPicture = req.BioPicture,
      //Photos = req.Photos
    };

    await _mediator.Send(command, ct);

    await SendAsync(new UpdatePetWalkerResponse { Id = req.PetWalkerId }, 200, ct);
  }
}
