namespace FurryFriends.BlazorUI.Client.Models.Bookings;

/// <summary>
/// Request DTO for creating a new booking
/// </summary>
public class BookingRequestDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid PetId { get; set; }
    public Guid PetWalkerId { get; set; }
    public Guid PetOwnerId { get; set; }
    public decimal Price { get; set; }
    public string? Notes { get; set; }
}
