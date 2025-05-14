using MediatR;
namespace SharedKernel;

public abstract class DomainEventBase : INotification
{
    public Guid Id { get; private set; }
    public DateTime OccurredOn { get; private set; }
    protected DomainEventBase()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}
