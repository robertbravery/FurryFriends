using Ardalis.Result;
using FastEndpoints;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Domain.PetWalkers.Command.CreatePetWalker;
using FurryFriends.Web.Endpoints.PetWalkerEndpoints.Create;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace FurryFriends.UnitTests.Web.PetWalkerTests;

public class CreatePetWalkerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<CreatePetWalker>> _loggerMock;
    private readonly CreatePetWalker _handler;
    private readonly CreatePetWalkerRequest _validRequest;

    public CreatePetWalkerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<CreatePetWalker>>();
        _handler = Factory.Create<CreatePetWalker>(_mediatorMock.Object);

        _validRequest = new CreatePetWalkerRequest
        {
            FirstName = "John",
            LastName = "Walker",
            Email = "john.walker@example.com",
            PhoneCountryCode = "+1",
            PhoneNumber = "1234567890",
            Street = "123 Walker St",
            City = "WalkCity",
            State = "WS",
            Country = "USA",
            PostalCode = "12345",
            Gender = GenderType.GenderCategory.Male,
            Biography = "Professional dog walker",
            DateOfBirth = new DateTime(1990, 1, 1),
            HourlyRate = 25.0m,
            Currency = "USD",
            // ServiceArea = "Downtown",
            YearsOfExperience = 5
        };
    }

    [Fact]
    public async Task HandleAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var result = Result<Guid>.Success(expectedId);
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePetWalkerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        await _handler.HandleAsync(_validRequest, CancellationToken.None);

        // Assert
        _handler.Response.IsSuccess.Should().BeTrue();
        _handler.Response.Value.Data.Should().Be(expectedId.ToString());
    }

    [Fact]
    public async Task HandleAsync_DuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var result = Result<Guid>.Error("Email already exists");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePetWalkerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        await _handler.HandleAsync(_validRequest, CancellationToken.None);

        // Assert
        _handler.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        _handler.ValidationFailures.Should().NotBeEmpty();
    }

    // [Theory]
    // [InlineData("")]
    // [InlineData(" ")]
    // [InlineData("TooLargeArea1234567890123456789012345678901234567890")]
    // public async Task HandleAsync_InvalidServiceArea_ReturnsBadRequest(string invalidServiceArea)
    // {
    //     // Arrange
    //     var request = _validRequest;
    //     request.ServiceArea = invalidServiceArea;

    //     var result = Result<Guid>.Error("Invalid service area");
    //     _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePetWalkerCommand>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(result);

    //     // Act
    //     await _handler.HandleAsync(request, CancellationToken.None);

    //     // Assert
    //     _handler.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    //     _handler.ValidationFailures.Should().NotBeEmpty();
    // }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(51)]
    public async Task HandleAsync_InvalidExperienceYears_ReturnsBadRequest(int years)
    {
        // Arrange
        var request = _validRequest;
        request.YearsOfExperience = years;

        var result = Result<Guid>.Error("Invalid years of experience");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePetWalkerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        _handler.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        _handler.ValidationFailures.Should().NotBeEmpty();
    }
}