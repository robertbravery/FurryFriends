using Ardalis.Result;
using FastEndpoints;
using FurryFriends.Web.Endpoints.Base;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FurryFriends.UnitTests.Web.Base;

public class TestRequest
{
    public string? Name { get; set; }
}

public class TestResponse
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}

/// <summary>
/// Concrete test endpoint that exposes protected base methods for testing.
/// </summary>
public class TestEndpoint : BaseEndpoint<TestRequest, TestResponse>
{
    public TestEndpoint(IMediator mediator, ILogger<TestEndpoint> logger)
        : base(mediator, logger) { }

    protected override string OperationName => "TestOperation";

    public override void Configure()
    {
        Get("/test");
        AllowAnonymous();
    }

    public override async Task HandleAsync(TestRequest request, CancellationToken cancellationToken)
    {
        await HandleResultAsync(
            ct => _mediator.Send(new TestCommand(), ct),
            (TestResultValue value, CancellationToken ct) =>
                Task.FromResult(new TestResponse { Id = value.Id, Name = request.Name }),
            cancellationToken);
    }

    /// <summary>
    /// Exposed wrapper for testing AddResultErrors with generic Result
    /// </summary>
    public void TestAddResultErrors<T>(Result<T> result)
    {
        AddResultErrors(result);
    }

    /// <summary>
    /// Exposed wrapper for testing AddResultErrors with non-generic Result
    /// </summary>
    public void TestAddResultErrors(Result result)
    {
        AddResultErrors(result);
    }
}

/// <summary>
/// Concrete test endpoint for non-generic HandleResultAsync (delete-style operations).
/// </summary>
public class TestDeleteEndpoint : BaseEndpoint<TestRequest, TestResponse>
{
    public TestDeleteEndpoint(IMediator mediator, ILogger<TestDeleteEndpoint> logger)
        : base(mediator, logger) { }

    protected override string OperationName => "TestDeleteOperation";

    public override void Configure()
    {
        Delete("/test");
        AllowAnonymous();
    }

    public override async Task HandleAsync(TestRequest request, CancellationToken cancellationToken)
    {
        await HandleResultAsync(
            ct => _mediator.Send(new TestDeleteCommand(), ct),
            cancellationToken);
    }
}

public class TestCommand : IRequest<Result<TestResultValue>>
{
}

public class TestResultValue
{
    public string? Id { get; set; }
}

public class TestDeleteCommand : IRequest<Result>
{
}

public class BaseEndpointTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<TestEndpoint>> _loggerMock;

    public BaseEndpointTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<TestEndpoint>>();
    }

    #region AddResultErrors<T> Tests

    [Fact]
    public void AddResultErrors_WithValidationErrors_AddsErrors()
    {
        // Arrange
        var endpoint = Factory.Create<TestEndpoint>(_mediatorMock.Object, _loggerMock.Object);
        var result = Result<TestResultValue>.Invalid(new List<ValidationError>
    {
      new() { ErrorMessage = "Field1 is required", Identifier = "Field1" },
      new() { ErrorMessage = "Field2 is invalid", Identifier = "Field2" }
    });

        // Act
        endpoint.TestAddResultErrors(result);

        // Assert
        endpoint.ValidationFailures.Should().HaveCount(2);
        endpoint.ValidationFailures.Should().Contain(f => f.ErrorMessage == "Field1 is required");
        endpoint.ValidationFailures.Should().Contain(f => f.ErrorMessage == "Field2 is invalid");
    }

    [Fact]
    public void AddResultErrors_WithGeneralErrors_AddsErrors()
    {
        // Arrange
        var endpoint = Factory.Create<TestEndpoint>(_mediatorMock.Object, _loggerMock.Object);
        var result = Result<TestResultValue>.Error("Error 1");
        result.Errors.ToList().Count.Should().Be(1); // Confirm Error() sets one error

        // Act
        endpoint.TestAddResultErrors(result);

        // Assert
        endpoint.ValidationFailures.Should().ContainSingle();
        endpoint.ValidationFailures.First().ErrorMessage.Should().Be("Error 1");
    }

    [Fact]
    public void AddResultErrors_WithEmptyErrors_AddsNoErrors()
    {
        // Arrange
        var endpoint = Factory.Create<TestEndpoint>(_mediatorMock.Object, _loggerMock.Object);
        var result = Result<TestResultValue>.Success(new TestResultValue { Id = "123" });

        // Act
        endpoint.TestAddResultErrors(result);

        // Assert
        endpoint.ValidationFailures.Should().BeEmpty();
    }

    [Fact]
    public void AddResultErrors_WithNullValidationErrors_DoesNotThrow()
    {
        // Arrange
        var endpoint = Factory.Create<TestEndpoint>(_mediatorMock.Object, _loggerMock.Object);
        var result = Result<TestResultValue>.Error("An error occurred");

        // Act
        var act = () => endpoint.TestAddResultErrors(result);

        // Assert
        act.Should().NotThrow();
        endpoint.ValidationFailures.Should().ContainSingle();
    }

    #endregion

    #region AddResultErrors (non-generic) Tests

    [Fact]
    public void AddResultErrors_NonGeneric_WithValidationErrors_AddsErrors()
    {
        // Arrange
        var endpoint = Factory.Create<TestEndpoint>(_mediatorMock.Object, _loggerMock.Object);
        var result = Result.Invalid(new List<ValidationError>
    {
      new() { ErrorMessage = "Field is required", Identifier = "Field" }
    });

        // Act
        endpoint.TestAddResultErrors(result);

        // Assert
        endpoint.ValidationFailures.Should().ContainSingle();
        endpoint.ValidationFailures.First().ErrorMessage.Should().Be("Field is required");
    }

    [Fact]
    public void AddResultErrors_NonGeneric_WithGeneralErrors_AddsErrors()
    {
        // Arrange
        var endpoint = Factory.Create<TestEndpoint>(_mediatorMock.Object, _loggerMock.Object);
        var result = Result.Error("Operation failed");

        // Act
        endpoint.TestAddResultErrors(result);

        // Assert
        endpoint.ValidationFailures.Should().ContainSingle();
        endpoint.ValidationFailures.First().ErrorMessage.Should().Be("Operation failed");
    }

    [Fact]
    public void AddResultErrors_NonGeneric_WithEmptyErrors_AddsNoErrors()
    {
        // Arrange
        var endpoint = Factory.Create<TestEndpoint>(_mediatorMock.Object, _loggerMock.Object);
        var result = Result.Success();

        // Act
        endpoint.TestAddResultErrors(result);

        // Assert
        endpoint.ValidationFailures.Should().BeEmpty();
    }

    #endregion

    #region HandleResultAsync<T> Tests (via HandleAsync)

    [Fact]
    public async Task HandleAsync_WithSuccessResult_ReturnsSuccessResponse()
    {
        // Arrange
        var endpoint = Factory.Create<TestEndpoint>(_mediatorMock.Object, _loggerMock.Object);
        var request = new TestRequest { Name = "TestName" };
        var result = Result<TestResultValue>.Success(new TestResultValue { Id = "123" });
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        endpoint.Response.IsSuccess.Should().BeTrue();
        endpoint.Response.Value.Should().NotBeNull();
        endpoint.Response.Value.Id.Should().Be("123");
        endpoint.Response.Value.Name.Should().Be("TestName");
    }

    [Fact]
    public async Task HandleAsync_WithNotFoundResult_Returns404()
    {
        // Arrange
        var endpoint = Factory.Create<TestEndpoint>(_mediatorMock.Object, _loggerMock.Object);
        var request = new TestRequest { Name = "NotFound" };
        var result = Result<TestResultValue>.NotFound(new[] { "Resource not found" });
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        endpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        endpoint.ValidationFailures.Should().Contain(f => f.ErrorMessage == "Resource not found");
    }

    [Fact]
    public async Task HandleAsync_WithInvalidResult_Returns400()
    {
        // Arrange
        var endpoint = Factory.Create<TestEndpoint>(_mediatorMock.Object, _loggerMock.Object);
        var request = new TestRequest { Name = "Invalid" };
        var result = Result<TestResultValue>.Invalid(new List<ValidationError>
    {
      new() { ErrorMessage = "Name is required", Identifier = "Name" }
    });
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        endpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        endpoint.ValidationFailures.Should().Contain(f => f.ErrorMessage == "Name is required");
    }

    [Fact]
    public async Task HandleAsync_WithErrorResult_Returns400()
    {
        // Arrange
        var endpoint = Factory.Create<TestEndpoint>(_mediatorMock.Object, _loggerMock.Object);
        var request = new TestRequest { Name = "Error" };
        var result = Result<TestResultValue>.Error("An unexpected error occurred");
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        endpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        endpoint.ValidationFailures.Should().Contain(f => f.ErrorMessage == "An unexpected error occurred");
    }

    #endregion

    #region HandleResultAsync (non-generic) Tests

    [Fact]
    public async Task HandleResultAsync_NonGeneric_WithSuccessResult_Returns204()
    {
        // Arrange
        var deleteEndpoint = Factory.Create<TestDeleteEndpoint>(_mediatorMock.Object, new Mock<ILogger<TestDeleteEndpoint>>().Object);
        var result = Result.Success();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<TestDeleteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        await deleteEndpoint.HandleAsync(new TestRequest(), CancellationToken.None);

        // Assert
        deleteEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Fact]
    public async Task HandleResultAsync_NonGeneric_WithNotFoundResult_Returns404()
    {
        // Arrange
        var deleteEndpoint = Factory.Create<TestDeleteEndpoint>(_mediatorMock.Object, new Mock<ILogger<TestDeleteEndpoint>>().Object);
        var result = Result.NotFound(new[] { "Resource not found" });
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<TestDeleteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        await deleteEndpoint.HandleAsync(new TestRequest(), CancellationToken.None);

        // Assert
        deleteEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        deleteEndpoint.ValidationFailures.Should().Contain(f => f.ErrorMessage == "Resource not found");
    }

    [Fact]
    public async Task HandleResultAsync_NonGeneric_WithInvalidResult_Returns400()
    {
        // Arrange
        var deleteEndpoint = Factory.Create<TestDeleteEndpoint>(_mediatorMock.Object, new Mock<ILogger<TestDeleteEndpoint>>().Object);
        var result = Result.Invalid(new List<ValidationError>
    {
      new() { ErrorMessage = "Invalid operation", Identifier = "Operation" }
    });
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<TestDeleteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        await deleteEndpoint.HandleAsync(new TestRequest(), CancellationToken.None);

        // Assert
        deleteEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        deleteEndpoint.ValidationFailures.Should().Contain(f => f.ErrorMessage == "Invalid operation");
    }

    [Fact]
    public async Task HandleResultAsync_NonGeneric_WithErrorResult_Returns400()
    {
        // Arrange
        var deleteEndpoint = Factory.Create<TestDeleteEndpoint>(_mediatorMock.Object, new Mock<ILogger<TestDeleteEndpoint>>().Object);
        var result = Result.Error("Operation failed");
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<TestDeleteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        await deleteEndpoint.HandleAsync(new TestRequest(), CancellationToken.None);

        // Assert
        deleteEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        deleteEndpoint.ValidationFailures.Should().Contain(f => f.ErrorMessage == "Operation failed");
    }

    #endregion
}
