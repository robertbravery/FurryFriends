﻿namespace FurryFriends.UseCases.Domain.Clients.DTO;

public record BreedDto(
    int Id,
    string Name,
    string Description,
    int SpeciesId,
    string SpeciesName
);
