using Ardalis.GuardClauses;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Domain.PetWalkers.Command.CreatePetWalker;
using FurryFriends.UseCases.Services.PetWalkerService;
using MediatR;
using Microsoft.Extensions.Logging;
using static FurryFriends.Core.ValueObjects.GenderType;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.UpdatePetWalker;

public class UpdatePetWalkerHandler(
    IPetWalkerService _petWalkerService,
    ILogger<UpdatePetWalkerHandler> _logger
    ) : IRequestHandler<UpdatePetWalkerCommand>
{
  public async Task Handle(UpdatePetWalkerCommand command, CancellationToken cancellationToken)
  {
    Guard.Against.Null(command, nameof(command));
    UpdatePetWalkerDto dto = CreatePetWalkerDto(command);
    _logger.LogInformation($"Updating pet walker with id: {command.PetWalkerId}");
    await _petWalkerService.UpdatePetWalkerAsync(dto, cancellationToken);
  }

  private UpdatePetWalkerDto CreatePetWalkerDto(UpdatePetWalkerCommand request)
  {
    var address = Address.Create(request.Street, request.City, request.State, request.Country, request.ZipCode).Value;
    var phoneNumber = PhoneNumber.Create(request.CountryCode, request.PhoneNumber).Result.Value;
    var name = Name.Create(request.FirstName, request.LastName).Value;
    var gender = GenderType.Create((GenderCategory)request.Gender);
    var updatePetWalkerDto = new UpdatePetWalkerDto(
       request.PetWalkerId,
       name,
       phoneNumber,
       address,
       gender,
       request.Biography ?? string.Empty, // provide a default value if null
       request.DateOfBirth,
       request.IsActive,
       request.IsVerified,
       request.YearsOfExperience,
       request.HasInsurance,
       request.HasFirstAidCertification,
       request.DailyPetWalkLimit,
       Compensation.Create(request.HourlyRate, request.Currency)
       );
    return updatePetWalkerDto;
  }
}
