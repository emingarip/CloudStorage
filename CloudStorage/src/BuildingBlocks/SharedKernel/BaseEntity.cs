namespace SharedKernel;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }

    private List<DomainEventBase> _domainEvents = new List<DomainEventBase>();

    public IReadOnlyCollection<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public void AddDomainEvent(DomainEventBase domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(DomainEventBase domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void MarkAsDeleted()
    {

        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
    public void MarkAsNotDeleted()
    {
        IsDeleted = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }



}
