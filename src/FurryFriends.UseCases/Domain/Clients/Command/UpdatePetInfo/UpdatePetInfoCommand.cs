﻿namespace FurryFriends.UseCases.Domain.Clients.Command.UpdatePetInfo;

public record UpdatePetInfoCommand(
    Guid ClientId,
    Guid PetId,
    string Name,
    int Age,
    double Weight,
    string Color,
    string? MedicalHistory,
    bool IsVaccinated,
    string? FavoriteActivities,
    string? DietaryRestrictions,
    string? SpecialNeeds,
    string? Photo,
    int BreedId = 0) : ICommand<Result>;
