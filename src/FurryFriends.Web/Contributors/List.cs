using FurryFriends.UseCases.Domain.Contributors;
using FurryFriends.UseCases.Domain.Contributors.List;

namespace FurryFriends.Web.Contributors;

/// <summary>
/// List all Contributors
/// </summary>
/// <remarks>
/// List all contributors - returns a ContributorListResponse containing the Contributors.
/// </remarks>
public class List(IMediator _mediator) : EndpointWithoutRequest<ContributorListResponse>
{
  public override void Configure()
  {
    Get("/Contributors");
    AllowAnonymous();
    Options(o => o.WithName("ListContributors_" + Guid.NewGuid().ToString())); // Ensure unique name

  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    Result<IEnumerable<ContributorDTO>> result = await _mediator.Send(new ListContributorsQuery(null, null), cancellationToken);

    if (result.IsSuccess)
    {
      Response = new ContributorListResponse
      {
        Contributors = result.Value.Select(c => new ContributorRecord(c.Id, c.FullName, c.PhoneNumber)).ToList()
      };
    }
  }
}
