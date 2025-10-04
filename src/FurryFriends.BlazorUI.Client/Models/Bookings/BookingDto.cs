using FurryFriends.BlazorUI.Client.Models.Bookings.Enums;

namespace FurryFriends.BlazorUI.Client.Models.Bookings;

/// <summary>
/// Data transfer object for booking information
/// </summary>
public class BookingDto
{
    public Guid Id { get; set; }
    public Guid PetWalkerId { get; set; }
    public Guid PetOwnerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingStatus Status { get; set; }
    public decimal Price { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties for display
    public string? PetWalkerName { get; set; }
    public string? PetOwnerName { get; set; }
}
