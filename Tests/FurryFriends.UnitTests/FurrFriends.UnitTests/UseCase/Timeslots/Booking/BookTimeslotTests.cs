using FurryFriends.Core.Enums;
using FurryFriends.Core.TimeslotAggregate;
using FurryFriends.Core.TimeslotAggregate.Specifications;
using FurryFriends.UseCases.Timeslots.Booking;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TimeslotEntity = FurryFriends.Core.TimeslotAggregate.Timeslot;

namespace FurryFriends.UseCases.Timeslots.Booking.Tests;

public class BookTimeslotTests
{
    private readonly Mock<IRepository<TimeslotEntity>> _timeslotRepositoryMock;
    private readonly Mock<IRepository<Core.BookingAggregate.Booking>> _bookingRepositoryMock;
    private readonly Mock<IRepository<Core.ClientAggregate.Client>> _clientRepositoryMock;
    private readonly Mock<IRepository<TravelBuffer>> _travelBufferRepositoryMock;
    private readonly Mock<ILogger<BookTimeslotHandler>> _loggerMock;
    private readonly TravelBufferCalculator _travelBufferCalculator;
    private readonly BookTimeslotHandler _handler;

    public BookTimeslotTests()
    {
        _timeslotRepositoryMock = new Mock<IRepository<TimeslotEntity>>();
        _bookingRepositoryMock = new Mock<IRepository<Core.BookingAggregate.Booking>>();
        _clientRepositoryMock = new Mock<IRepository<Core.ClientAggregate.Client>>();
        _travelBufferRepositoryMock = new Mock<IRepository<TravelBuffer>>();
        _loggerMock = new Mock<ILogger<BookTimeslotHandler>>();
        _travelBufferCalculator = new TravelBufferCalculator();

        _handler = new BookTimeslotHandler(
            _timeslotRepositoryMock.Object,
            _bookingRepositoryMock.Object,
            _clientRepositoryMock.Object,
            _travelBufferRepositoryMock.Object,
            _travelBufferCalculator,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_TimeslotNotFound_ReturnsError()
    {
        // Arrange
        var timeslotId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var clientAddress = "123 Main St, Johannesburg, Gauteng, 2001";

        _timeslotRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<TimeslotByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TimeslotEntity?)null);

        var command = new BookTimeslotCommand(
            timeslotId,
            clientId,
            clientAddress,
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Errors.First());
    }

    [Fact]
    public async Task Handle_TimeslotAlreadyBooked_ReturnsError()
    {
        // Arrange
        var timeslotId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var clientAddress = "123 Main St, Johannesburg, Gauteng, 2001";
        
        var timeslot = TimeslotEntity.Create(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new TimeOnly(9, 0),
            45,
            TimeslotStatus.Booked).Value;

        _timeslotRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<TimeslotByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeslot);

        var command = new BookTimeslotCommand(
            timeslotId,
            clientId,
            clientAddress,
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("no longer available", result.Errors.First());
    }

    [Fact]
    public async Task Handle_ClientNotFound_ReturnsError()
    {
        // Arrange
        var timeslotId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var clientAddress = "123 Main St, Johannesburg, Gauteng, 2001";
        
        var timeslot = TimeslotEntity.Create(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            new TimeOnly(9, 0),
            45,
            TimeslotStatus.Available).Value;

        _timeslotRepositoryMock
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<TimeslotByIdSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(timeslot);

        _clientRepositoryMock
            .Setup(x => x.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Core.ClientAggregate.Client?)null);

        var command = new BookTimeslotCommand(
            timeslotId,
            clientId,
            clientAddress,
            new List<Guid> { Guid.NewGuid() });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Client not found", result.Errors.First());
    }
}
