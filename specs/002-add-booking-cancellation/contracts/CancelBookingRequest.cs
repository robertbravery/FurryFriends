using System;

namespace FurryFriends.Contracts.BookingContracts
{
    public class CancelBookingRequest
    {
        public const string Route = "/api/bookings/{BookingId}/cancel";
        public Guid BookingId { get; set; }
        public CancellationReason Reason { get; set; }
    }

    public enum CancellationReason
    {
        ClientRequest,
        PetWalkerRequest,
        Other
    }
}