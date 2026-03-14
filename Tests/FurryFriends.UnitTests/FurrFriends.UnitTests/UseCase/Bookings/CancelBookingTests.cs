using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.BookingAggregate.Enums;
using FurryFriends.UseCases.Domain.Bookings.Command;
using Microsoft.Extensions.Logging;
using Moq;

namespace FurryFriends.UnitTests.UseCase.Bookings;

public class CancelBookingTests
{
  private readonly Mock<IRepository<Booking>> _mockBookingRepository;
  private readonly Mock<ILogger<CancelBookingHandler>> _mockLogger;
  private readonly CancelBookingHandler _handler;
  private readonly CancellationToken _ct;

  public CancelBookingTests()
  {
    _mockBookingRepository = new Mock<IRepository<Booking>>();
    _mockLogger = new Mock<ILogger<CancelBookingHandler>>();
    _handler = new CancelBookingHandler(_mockBookingRepository.Object, _mockLogger.Object);
    _ct = CancellationToken.None;
  }

  [Fact]
  public async Task Handle_ShouldReturnSuccess_WhenBookingExistsAndCanBeCancelled()
  {
    // Arrange
    var bookingId = Guid.NewGuid();
    var booking = Mock.Of<Booking>();

    // Setup mock properties
    var bookingMock = Mock.Get(booking);
    bookingMock.SetupProperty(b => b.Id, bookingId);
    bookingMock.SetupProperty(b => b.Status, BookingStatus.Confirmed);
    bookingMock.SetupProperty(b => b.Cancellation, null as Cancellation);

    _mockBookingRepository
        .Setup(repo => repo.GetByIdAsync(bookingId, _ct))
        .ReturnsAsync(booking);

    _mockBookingRepository
        .Setup(repo => repo.UpdateAsync(booking, _ct))
        .Returns(Task.CompletedTask);

    var command = new CancelBookingCommand(
      bookingId,
      CancellationReason.ClientRequest,
      CancelledBy.Client,
      "Client requested cancellation");

    // Act
    var result = await _handler.Handle(command, _ct);

    // Assert
    result.IsSuccess.Should().BeTrue();

    _mockBookingRepository.Verify(repo => repo.GetByIdAsync(bookingId, _ct), Times.Once);
    _mockBookingRepository.Verify(repo => repo.UpdateAsync(booking, _ct), Times.Once);
  }

  [Fact]
  public async Task Handle_ShouldReturnNotFound_WhenBookingDoesNotExist()
  {
    // Arrange
    var bookingId = Guid.NewGuid();

    _mockBookingRepository
        .Setup(repo => repo.GetByIdAsync(bookingId, _ct))
        .ReturnsAsync((Booking)null!);

    var command = new CancelBookingCommand(
      bookingId,
      CancellationReason.ClientRequest,
      CancelledBy.Client);

    // Act
    var result = await _handler.Handle(command, _ct);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Errors.Should().Contain("Booking not found");

    _mockBookingRepository.Verify(repo => repo.GetByIdAsync(bookingId, _ct), Times.Once);
    _mockBookingRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Booking>(), _ct), Times.Never);
  }

  [Fact]
  public async Task Handle_ShouldReturnError_WhenBookingIsAlreadyCancelled()
  {
    // Arrange
    var bookingId = Guid.NewGuid();
    var booking = Mock.Of<Booking>();

    var bookingMock = Mock.Get(booking);
    bookingMock.SetupProperty(b => b.Id, bookingId);
    bookingMock.SetupProperty(b => b.Status, BookingStatus.Cancelled);

    _mockBookingRepository
        .Setup(repo => repo.GetByIdAsync(bookingId, _ct))
        .ReturnsAsync(booking);

    var command = new CancelBookingCommand(
      bookingId,
      CancellationReason.ClientRequest,
      CancelledBy.Client);

    // Act
    var result = await _handler.Handle(command, _ct);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Errors.Should().Contain("Booking is already cancelled");

    _mockBookingRepository.Verify(repo => repo.GetByIdAsync(bookingId, _ct), Times.Once);
    _mockBookingRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Booking>(), _ct), Times.Never);
  }

  [Fact]
  public async Task Handle_ShouldReturnError_WhenBookingIsCompleted()
  {
    // Arrange
    var bookingId = Guid.NewGuid();
    var booking = Mock.Of<Booking>();

    var bookingMock = Mock.Get(booking);
    bookingMock.SetupProperty(b => b.Id, bookingId);
    bookingMock.SetupProperty(b => b.Status, BookingStatus.Completed);

    _mockBookingRepository
        .Setup(repo => repo.GetByIdAsync(bookingId, _ct))
        .ReturnsAsync(booking);

    var command = new CancelBookingCommand(
      bookingId,
      CancellationReason.ClientRequest,
      CancelledBy.Client);

    // Act
    var result = await _handler.Handle(command, _ct);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Errors.Should().Contain("Cannot cancel a completed booking");

    _mockBookingRepository.Verify(repo => repo.GetByIdAsync(bookingId, _ct), Times.Once);
    _mockBookingRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Booking>(), _ct), Times.Never);
  }

  [Fact]
  public async Task Handle_ShouldReturnError_WhenBookingIsInProgress()
  {
    // Arrange
    var bookingId = Guid.NewGuid();
    var booking = Mock.Of<Booking>();

    var bookingMock = Mock.Get(booking);
    bookingMock.SetupProperty(b => b.Id, bookingId);
    bookingMock.SetupProperty(b => b.Status, BookingStatus.InProgress);

    _mockBookingRepository
        .Setup(repo => repo.GetByIdAsync(bookingId, _ct))
        .ReturnsAsync(booking);

    var command = new CancelBookingCommand(
      bookingId,
      CancellationReason.ClientRequest,
      CancelledBy.Client);

    // Act
    var result = await _handler.Handle(command, _ct);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Errors.Should().Contain("Cannot cancel a booking that is in progress");

    _mockBookingRepository.Verify(repo => repo.GetByIdAsync(bookingId, _ct), Times.Once);
    _mockBookingRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Booking>(), _ct), Times.Never);
  }

  [Fact]
  public async Task Handle_ShouldReturnError_WhenCancellationThrowsException()
  {
    // Arrange
    var bookingId = Guid.NewGuid();
    var booking = Mock.Of<Booking>();

    var bookingMock = Mock.Get(booking);
    bookingMock.SetupProperty(b => b.Id, bookingId);
    bookingMock.SetupProperty(b => b.Status, BookingStatus.Confirmed);

    _mockBookingRepository
        .Setup(repo => repo.GetByIdAsync(bookingId, _ct))
        .ReturnsAsync(booking);

    _mockBookingRepository
        .Setup(repo => repo.UpdateAsync(booking, _ct))
        .ThrowsAsync(new InvalidOperationException("Database error"));

    var command = new CancelBookingCommand(
      bookingId,
      CancellationReason.ClientRequest,
      CancelledBy.Client);

    // Act
    var result = await _handler.Handle(command, _ct);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Errors.Should().Contain("Database error");

    _mockBookingRepository.Verify(repo => repo.GetByIdAsync(bookingId, _ct), Times.Once);
    _mockBookingRepository.Verify(repo => repo.UpdateAsync(booking, _ct), Times.Once);
  }
}