using Ardalis.GuardClauses;
using FurryFriends.UseCases.Domain.Clients.Query.ListBreeds;

namespace FurryFriends.Web.Endpoints.ClientEndpoints.ListBreeds;

public class ListBreeds(IMediator mediator, ILogger<ListBreeds> logger) : EndpointWithoutRequest<List<BreedDto>>
{
  private readonly IMediator _mediator = Guard.Against.Null(mediator);
  private readonly ILogger<ListBreeds> _logger = Guard.Against.Null(logger);

  public override void Configure()
  {
    Get(ListBreedsRequest.Route);
    AllowAnonymous();
    Options(o => o.WithName("ListBreeds_" + Guid.NewGuid().ToString())); // Ensure unique name
    Summary(s =>
    {
      s.Summary = "Retrieve List of Breeds";
      s.Description = "Returns a list of all available breeds with their species";
      s.Response<Result<List<BreedDto>>>(200, "Breeds retrieved successfully");
      s.Response<Result<List<BreedDto>>>(400, "Failed to retrieve breeds");
    });
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    try
    {
      var query = new ListBreedsQuery();
      var result = await _mediator.Send(query, cancellationToken);

      if (!result.IsSuccess)
      {
        _logger.LogError("Error retrieving breeds: {Errors}", result.Errors);
        await SendErrorsAsync(400, cancellationToken);
        return;
      }

      // Map from UseCases DTO to Endpoint DTO
      var breedDtos = result.Value.Select(b => new BreedDto(
          b.Id,
          b.Name,
          b.Description,
          b.SpeciesId,
          b.SpeciesName
      )).ToList();

      Response = Result.Success(breedDtos);
      await SendOkAsync(Response, cancellationToken);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving breeds");
      await SendErrorsAsync(500, cancellationToken);
    }
  }
}
