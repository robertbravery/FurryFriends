using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.PetWalkerAggregate.Specifications;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Domain.PetWalkers.Command.CreatePetWalker;
using FurryFriends.UseCases.Domain.PetWalkers.Query.ListPetWalker;

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
}
