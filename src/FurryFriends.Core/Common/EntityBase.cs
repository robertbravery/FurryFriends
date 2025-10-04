using System.Collections.ObjectModel;

namespace FurryFriends.Core.Common;

public abstract class EntityBase<T> : BaseEntity<T>
{
    private readonly List<IDomainEvent> _domainEvents = new();
    
    public IReadOnlyCollection<IDomainEvent> DomainEvents => 
        new ReadOnlyCollection<IDomainEvent>(_domainEvents);

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}