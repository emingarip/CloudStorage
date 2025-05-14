namespace SharedKernel.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync(BaseEntity entity);

}
