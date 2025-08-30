// Application/Scheduling/Commands/CreateBookingCommand.cs
using Ardalis.GuardClauses;
using FurryFriends.Core.PetWalkerAggregate;
using FurryFriends.UseCases.Domain.Bookings.Dto;

// Application/Scheduling/Handlers/CreateBookingHandler.cs
public class CreateBookingHandler : ICommandHandler<CreateBookingCommand, Result<BookingDto>>
{
  private readonly IRepository<PetWalker> _walkerRepo;
  private readonly IRepository<Booking> _bookingRepo;

  public CreateBookingHandler(IRepository<PetWalker> walkerRepo, IRepository<Booking> bookingRepo)
  {
    _walkerRepo = walkerRepo;
    _bookingRepo = bookingRepo;
  }

  public async Task<Result<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
  {
    // Check if walker exists
    var walker = await _walkerRepo.GetByIdAsync(request.PetWalkerId, cancellationToken)
        ?? throw new NotFoundException("Id", request.PetWalkerId.ToString());

    // Check for booking conflicts
    var conflicts = await _bookingRepo.ListAsync(
        new BookingConflictSpec(request.PetWalkerId, request.Start, request.End),
        cancellationToken
    );

    if (conflicts.Any())
      throw new InvalidOperationException("Time slot is already booked");

    // Create booking
    // Fix for CS9035: Set the required 'Id' property of BaseEntity<Guid> in the object initializer
    var booking = new Booking(request.PetWalkerId, request.PetOwnerId, request.Start, request.End)
    {
      Id = Guid.NewGuid() // Generate a new unique identifier for the booking
    };
    var bookingResult = await _bookingRepo.AddAsync(booking, cancellationToken);


    return bookingResult == null
      ? Result<BookingDto>.Error("Failed to create booking")
      : Result<BookingDto>.Success(new BookingDto(booking.Id, booking.Start, booking.End));
  }
}
