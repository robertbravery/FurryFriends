using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Domain.PetWalkers.Command.CreatePetWalker;
using FurryFriends.UseCases.Services.PetWalkerService;
using Moq;

namespace FurryFriends.UnitTests.UseCase.PetWalkerTests;

/// <summary>
/// Pins down the three-state contract for the daily walk limit.
/// <para>
/// A non-nullable int collapsed "not supplied" and "explicitly zero" into the same value,
/// so a form that omitted the field silently wrote 0 and made the walker unbookable
/// (the daily-limit guard rejects when bookingsOnDate &gt;= limit). Nullable makes the
/// caller's intent explicit: null leaves the value alone, a supplied value is applied.
/// </para>
/// </summary>
public class PetWalkerServiceDailyWalkLimitTests
{
  [Fact]
  public async Task CreatePetWalker_WithNullLimit_KeepsTheWalkerDefault()
  {
    // Arrange
    var (service, repository) = CreateService();

    // Act
    var result = await service.CreatePetWalkerAsync(await BuildDtoAsync(null));

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.DailyPetWalkLimit.Should().Be(PetWalker.DefaultDailyPetWalkLimit);
    repository.Verify(r => r.AddAsync(It.IsAny<PetWalker>(), It.IsAny<CancellationToken>()), Times.Once);
  }

  [Fact]
  public async Task CreatePetWalker_WithExplicitLimit_AppliesIt()
  {
    // Arrange
    var (service, _) = CreateService();

    // Act
    var result = await service.CreatePetWalkerAsync(await BuildDtoAsync(3));

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.DailyPetWalkLimit.Should().Be(3);
  }

  [Fact]
  public async Task CreatePetWalker_WithExplicitZero_AppliesZero()
  {
    // Arrange - zero is a legitimate business value ("not taking bookings"), not "unset"
    var (service, _) = CreateService();

    // Act
    var result = await service.CreatePetWalkerAsync(await BuildDtoAsync(0));

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.DailyPetWalkLimit.Should().Be(0);
  }

  [Fact]
  public async Task UpdatePetWalker_WithNullLimit_LeavesTheExistingValueUntouched()
  {
    // Arrange
    var (service, repository) = CreateService();
    var existing = await BuildWalkerAsync();
    existing.UpdateDailyPetWalkLimit(5);

    repository
        .Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
        .ReturnsAsync(existing);

    // Act - mirrors an edit form that does not post the limit field at all
    await service.UpdatePetWalkerAsync(BuildUpdateDto(existing, requestedLimit: null), CancellationToken.None);

    // Assert
    existing.DailyPetWalkLimit.Should().Be(5);
  }

  [Fact]
  public async Task UpdatePetWalker_WithExplicitLimit_AppliesIt()
  {
    // Arrange
    var (service, repository) = CreateService();
    var existing = await BuildWalkerAsync();

    repository
        .Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
        .ReturnsAsync(existing);

    // Act
    await service.UpdatePetWalkerAsync(BuildUpdateDto(existing, requestedLimit: 4), CancellationToken.None);

    // Assert
    existing.DailyPetWalkLimit.Should().Be(4);
  }

  [Fact]
  public async Task UpdatePetWalker_WithExplicitZero_AppliesZero()
  {
    // Arrange
    var (service, repository) = CreateService();
    var existing = await BuildWalkerAsync();
    existing.UpdateDailyPetWalkLimit(9);

    repository
        .Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
        .ReturnsAsync(existing);

    // Act
    await service.UpdatePetWalkerAsync(BuildUpdateDto(existing, requestedLimit: 0), CancellationToken.None);

    // Assert
    existing.DailyPetWalkLimit.Should().Be(0);
  }

  private static (PetWalkerService Service, Mock<IRepository<PetWalker>> Repository) CreateService()
  {
    var repository = new Mock<IRepository<PetWalker>>();

    // No walker with the same email exists, so creation is permitted.
    repository
        .Setup(r => r.AnyAsync(It.IsAny<Ardalis.Specification.ISpecification<PetWalker>>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(false);
    repository
        .Setup(r => r.AddAsync(It.IsAny<PetWalker>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((PetWalker w, CancellationToken _) => w);
    repository
        .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(1);

    return (new PetWalkerService(repository.Object), repository);
  }

  private static async Task<CreatePetWalkerDto> BuildDtoAsync(int? dailyPetWalkLimit)
  {
    var walker = await BuildWalkerAsync();

    return new CreatePetWalkerDto(
        walker.Name,
        walker.Email,
        walker.PhoneNumber,
        walker.Address,
        walker.Gender,
        "Test biography",
        new DateTime(1990, 1, 1),
        IsActive: true,
        IsVerified: false,
        YearsOfExperience: 3,
        HasInsurance: true,
        HasFirstAidCertification: true,
        DailyPetWalkLimit: dailyPetWalkLimit,
        walker.Compensation);
  }

  private static UpdatePetWalkerDto BuildUpdateDto(PetWalker walker, int? requestedLimit) =>
      new(
          walker.Id,
          walker.Name,
          walker.PhoneNumber,
          walker.Address,
          walker.Gender,
          "Updated biography",
          new DateTime(1990, 1, 1),
          IsActive: true,
          IsVerified: false,
          YearsOfExperience: 3,
          HasInsurance: true,
          HasFirstAidCertification: true,
          DailyPetWalkLimit: requestedLimit,
          walker.Compensation);

  private static async Task<PetWalker> BuildWalkerAsync()
  {
    var phoneNumber = (await PhoneNumber.Create("027", "011-123-4567")).Value;
    var walker = PetWalker.Create(
        Name.Create("Test", "Walker"),
        Email.Create($"{Guid.NewGuid():N}@example.com"),
        phoneNumber,
        Address.Create("123 Main St", "Johannesburg", "Gauteng", "2001", "South Africa"));

    walker.UpdateCompensation(Compensation.Create(100m, "USD").Value);

    return walker;
  }
}
