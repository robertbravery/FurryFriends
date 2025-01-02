using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UseCases.Users.CreateUser;

public record CreateUserCommand
(
    string FirstName,
    string LastName,
    string Email,
    string CountryCode,
    string Number, string Street, string City, string State, string Country, string ZipCode
) : ICommand<Result<Guid>>;
