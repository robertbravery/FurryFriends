﻿using FurryFriends.Core.ClientAggregate.Enums;

namespace FurryFriends.UseCases.Domain.Clients.DTO;

public record ClientDTO(
    Guid Id,
    string Name,
    string Email,
    string PhoneNumber,
    string Street,
    string City,
    string State,
    string ZipCode,
    ClientType ClientType,
    TimeOnly? PreferredContactTime,
    ReferralSource? ReferralSource,
    List<ClientPetDto> Pets
);
