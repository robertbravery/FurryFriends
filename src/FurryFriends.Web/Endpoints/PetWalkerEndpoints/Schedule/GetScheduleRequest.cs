using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Schedule;

public class GetScheduleRequest
{
  public const string Route = "/petwalker/{PetWalkerId:guid}/schedule";
  [FromRoute] public Guid PetWalkerId { get; set; } = default!;
}
