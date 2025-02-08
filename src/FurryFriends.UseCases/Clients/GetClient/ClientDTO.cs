﻿using FurryFriends.Core.ClientAggregate.Enums;

namespace FurryFriends.UseCases.Clients.GetClient;

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
    string? ReferralSource
);
