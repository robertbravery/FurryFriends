﻿using Ardalis.Result;
using FastEndpoints;
using FurryFriends.Core.ClientAggregate.Enums;
using FurryFriends.UseCases.Domain.Clients.Command.CreateClient;
using FurryFriends.Web.Endpoints.ClientEndpoints.Create;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace FurryFriends.UnitTests.Web.ClientTests;

public class CreateClientTests
{
  private readonly Mock<IMediator> _mediatorMock;
  private readonly Mock<ILogger<CreateClient>> _loggerMock;
  private readonly CreateClient _createClient;
  private readonly CreateClientRequest _validRequest;

  public CreateClientTests()
  {
    _mediatorMock = new Mock<IMediator>();
    _loggerMock = new Mock<ILogger<CreateClient>>();
    _createClient = Factory.Create<CreateClient>(_mediatorMock.Object, _loggerMock.Object);
    
    _validRequest = new CreateClientRequest
    {
      FirstName = "John",
      LastName = "Doe",
      Email = "john.doe@example.com",
      PhoneCountryCode = "+1",
      PhoneNumber = "1234567890",
      Street = "123 Main St",
      City = "Anytown",
      State = "State",
      Country = "Country",
      ZipCode = "12345",
      ClientType = ClientType.Regular,
      PreferredContactTime = new TimeOnly(9, 0),
      ReferralSource = ReferralSource.Website
    };
  }

  [Fact]
  public async Task HandleAsync_CallMediatorSendSuccess()
  {
    // Arrange
    var request = new CreateClientRequest
    {
      FirstName = "John",
      LastName = "Doe",
      Email = "john.doe@example.com",
      PhoneCountryCode = "+1",
      PhoneNumber = "1234567890",
      Street = "123 Main St",
      City = "Anytown",
      State = "Anystate",
      Country = "USA",
      ZipCode = "12345",
      ClientType = ClientType.Regular,
      PreferredContactTime = new TimeOnly(9, 0),
      ReferralSource = ReferralSource.Website
    };

    var result = Result<Guid>.Success(Guid.NewGuid());
    _mediatorMock.Setup(m => m.Send(It.IsAny<CreateClientCommand>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(result);

    // Act
    await _createClient.HandleAsync(request, CancellationToken.None);

    // Assert
    _createClient.Response.IsSuccess.Should().BeTrue();
    _mediatorMock.Verify(m => m.Send(It.IsAny<CreateClientCommand>(), It.IsAny<CancellationToken>()));
  }

  [Fact]
  public async Task HandleAsync_ShouldReturnErrors_WhenResultIsNotSuccess()
  {
    // Arrange
    var request = new CreateClientRequest
    {
      FirstName = "John",
      LastName = "Doe",
      Email = "john.doe@example.com",
      PhoneCountryCode = "+1",
      PhoneNumber = "1234567890",
      Street = "123 Main St",
      City = "Anytown",
      State = "Anystate",
      Country = "USA",
      ZipCode = "12345",
      ClientType = ClientType.Regular,
      PreferredContactTime = new TimeOnly(9, 0),
      ReferralSource = ReferralSource.Website
    };

    var result = Result<Guid>.NotFound(new[] { "Error1", "Error2" });
    _mediatorMock.Setup(m => m.Send(It.IsAny<CreateClientCommand>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(result);

    // Act
    await _createClient.HandleAsync(request, CancellationToken.None);

    // Assert
    _createClient.Response.IsSuccess.Should().BeFalse();
  }

  [Fact]
  public async Task HandleAsync_ShouldReturnClientId_WhenResultIsSuccess()
  {
    // Arrange
    var request = new CreateClientRequest
    {
      FirstName = "John",
      LastName = "Doe",
      Email = "john.doe@example.com",
      PhoneCountryCode = "+1",
      PhoneNumber = "1234567890",
      Street = "123 Main St",
      City = "Anytown",
      State = "Anystate",
      Country = "USA",
      ZipCode = "12345",
      ClientType = ClientType.Regular,
      PreferredContactTime = new TimeOnly(9, 0),
      ReferralSource = ReferralSource.Website
    };

    var clientId = Guid.NewGuid();
    var result = Result<Guid>.Success(clientId);
    _mediatorMock.Setup(m => m.Send(It.IsAny<CreateClientCommand>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(result);

    // Act
    await _createClient.HandleAsync(request, CancellationToken.None);

    // Assert
    Assert.Equal(clientId.ToString(), _createClient.Response.Value.ClientId);
  }

  [Fact]
  public async Task HandleAsync_InvalidRequest_ReturnsBadRequest()
  {
    // Arrange
    var request = new CreateClientRequest
    {
      FirstName = "", // Invalid: FirstName is required
      LastName = "Doe",
      Email = "john.doe@example.com",
      PhoneCountryCode = "1",
      PhoneNumber = "1234567890",
      Street = "123 Main St",
      City = "Anytown",
      State = "CA",
      Country = "USA",
      ZipCode = "12345",
      PreferredContactTime = new TimeOnly(9, 0),
      ReferralSource = ReferralSource.Website
    };

    var expectedError = "FirstName is required";
    var commandResult = Result<Guid>.Error(expectedError);
    _mediatorMock
        .Setup(m => m.Send(It.IsAny<CreateClientCommand>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(commandResult);

    // Act
    await _createClient.HandleAsync(request, CancellationToken.None);

    // Assert
    _createClient.Response.Value.Should().BeNull();
    _createClient.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    _createClient.ValidationFailures.Should().ContainSingle();
    _createClient.ValidationFailures.First().ErrorMessage.Should().Be(expectedError);
  }

  [Fact]
  public async Task HandleAsync_MediatorThrowsException_ReturnsInternalServerError()
  {
    // Arrange
    var request = new CreateClientRequest
    {
      FirstName = "John",
      LastName = "Doe",
      Email = "john.doe@example.com",
      PhoneCountryCode = "1",
      PhoneNumber = "1234567890",
      Street = "123 Main St",
      City = "Anytown",
      State = "CA",
      Country = "USA",
      ZipCode = "12345",
      ClientType = ClientType.Regular,
      PreferredContactTime = new TimeOnly(9, 0),
      ReferralSource = ReferralSource.Other
    };

    _mediatorMock
        .Setup(m => m.Send(It.IsAny<CreateClientCommand>(), It.IsAny<CancellationToken>()))
        .ThrowsAsync(new Exception("Mediator error"));

    // Act
    Func<Task> act = async () => await _createClient.HandleAsync(request, CancellationToken.None);

    // Assert
    await act.Should().ThrowAsync<Exception>().WithMessage("Mediator error");
  }

  [Fact]
  public async Task HandleAsync_WithNullRequest_ThrowsArgumentNullException()
  {
    // Act & Assert
    await Assert.ThrowsAsync<ArgumentNullException>(() => 
        _createClient.HandleAsync(null!, CancellationToken.None));
  }

  [Theory]
  [InlineData("invalid-email")]
  [InlineData("@invalid.com")]
  [InlineData("invalid@")]
  public async Task HandleAsync_WithInvalidEmail_ReturnsBadRequest(string invalidEmail)
  {
    // Arrange
    var request = _validRequest;
    request.Email = invalidEmail;

    var result = Result<Guid>.Error("Invalid email format");
    _mediatorMock.Setup(m => m.Send(It.IsAny<CreateClientCommand>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(result);

    // Act
    await _createClient.HandleAsync(request, CancellationToken.None);

    // Assert
    _createClient.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    _createClient.ValidationFailures.Should().NotBeEmpty();
  }

  [Theory]
  [InlineData("")]
  [InlineData("+")]
  [InlineData("++1")]
  public async Task HandleAsync_WithInvalidPhoneNumber_ReturnsBadRequest(string invalidPhoneCode)
  {
    // Arrange
    var request = _validRequest;
    request.PhoneCountryCode = invalidPhoneCode;

    var result = Result<Guid>.Error("Invalid phone country code");
    _mediatorMock.Setup(m => m.Send(It.IsAny<CreateClientCommand>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(result);

    // Act
    await _createClient.HandleAsync(request, CancellationToken.None);

    // Assert
    _createClient.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    _createClient.ValidationFailures.Should().NotBeEmpty();
  }

  [Fact]
  public async Task HandleAsync_WhenMediatorFails_ThrowsException()
  {
    // Arrange
    _mediatorMock.Setup(m => m.Send(It.IsAny<CreateClientCommand>(), It.IsAny<CancellationToken>()))
        .ThrowsAsync(new Exception("Unexpected error"));

    // Act
    Func<Task> act = async () => await _createClient.HandleAsync(_validRequest, CancellationToken.None);

    // Assert
    await act.Should().ThrowAsync<Exception>().WithMessage("Unexpected error");
  }

  // [Fact]
  // public void Configure_SetsCorrectRouteAndMethod()
  // {
  //   // Arrange
  //   var endpoint = new CreateClient(_mediatorMock.Object, _loggerMock.Object);

  //   // Act
  //   endpoint.Configure();

  //   // Assert
  //   // Verify the configuration using reflection or endpoint properties
  //   endpoint.Routes.Should().NotBeEmpty();
  //   endpoint.Routes.First().HttpMethod.Should().Be("POST");
  // }
}
