using Ardalis.GuardClauses;
using FurryFriends.Core;

namespace FurryFriends.Core.UserAggregate.Events;

public sealed class UserAuthenticatedEvent : DomainEventBase
{
    public Guid UserId { get; }
    public string Email { get; }
    public string Role { get; }

    public UserAuthenticatedEvent(Guid userId, string email, string role)
    {
        UserId = Guard.Against.Null(userId, nameof(userId));
        Email = Guard.Against.NullOrWhiteSpace(email, nameof(email));
        Role = Guard.Against.NullOrWhiteSpace(role, nameof(role));
        Id = Guid.NewGuid();
    }
}