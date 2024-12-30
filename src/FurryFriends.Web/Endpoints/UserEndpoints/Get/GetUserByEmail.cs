using Azure;
using FurryFriends.Web.Endpoints.UserEndpoints.Create;
using FurryFriends.Web.Endpoints.UserEndpoints.List;

namespace FurryFriends.Web.Endpoints.UserEndpoints.Get;

public class GetUserByEmail(IMediator _mediator) : Endpoint<GetUserByEmailRequest, GetUserByEmailResponse>
{
  public override void Configure()
  {
    Get(GetUserByEmailRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("GetUserByEmail_" + Guid.NewGuid().ToString())); // Ensure unique name
    Summary(s =>
    {
      s.Summary = "Get User By Email";
      s.Description = "Returns a user by email";
      s.Response<ListUsersResponse>(200, "User retrieved successfully");
      s.Response<Response>(400, "Failed to retrieve user");
      s.Response<Response>(401, "Unauthorized");
    });
  }

  public override async Task HandleAsync(GetUserByEmailRequest req, CancellationToken ct)
  {
    var query = new GetUserQuery(req.Email);
    var user = await _mediator.Send(query, ct);
    if (user is null)
    {
      await SendNotFoundAsync(ct);
    }
    else
    {
      await SendOkAsync(new GetUserByEmailResponse(user.Value.Id, user.Value.Name, user.Value.PhoneNumber, user.Value.Address), ct);
    }
  }
}
