using System.Net;
using FluentValidation;
using FurryFriends.Web.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FurryFriends.UnitTests.Web.Middleware;

public class GlobalExceptionHandlerMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionHandlerMiddleware>> _loggerMock;
    private readonly RequestDelegate _nextMock;

    public GlobalExceptionHandlerMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionHandlerMiddleware>>();
        _nextMock = _ => Task.CompletedTask;
    }

    [Fact]
    public async Task InvokeAsync_NoException_CallsNextDelegate()
    {
        // Arrange
        var wasCalled = false;
        RequestDelegate next = (context) =>
        {
            wasCalled = true;
            return Task.CompletedTask;
        };
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_ValidationExceptionWithErrors_ReturnsBadRequestWithErrors()
    {
        // Arrange
        var middleware = new GlobalExceptionHandlerMiddleware(_nextMock, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Create a ValidationException with errors
        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
    {
      new("Field1", "Field1 is required"),
      new("Field2", "Field2 is invalid")
    };
        var validationException = new ValidationException(validationFailures);

        RequestDelegate throwingNext = (ctx) => throw validationException;

        var errorMiddleware = new GlobalExceptionHandlerMiddleware(throwingNext, _loggerMock.Object);

        // Act
        await errorMiddleware.InvokeAsync(context);
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        context.Response.ContentType.Should().StartWith("application/json");
    }

    [Fact]
    public async Task InvokeAsync_ValidationExceptionWithoutErrors_ReturnsBadRequest()
    {
        // Arrange
        var middleware = new GlobalExceptionHandlerMiddleware(_nextMock, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var validationException = new ValidationException(new List<FluentValidation.Results.ValidationFailure>());

        RequestDelegate throwingNext = (ctx) => throw validationException;

        var errorMiddleware = new GlobalExceptionHandlerMiddleware(throwingNext, _loggerMock.Object);

        // Act
        await errorMiddleware.InvokeAsync(context);
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_GeneralException_ReturnsInternalServerError()
    {
        // Arrange
        var middleware = new GlobalExceptionHandlerMiddleware(_nextMock, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        RequestDelegate throwingNext = (ctx) => throw new InvalidOperationException("Something went wrong");

        var errorMiddleware = new GlobalExceptionHandlerMiddleware(throwingNext, _loggerMock.Object);

        // Act
        await errorMiddleware.InvokeAsync(context);
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        context.Response.ContentType.Should().StartWith("application/json");
    }

    [Fact]
    public async Task InvokeAsync_GeneralException_LogsError()
    {
        // Arrange
        RequestDelegate throwingNext = (ctx) => throw new Exception("Test exception");
        var middleware = new GlobalExceptionHandlerMiddleware(throwingNext, _loggerMock.Object);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert - Verify LogError was called
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task InvokeAsync_ValidationException_LogsError()
    {
        // Arrange
        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
    {
      new("Field1", "Field1 is required")
    };
        var validationException = new ValidationException(validationFailures);

        RequestDelegate throwingNext = (ctx) => throw validationException;
        var middleware = new GlobalExceptionHandlerMiddleware(throwingNext, _loggerMock.Object);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert - Verify LogError was called for validation errors
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task InvokeAsync_ResponseBaseHasCorrectShape()
    {
        // Arrange
        RequestDelegate throwingNext = (ctx) => throw new Exception("Critical error");
        var middleware = new GlobalExceptionHandlerMiddleware(throwingNext, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync();

        // Assert
        body.Should().NotBeNullOrEmpty();
        body.Should().Contain("success");
        body.Should().Contain("false");
        body.Should().Contain("errorCode");
        body.Should().Contain("InternalServerError");
    }
}
