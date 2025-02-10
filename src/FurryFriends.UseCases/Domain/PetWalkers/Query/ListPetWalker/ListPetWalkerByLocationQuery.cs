using FurryFriends.Core.PetWalkerAggregate;

namespace FurryFriends.UseCases.Domain.PetWalkers.Query.ListPetWalker;

public record ListPetWalkerByLocationQuery(
  string? SearchString,
  Guid? Location,
  int PageNumber = 1,
  int PageSize = 10) : IQuery<Result<(List<PetWalker> Users, int TotalCount)>>;
