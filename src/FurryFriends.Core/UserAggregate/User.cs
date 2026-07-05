using System;
using Ardalis.GuardClauses;

namespace FurryFriends.Core.UserAggregate;

public class User : BaseEntity<Guid>, IAggregateRoot
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; }
    public DateTime RegisteredAt { get; private set; }
    public Guid? ProfileId { get; private set; }

    private User() { }

    public User(string email, string passwordHash, string role)
    {
        Id = Guid.NewGuid();
        Email = Guard.Against.NullOrWhiteSpace(email, nameof(email));
        PasswordHash = Guard.Against.NullOrWhiteSpace(passwordHash, nameof(passwordHash));
        Role = Guard.Against.NullOrWhiteSpace(role, nameof(role));
        RegisteredAt = DateTime.UtcNow;
    }

    public void AssignProfile(Guid profileId) => ProfileId = profileId;
}
