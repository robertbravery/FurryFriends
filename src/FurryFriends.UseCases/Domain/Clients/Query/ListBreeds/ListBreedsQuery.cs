﻿using FurryFriends.UseCases.Domain.Clients.DTO;
using Mediator;

namespace FurryFriends.UseCases.Domain.Clients.Query.ListBreeds;

public record ListBreedsQuery() : IQuery<Result<List<BreedDto>>>;
