using Ardalis.Result;
using Ardalis.SharedKernel;
using FurryFriends.Core.RatingAggregate;
using FurryFriends.UseCases.Rating.GetPetWalkerRatingSummary;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RatingEntity = FurryFriends.Core.RatingAggregate.Rating;

namespace FurrFriends.UnitTests.UseCases.Rating;

public class GetPetWalkerRatingSummaryTests
{
    private readonly IRepository<RatingEntity> _repository;
    private readonly ILogger<GetPetWalkerRatingSummaryHandler> _logger;
    private readonly GetPetWalkerRatingSummaryHandler _handler;

    public GetPetWalkerRatingSummaryTests()
    {
        _repository = Substitute.For<IRepository<RatingEntity>>();
        _logger = Substitute.For<ILogger<GetPetWalkerRatingSummaryHandler>>();
        _handler = new GetPetWalkerRatingSummaryHandler(_repository, _logger);
    }

    [Fact]
    public async Task Handle_WithRatings_ReturnsSummary()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var query = new GetPetWalkerRatingSummaryQuery(petWalkerId);

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
        result.Value.PetWalkerId.Should().Be(petWalkerId);
        result.Value.AverageRating.Should().Be(4.0);
        result.Value.TotalRatings.Should().Be(3);
        result.Value.RecentRatings.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WithNoRatings_ReturnsNullAverage()
    {
        // Arrange
        var petWalkerId = Guid.NewGuid();
        var query = new GetPetWalkerRatingSummaryQuery(petWalkerId);

        _repository.ListAsync(Arg.Any<Ardalis.Specification.ISpecification<RatingEntity>>())
            .Returns(new List<RatingEntity>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PetWalkerId.Should().Be(petWalkerId);
        result.Value.AverageRating.Should().Be(0);
        result.Value.TotalRatings.Should().Be(0);
        result.Value.RecentRatings.Should().BeEmpty();
    }
}
