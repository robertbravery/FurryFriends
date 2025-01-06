using System.ComponentModel.DataAnnotations;

public class UpdateUserHourlyRateRequest
{
    public const string Route = "/petwalker/UpdateHourlyRate";
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public decimal HourlyRate { get; set; }
    [Required]
    public string Currency { get; set; } = default!;
}
