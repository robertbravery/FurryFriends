using Azure;
using FurryFriends.UseCases.PetWalkers.GetPetWalker;
using FurryFriends.Web.Endpoints.Base;
using FurryFriends.Web.Endpoints.PetWalkerEndpoints.Records;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Get;

public class GetPetWalkerByEmail(IMediator _mediator) : Endpoint<GetPetWalkerByEmailRequest, ResponseBase<PetWalkerRecord>>
{
  public override void Configure()
  {
    Get(GetPetWalkerByEmailRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("GetUserByEmail_" + Guid.NewGuid().ToString())); // Ensure unique name
    Summary(s =>
    {
      s.Summary = "Get User By Email";
      s.Description = "Returns a user by email";
      s.Response<GetPetWalkerByEmailResponse>(200, "Get User By Email");
      s.Response<Response>(400, "Failed to retrieve user");
      s.Response<Response>(401, "Unauthorized");
      s.Response<Response>(404, "Not Found");
    });
  }

  public override async Task HandleAsync(GetPetWalkerByEmailRequest req, CancellationToken ct)
  {
    var query = new GetPetWalkerQuery(req.Email);
    var result = await _mediator.Send(query, ct);
    if (result.Value is null || !result.IsSuccess)
    {
      if (result.IsNotFound())
      {
        AddError("User not found");
        await SendNotFoundAsync(cancellation: ct);
        return;
      }

      AddError("Failed to retrieve user");
      await SendErrorsAsync(cancellation: ct);
      return;
    }
    else
    {
      var petWalkerDto = new PetWalkerRecord(
        result.Value.Id,
        result.Value.Name,
        result.Value.Email,
        result.Value.PhoneNumber,
        result.Value.Address,
        result.Value.ServiceLocation,
        result.Value.BioPicture,
        result.Value.Photos);
      Response = new GetPetWalkerByEmailResponse(petWalkerDto);
    }
  }
}
