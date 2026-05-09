using FurryFriends.BlazorUI.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FurryFriends.BlazorUI.Components.Common;

public partial class RatingDisplay
{
    [Parameter]
    public RatingSummaryDto? RatingSummary { get; set; }

    [Parameter]
    public int MaxStars { get; set; } = 5;

    [Parameter]
    public bool ShowNumericValue { get; set; } = true;

    private int RoundedAverage => RatingSummary != null
        ? (int)Math.Round(RatingSummary.AverageRating, MidpointRounding.AwayFromZero)
        : 0;
}
