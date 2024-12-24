using FurryFriends.Core.GuardClauses;
using FurryFriends.Core.ValueObjects;

namespace FurryFriends.Core.Entities;

    public class User: IAggregateRoot
    {
        public Guid Id { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public PhoneNumber PhoneNumber { get; private set; } = default!;
        public Address Address { get; private set; } = default!;

        public User()
        {
            
        }
        public User(string name, string email, PhoneNumber phoneNumber, Address address)
        {
            // Guard clauses to ensure valid input
            Guard.Against.NullOrEmpty(name, nameof(name));
            Guard.Against.NullOrEmpty(email, nameof(email));
            Guard.Against.InvalidEmail(email, nameof(email));
            Guard.Against.Null(address, nameof(address));

            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
        }

        public void UpdateDetails(string name, string email, PhoneNumber phoneNumber, Address address)
        {
            // Guard clauses to ensure valid input
            Guard.Against.NullOrEmpty(name, nameof(name));
            Guard.Against.NullOrEmpty(email, nameof(email));
            Guard.Against.InvalidEmail(email, nameof(email));
            Guard.Against.Null(address, nameof(address));

            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
        }

        public void Deactivate()
        {
            // Deactivation logic can be added here
        }
    }
