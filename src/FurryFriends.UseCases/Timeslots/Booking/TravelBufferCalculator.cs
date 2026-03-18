namespace FurryFriends.UseCases.Timeslots.Booking;

/// <summary>
/// Service to calculate travel buffer time between bookings
/// </summary>
public class TravelBufferCalculator
{
    // Average travel speed in South Africa urban areas (km/h)
    private const double AverageTravelSpeedKmh = 40.0;
    
    // Default buffer time in minutes when distance cannot be calculated
    private const int DefaultBufferMinutes = 15;

    /// <summary>
    /// Calculates travel buffer time in minutes between two addresses
    /// </summary>
    /// <param name="originAddress">Origin address</param>
    /// <param name="destinationAddress">Destination address</param>
    /// <returns>Buffer time in minutes</returns>
    public int CalculateBufferMinutes(string originAddress, string destinationAddress)
    {
        if (string.IsNullOrWhiteSpace(originAddress) || string.IsNullOrWhiteSpace(destinationAddress))
        {
            return DefaultBufferMinutes;
        }

        // Simple string comparison for now - in production this would use a mapping service
        if (AreAddressesSimilar(originAddress, destinationAddress))
        {
            return 0;
        }

        // For different addresses, calculate based on assumed distance
        // In a real implementation, this would call a distance API
        var estimatedDistanceKm = EstimateDistance(originAddress, destinationAddress);
        var travelTimeMinutes = (estimatedDistanceKm / AverageTravelSpeedKmh) * 60;

        // Add buffer for setup/cleanup time
        return (int)Math.Ceiling(travelTimeMinutes + 5);
    }

    /// <summary>
    /// Checks if two addresses are similar (same suburb/city)
    /// </summary>
    private bool AreAddressesSimilar(string address1, string address2)
    {
        var normalized1 = NormalizeAddress(address1);
        var normalized2 = NormalizeAddress(address2);
        
        return normalized1.Equals(normalized2, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Normalizes address for comparison
    /// </summary>
    private string NormalizeAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return string.Empty;

        // Extract city/suburb from address (simple implementation)
        var parts = address.Split(',');
        if (parts.Length >= 2)
        {
            // Return the last two parts (suburb, city)
            return string.Join(",", parts.Skip(parts.Length - 2)).Trim().ToLowerInvariant();
        }

        return address.Trim().ToLowerInvariant();
    }

    /// <summary>
    /// Estimates distance between two addresses
    /// </summary>
    private double EstimateDistance(string originAddress, string destinationAddress)
    {
        // In production, this would call a mapping API like Google Maps or OpenStreetMap
        // For now, return a default estimated distance
        return 5.0; // 5 km default
    }
}
