using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Domain.PetWalkers.Command.CreatePetWalker;
using FurryFriends.UseCases.Domain.PetWalkers.Query.ListPetWalker;
//using Microsoft.EntityFrameworkCore


namespace FurryFriends.UseCases.Services.PetWalkerService;

public class PetWalkerService : IPetWalkerService
{
  private readonly IRepository<PetWalker> _repository;

  public PetWalkerService(IRepository<PetWalker> repository)
  {
    _repository = repository;
  }

  public async Task AddBioPictureAsync(Photo photo, Guid userId)
  {
    var user = await _repository.GetByIdAsync(userId)
      ?? throw new InvalidOperationException("User not found.");

    // Update the user's bio picture
    user.AddPhoto(photo);

    // Save changes to the repository
    await _repository.UpdateAsync(user);

    // Optionally, you can fire a domain event or log the update
  }

  public async Task<Result<PetWalker>> CreatePetWalkerAsync(CreatePetWalkerDto dto)
  {
    // 1. Business Rules
    var existingPetWalkerSpec = new GetPetWalkerByEmailSpecification(dto.Email.EmailAddress);
    if (await _repository.AnyAsync(existingPetWalkerSpec))
    {
      return Result.Error("Email already exists");
    }

    // 2. Entity Creation
    var petWalker = PetWalker.Create(dto.Name, dto.Email, dto.PhoneNumber, dto.Address);

    // Entity Configuration
    petWalker.UpdateGender(dto.Gender);
    petWalker.UpdateBiography(dto.Biography);
    petWalker.UpdateDateOfBirth(dto.DateOfBirth);
    petWalker.UpdateIsActive(dto.IsActive);
    petWalker.UpdateIsVerified(dto.IsVerified);
    petWalker.UpdateYearsOfExperience(dto.YearsOfExperience);
    petWalker.UpdateHasInsurance(dto.HasInsurance);
    petWalker.UpdateHasFirstAidCertification(dto.HasFirstAidCertification);
    petWalker.UpdateDailyPetWalkLimit(dto.DailyPetWalkLimit);
    petWalker.UpdateCompensation(dto.Compensation);

    // 4. Persistence
    await _repository.AddAsync(petWalker);
    await _repository.SaveChangesAsync();

    return Result.Success(petWalker);
  }

  public async Task<Result<PetWalker>> UpdatePetWalkerAsync(UpdatePetWalkerDto dto, CancellationToken cancellationToken)
  {
    // 1. Business Rules
    var petWalker = await _repository.GetByIdAsync(dto.Id, cancellationToken);
    if (petWalker is null)
    {
      return Result.Error("Petwalker does not exist");
    }

    // 2 Entity Configuration
    petWalker.UpdateUsername(dto.Name);
    petWalker.UpdatePhoneNumber(dto.PhoneNumber.CountryCode, dto.PhoneNumber.Number);
    petWalker.UpdateAddress(dto.Address);
    petWalker.UpdateGender(dto.Gender);
    petWalker.UpdateBiography(dto.Biography);
    petWalker.UpdateDateOfBirth(dto.DateOfBirth);
    petWalker.UpdateIsActive(dto.IsActive);
    petWalker.UpdateIsVerified(dto.IsVerified);
    petWalker.UpdateYearsOfExperience(dto.YearsOfExperience);
    petWalker.UpdateHasInsurance(dto.HasInsurance);
    petWalker.UpdateHasFirstAidCertification(dto.HasFirstAidCertification);
    petWalker.UpdateDailyPetWalkLimit(dto.DailyPetWalkLimit);
    petWalker.UpdateCompensation(dto.Compensation);
    petWalker.UpdatedAt = DateTime.UtcNow;
    // 4. Persistence
    await _repository.SaveChangesAsync(cancellationToken);

    return Result.Success(petWalker);
  }

  public async Task<Result<PetWalker>> GetPetWalkerByIdAsync(Guid id, bool isAsNoTracking = false, CancellationToken cancellationToken = default)
  {
    try
    {
      var spec = new GetPetWalkerByIdSpecification(id, isAsNoTracking);
      var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
      if (entity == null)
        return Result.NotFound($"Pet walker with ID {id} not found");

      return entity;
    }
    catch (Exception ex)
    {
      throw new Exception($"Error getting pet walker by ID: {ex.Message}");
    }
  }

  public async Task<Result<PetWalker>> GetPetWalkerByEmailAsync(string email, CancellationToken cancellationToken)
  {
    var spec = new GetPetWalkerByEmailSpecification(email);
    try
    {
      var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
      if (entity == null)
        return Result.NotFound("User Not Found");

      return entity;
    }
    catch (Exception ex)
    {
      throw new Exception(ex.Message);
    }
  }

  public async Task<Result<PetWalkerListDto>> ListPetWalkersAsync(ListPetWalkerQuery query)
  {
    var spec = new ListPetWalkerSpecification(query.SearchString, query.PageNumber, query.PageSize);
    var users = await _repository.ListAsync(spec);
    var totalCount = await _repository.CountAsync(spec);
    return new PetWalkerListDto(users, totalCount);
  }

  public async Task<Result<PetWalkerListDto>> ListPetWalkersByLocationAsync(ListPetWalkerByLocationQuery query)
  {
    var spec = new ListPetWalkerByLocationSpecification(query.SearchString, query.Location, query.PageNumber, query.PageSize);
    var users = await _repository.ListAsync(spec);
    var totalCount = await _repository.CountAsync(spec);
    return new PetWalkerListDto(users, totalCount);
  }

  public async Task<Result> UpdatePetWalkerHourlyRateAsync(Guid userId, decimal hourlyRate, string currency, CancellationToken cancellationToken)
  {
    var user = await _repository.GetByIdAsync(userId, cancellationToken);
    if (user == null)
    {
      return Result.Error("User not found.");
    }
    var compensation = Compensation.Create(hourlyRate, currency);
    user.UpdateCompensation(compensation);
    await _repository.UpdateAsync(user, cancellationToken);

    return Result.Success();
  }


  public async Task<Result> RemoveServiceAreaAsync(Guid petWalkerId, Guid serviceAreaId, CancellationToken cancellationToken = default)
  {
    try
    {
      // Get the pet walker with service areas included
      var spec = new GetPetWalkerByIdSpecification(petWalkerId);
      var petWalker = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
      if (petWalker == null)
      {
        return Result.Error("Pet walker not found.");
      }

      // Find the service area
      var serviceArea = petWalker.ServiceAreas.FirstOrDefault(sa => sa.Id == serviceAreaId);
      if (serviceArea == null)
      {
        return Result.Error("Service area not found.");
      }

      // Remove the service area from the pet walker's collection
      petWalker.RemoveServiceArea(serviceArea);


      // Save changes
      await _repository.SaveChangesAsync(cancellationToken);

      return Result.Success();
    }
    catch (Exception ex)
    {
      return Result.Error($"Failed to remove service area: {ex.Message}");
    }
  }
}
