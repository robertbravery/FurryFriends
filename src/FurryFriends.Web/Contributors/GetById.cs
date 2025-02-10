using FurryFriends.UseCases.Domain.Contributors.Get;

namespace FurryFriends.Web.Contributors;

/// <summary>
/// Get a Contributor by integer ID.
/// </summary>
/// <remarks>
/// Takes a positive integer ID and returns a matching Contributor record.
/// </remarks>
public class GetById(IMediator _mediator)
  : Endpoint<GetContributorByIdRequest, ContributorRecord>
{
  public override void Configure()
  {
    Get(GetContributorByIdRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("GetContributorById_" + Guid.NewGuid().ToString())); // Ensure unique name

  }

  public override async Task HandleAsync(GetContributorByIdRequest request,
    CancellationToken cancellationToken)
  {
    var query = new GetContributorQuery(request.ContributorId);

    var result = await _mediator.Send(query, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      Response = new ContributorRecord(result.Value.Id, result.Value.FullName, result.Value.PhoneNumber);
    }
  }
}
