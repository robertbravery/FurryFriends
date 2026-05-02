using Ardalis.Result;
using Ardalis.SharedKernel;
using FurryFriends.Core.RatingAggregate;
using FurryFriends.UseCases.Rating.GetRatingsForPetWalker;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RatingEntity = FurryFriends.Core.RatingAggregate.Rating;

namespace FurrFriends.UnitTests.UseCases.Rating;

public class GetRatingsForPetWalkerTests
{
    private readonly IRepository<RatingEntity> _repository;
    private readonly ILogger<GetRatingsForPetWalkerHandler> _logger;
    private readonly GetRatingsForPetWalkerHandler _handler;

    public GetRatingsForPetWalkerTests()
    {
        _repository = Substitute.For<IRepository<RatingEntity>>();
        _logger = Substitute.For<ILogger<GetRatingsForPetWalkerHandler>>();
        _handler = new GetRatingsForPetWalkerHandler(_repository, _logger);
    }

    [Fact]
    public async Task Handle_WithRatings_ReturnsPaginatedResults()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var query = new GetRatingsForPetWalkerQuery(petWalkerId, 1, 20);

        var ratings = new List<RatingEntity>
        {
            RatingEntity.Create(petWalkerId, Guid.NewGuid(), 5, "Great!"),
            RatingEntity.Create(petWalkerId, Guid.NewGuid(), 4, "Good"),
            RatingEntity.Create(petWalkerId, Guid.NewGuid(), 3, "Average")
        };

        _repository.ListAsync(Arg.Any<Ardalis.Specification.ISpecification<RatingEntity>>())
            .Returns(ratings);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value.Should().AllSatisfy(dto => dto.PetWalkerId.Should().Be(petWalkerId));
    }

    [Fact]
    public async Task Handle_WithNoRatings_ReturnsEmptyList()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var query = new GetRatingsForPetWalkerQuery(petWalkerId, 1, 20);

        _repository.ListAsync(Arg.Any<Ardalis.Specification.ISpecification<RatingEntity>>())
            .Returns(new List<RatingEntity>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
