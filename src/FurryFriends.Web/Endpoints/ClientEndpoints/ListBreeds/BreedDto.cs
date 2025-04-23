﻿namespace FurryFriends.Web.Endpoints.ClientEndpoints.ListBreeds;

public record BreedDto(
    int Id,
    string Name,
    string Description,
    int SpeciesId,
    string SpeciesName
);
