using Ardalis.GuardClauses;
using FurryFriends.Core.PetWalkerAggregate;
using MediatR;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetSchedule;

public class GetAvailabilityHandler : IRequestHandler<GetAvailabilityQuery, List<AvailableSlotDto>>
{
  private readonly IRepository<PetWalker> _walkerRepo;
  private readonly IRepository<Booking> _bookingRepo;

  public GetAvailabilityHandler(IRepository<PetWalker> walkerRepo, IRepository<Booking> bookingRepo)
  {
    _walkerRepo = walkerRepo;
    _bookingRepo = bookingRepo;
  }

  public async Task<List<AvailableSlotDto>> Handle(GetAvailabilityQuery request, CancellationToken cancellationToken)
  {
    var walker = await _walkerRepo.GetByIdAsync(request.PetWalkerId, cancellationToken)
        ?? throw new NotFoundException("PetWalker ID", request.PetWalkerId.ToString());

    var daySchedule = walker.Schedules
        .Where(s => s.DayOfWeek == request.Date.DayOfWeek)
        .ToList();

    if (!daySchedule.Any())
      return new List<AvailableSlotDto>();

    var bookings = await _bookingRepo.ListAsync(
        new BookingByDateSpec(request.PetWalkerId, request.Date),
        cancellationToken
    );

    var availableSlots = new List<AvailableSlotDto>();

    foreach (var schedule in daySchedule)
    {
      var start = request.Date.Date + schedule.StartTime.ToTimeSpan();
      var end = request.Date.Date + schedule.EndTime.ToTimeSpan();

      var currentTime = start;

      foreach (var booking in bookings.OrderBy(b => b.Start))
      {
        if (booking.Start > currentTime)
        {
          availableSlots.Add(new AvailableSlotDto(currentTime, booking.Start));
        }
        currentTime = booking.End > currentTime ? booking.End : currentTime;
      }

      if (currentTime < end)
      {
        availableSlots.Add(new AvailableSlotDto(currentTime, end));
      }
    }

    return availableSlots;
  }
}
