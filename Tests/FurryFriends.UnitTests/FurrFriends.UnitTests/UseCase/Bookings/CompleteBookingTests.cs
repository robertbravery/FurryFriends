using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.BookingAggregate.Enums;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Domain.Bookings.Command;
using Microsoft.Extensions.Logging;
using Moq;

namespace FurryFriends.UnitTests.UseCase.Bookings;

public class CompleteBookingTests
{
  private readonly Mock<IRepository<Booking>> _mockBookingRepository;
  private readonly Mock<ILogger<CompleteBookingHandler>> _mockLogger;
  private readonly CompleteBookingHandler _handler;
  private readonly CancellationToken _ct;

  public CompleteBookingTests()
  {
    _mockBookingRepository = new Mock<IRepository<Booking>>();
    _mockLogger = new Mock<ILogger<CompleteBookingHandler>>();
    _handler = new CompleteBookingHandler(_mockBookingRepository.Object, _mockLogger.Object);
    _ct = CancellationToken.None;
  }

  [Fact]
  public async Task Handle_ShouldCompleteBooking_WhenStatusIsInProgress()
  {
    // Arrange
    var booking = await CreateBookingAsync(BookingStatus.InProgress);
    SetupRepository(booking);

    // Act
    var result = await _handler.Handle(new CompleteBookingCommand(booking.Id), _ct);

    // Assert
    result.IsSuccess.Should().BeTrue();
    booking.Status.Should().Be(BookingStatus.Completed);
    result.Value.Status.Should().Be("Completed");
    result.Value.BookingId.Should().Be(booking.Id);
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

    // Act
    var result = await _handler.Handle(new CompleteBookingCommand(bookingId), _ct);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Errors.Should().Contain("Booking not found");
    _mockBookingRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Booking>(), _ct), Times.Never);
  }

  [Theory]
  [InlineData(BookingStatus.Pending)]
  [InlineData(BookingStatus.Confirmed)]
  [InlineData(BookingStatus.Completed)]
  [InlineData(BookingStatus.Cancelled)]
  [InlineData(BookingStatus.NoShow)]
  public async Task Handle_ShouldReturnError_WhenStatusIsNotInProgress(BookingStatus status)
  {
    // Arrange
    var booking = await CreateBookingAsync(status);
    SetupRepository(booking);

    // Act
    var result = await _handler.Handle(new CompleteBookingCommand(booking.Id), _ct);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Errors.Should().Contain($"Cannot complete booking in {status} status");
    _mockBookingRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Booking>(), _ct), Times.Never);
  }

  [Fact]
  public async Task Handle_ShouldReturnError_WhenRepositoryUpdateThrows()
  {
    // Arrange
    var booking = await CreateBookingAsync(BookingStatus.InProgress);
    _mockBookingRepository
        .Setup(repo => repo.GetByIdAsync(booking.Id, _ct))
        .ReturnsAsync(booking);
    _mockBookingRepository
        .Setup(repo => repo.UpdateAsync(booking, _ct))
        .ThrowsAsync(new InvalidOperationException("Database error"));

    // Act
    var result = await _handler.Handle(new CompleteBookingCommand(booking.Id), _ct);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Errors.Should().Contain("Database error");
  }

  private void SetupRepository(Booking booking)
  {
    _mockBookingRepository
        .Setup(repo => repo.GetByIdAsync(booking.Id, _ct))
        .ReturnsAsync(booking);
    _mockBookingRepository
        .Setup(repo => repo.UpdateAsync(booking, _ct))
        .Returns(Task.CompletedTask);
  }

  /// <summary>
  /// Builds a real booking aggregate (not a mock) so the state machine is genuinely exercised.
  /// </summary>
  private static async Task<Booking> CreateBookingAsync(BookingStatus status)
  {
    var phoneNumber = (await PhoneNumber.Create("027", "011-123-4567")).Value;
    var petWalker = PetWalker.Create(
        Name.Create("John", "Doe"),
        Email.Create("john.doe@example.com"),
        phoneNumber,
        Address.Create("123 Main St", "Johannesburg", "Gauteng", "2001", "South Africa"));
    petWalker.AddSchedule(new Schedule(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0)));
    petWalker.UpdateDailyPetWalkLimit(10);

    var monday = NextMonday();
    var booking = Booking.Create(
        Guid.NewGuid(),
        monday,
        monday.AddHours(1),
        100m,
        petWalker,
        Enumerable.Empty<Booking>());

    AdvanceTo(booking, status);
    return booking;
  }

  private static DateTime NextMonday()
  {
    var date = DateTime.UtcNow.Date.AddDays(1);
    while (date.DayOfWeek != DayOfWeek.Monday)
    {
      date = date.AddDays(1);
    }
    return date.AddHours(10);
  }

  private static void AdvanceTo(Booking booking, BookingStatus status)
  {
    switch (status)
    {
      case BookingStatus.Pending:
        break;
      case BookingStatus.Confirmed:
        booking.Confirm();
        break;
      case BookingStatus.InProgress:
        booking.Confirm();
        booking.BeginWalk();
        break;
      case BookingStatus.Completed:
        booking.Confirm();
        booking.BeginWalk();
        booking.Complete();
        break;
      case BookingStatus.Cancelled:
        booking.Cancel("Cancelled for test");
        break;
      case BookingStatus.NoShow:
        booking.Confirm();
        booking.MarkAsNoShow();
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(status), status, null);
    }
  }
}
