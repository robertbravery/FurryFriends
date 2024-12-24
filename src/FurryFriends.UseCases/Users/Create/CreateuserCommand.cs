using FurryFriends.Core.ValueObjects;

namespace FurryFriends.UseCases.Users.Create;

  public record CreateUserCommand
  (
      string Name,
      string Email,
      string CountryCode,
      string AreaCode,
      string Number,
      string Street, string City, string State, string ZipCode
  ) : ICommand<Result<Guid>>;
