using Ardalis.GuardClauses;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Domain.PetWalkers.Command.CreatePetWalker;
using FurryFriends.UseCases.Domain.PetWalkers.Dto;
using FurryFriends.UseCases.Services.PetWalkerService;
using FurryFriends.UseCases.Services.PictureService;
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

  private UpdatePetWalkerDto CreatePetWalkerDto(UpdatePetWalkerCommand command)
  {
    var address = Address.Create(command.Street, command.City, command.State, command.Country, command.ZipCode).Value;
    var phoneNumber = PhoneNumber.Create(command.CountryCode, command.PhoneNumber).Result.Value;
    var name = Name.Create(command.FirstName, command.LastName).Value;
    var gender = GenderType.Create((GenderCategory)command.Gender);
    var updatePetWalkerDto = new UpdatePetWalkerDto(
       command.PetWalkerId,
       name,
       phoneNumber,
       address,
       gender,
       command.Biography ?? string.Empty, // provide a default value if null
       command.DateOfBirth,
       command.IsActive,
       command.IsVerified,
       command.YearsOfExperience,
       command.HasInsurance,
       command.HasFirstAidCertification,
       command.DailyPetWalkLimit,
       Compensation.Create(command.HourlyRate, command.Currency)
       );
    return updatePetWalkerDto;
  }
}


public class UpdateBioPictureHandler(
    IPictureService _pictureService,
    ILogger<UpdatePetWalkerHandler> _logger
    ) : IRequestHandler<UpdateBioPictureCommand, Result<PhotoDto>>
{
  public async Task<Result<PhotoDto>> Handle(UpdateBioPictureCommand command, CancellationToken cancellationToken)
  {
    Guard.Against.Null(command, nameof(command));

    _logger.LogInformation($"Updating pet walker with id: {command.PetWalkerId}");
    //ToDO: Call the service to update the bio picture
    await _pictureService.UpdatePetWalkerBioPictureAsync(command.PetWalkerId, command.File, cancellationToken);
    return Result.Success();
  }


}
