using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Schedule;

public class AddScheduleRequest
{
  public const string Route = "/petwalker/{PetWalkerId:guid}/schedule";
  [FromRoute] public Guid PetWalkerId { get; set; } = default!;
  [FastEndpoints.FromBody] public List<SetScheduleCommandBody> Schedules { get; set; } = default!;
}
