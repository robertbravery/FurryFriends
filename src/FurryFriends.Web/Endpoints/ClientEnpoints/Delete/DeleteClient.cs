using System.Net;
using Ardalis.GuardClauses;
using FurryFriends.UseCases.Domain.Clients.Command.DeleteClient;

namespace FurryFriends.Web.Endpoints.ClientEnpoints.Delete;

public class DeleteClient(IMediator mediator)
    : Endpoint<DeleteClientRequest>
{
  private readonly IMediator _mediator = Guard.Against.Null(mediator);

  public override void Configure()
  {
    Delete(DeleteClientRequest.Route);
    AllowAnonymous(); // Adjust authentication as needed
    Options(o => o.WithName("DeleteClient_" + Guid.NewGuid().ToString()));
    Summary(s =>
    {
      s.Summary = "Delete a Client";
      s.Description = "Deletes a client by their ID";
      s.Response<bool>(204, "Client successfully deleted");
      s.Response<bool>(404, "Client not found");
      s.Response<bool>(400, "Invalid client ID");
    });
  }

  public override async Task HandleAsync(DeleteClientRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new DeleteClientCommand(req.ClientId), ct);
    if (result.Status == ResultStatus.NotFound)
    {

      AddError(e => e.ClientId, "Client not found");
      await SendErrorsAsync((int)HttpStatusCode.NotFound, ct);
      return;

    }
    if (result.IsSuccess)
    {
      await SendNoContentAsync(ct);
      return;
    }

    await SendAsync(false, (int)HttpStatusCode.BadRequest, ct);
  }
}
