using Ardalis.Specification;
using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.ClientAggregate.Specifications;
using FurryFriends.UseCases.Domain.Clients.DTO;

namespace FurryFriends.UseCases.Domain.Clients.Query.ListBreeds;

public class ListBreedsHandler : IQueryHandler<ListBreedsQuery, Result<List<BreedDto>>>
{
  private readonly IReadRepository<Breed> _breedRepository;

  public ListBreedsHandler(IReadRepository<Breed> breedRepository)
  {
    _breedRepository = breedRepository;
  }

  public async Task<Result<List<BreedDto>>> Handle(ListBreedsQuery request, CancellationToken cancellationToken)
  {
    try
    {
      // Create a specification to include the Species navigation property
      var specification = new BreedsWithSpeciesSpec();

      // Get all breeds with their species
      var breeds = await _breedRepository.ListAsync(specification, cancellationToken);

      // Map to DTOs
      var breedDtos = breeds.Select(b => new BreedDto(
          b.Id,
          b.Name,
          b.Description,
          b.SpeciesId,
          b.Species.Name
      )).ToList();

      return Result.Success(breedDtos);
    }
    catch (Exception ex)
    {
      return Result.Error($"Error retrieving breeds: {ex.Message}");
    }
  }
}
