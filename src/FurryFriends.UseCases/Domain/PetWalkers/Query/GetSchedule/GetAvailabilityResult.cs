namespace FurryFriends.UseCases.Domain.PetWalkers.Query.GetSchedule;

public class GetAvailabilityResult
{
    public List<AvailableSlotDto> AvailableSlots { get; set; } = new();
    public bool HasSchedule { get; set; }
}