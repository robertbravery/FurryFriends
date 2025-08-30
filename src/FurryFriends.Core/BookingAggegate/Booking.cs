// Core/Entities/Booking.cs
using FurryFriends.Core.ClientAggregate;
using FurryFriends.Core.PetWalkerAggregate;

public class Booking : BaseEntity<Guid>, IAggregateRoot
{
  public Guid PetWalkerId { get; private set; }
  public Guid PetOwnerId { get; private set; }
  public DateTime Start { get; private set; }
  public DateTime End { get; private set; }

  public virtual PetWalker PetWalker { get; private set; } = default!;
  public virtual Client PetOwner { get; private set; } = default!;

  public Booking(Guid petWalkerId, Guid petOwnerId, DateTime start, DateTime end)
  {
    Id = Guid.NewGuid();
    PetWalkerId = petWalkerId;
    PetOwnerId = petOwnerId;
    Start = start;
    End = end;
  }
}
