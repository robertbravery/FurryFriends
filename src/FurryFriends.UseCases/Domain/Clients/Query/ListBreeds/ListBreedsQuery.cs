﻿using FurryFriends.UseCases.Domain.Clients.DTO;

namespace FurryFriends.UseCases.Domain.Clients.Query.ListBreeds;

public record ListBreedsQuery() : IQuery<Result<List<BreedDto>>>;
