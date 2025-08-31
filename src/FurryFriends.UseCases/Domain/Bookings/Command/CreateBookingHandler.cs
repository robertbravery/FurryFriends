// Application/Scheduling/Commands/CreateBookingCommand.cs
using FurryFriends.Core.BookingAggregate;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.UseCases.Domain.Bookings.Dto;
using FurryFriends.UseCases.Services.BookingService;

// Application/Scheduling/Handlers/CreateBookingHandler.cs
public class CreateBookingHandler : ICommandHandler<CreateBookingCommand, Result<BookingDto>>
{
  private readonly IRepository<PetWalker> _walkerRepo;
  private readonly IRepository<Booking> _bookingRepo;
  private readonly IBookingService _bookingService;

  public CreateBookingHandler(IRepository<PetWalker> walkerRepo, IRepository<Booking> bookingRepo, IBookingService bookingService)
  {
    _walkerRepo = walkerRepo;
    _bookingRepo = bookingRepo;
    _bookingService = bookingService;
  }

  public async Task<Result<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
  {
    //// Check if walker exists
    //var walker = await _walkerRepo.GetByIdAsync(request.PetWalkerId, cancellationToken)
    //    ?? throw new NotFoundException("Id", request.PetWalkerId.ToString());

    //// Check for booking conflicts
    //var conflicts = await _bookingRepo.ListAsync(
    //    new BookingConflictSpec(request.PetWalkerId, request.Start, request.End),
    //    cancellationToken
    //);

    //if (conflicts.Any())
    //  throw new InvalidOperationException("Time slot is already booked");

    // Create booking
    // Fix for CS9035: Set the required 'Id' property of BaseEntity<Guid> in the object initializer
    //var booking = new Booking(request.PetWalkerId, request.PetOwnerId, request.Start, request.End)
    //{
    //  Id = Guid.NewGuid() // Generate a new unique identifier for the booking
    //};
    //var bookingResult = await _bookingRepo.AddAsync(booking, cancellationToken);

    var booking = await _bookingService.CreateBookingAsync(request.PetWalkerId, request.PetOwnerId, request.Start, request.End, 0.0m);

    return booking == null
      ? Result<BookingDto>.Error("Failed to create booking")
      : Result<BookingDto>.Success(new BookingDto(booking.Id, booking.StartTime, booking.EndTime));
  }
}
