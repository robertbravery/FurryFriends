using FluentAssertions;
using FurryFriends.Contracts.BookingContracts;
using Xunit;

namespace FurryFriends.Contracts.Tests.BookingContracts
{
    public class CancelBookingContractTests
    {
        [Fact]
        public void CancelBookingRequest_Should_HaveCorrectSchema()
        {
            // Arrange
            var request = new CancelBookingRequest
            {
                BookingId = Guid.NewGuid(),
                Reason = CancellationReason.ClientRequest
            };

            // Act & Assert
            request.BookingId.Should().NotBeEmpty();
            request.Reason.Should().BeOneOf(CancellationReason.ClientRequest, CancellationReason.PetWalkerRequest, CancellationReason.Other);
        }

        [Fact]
        public void CancelBookingResponse_Should_HaveCorrectSchema()
        {
            // Arrange
            var response = new CancelBookingResponse
            {
                Success = true
            };

            // Act & Assert
            response.Success.Should().BeTrue();
        }
    }
}