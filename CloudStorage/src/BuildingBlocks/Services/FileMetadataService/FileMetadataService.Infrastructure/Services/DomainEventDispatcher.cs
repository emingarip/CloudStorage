using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel;
using SharedKernel.Interfaces;

namespace FileMetadataService.Infrastructure.Services;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly ILogger<DomainEventDispatcher> _logger;
    private readonly IPublisher _mediator;

    public DomainEventDispatcher(ILogger<DomainEventDispatcher> logger, IPublisher mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task DispatchEventsAsync(BaseEntity entity)
    {
        foreach (var domainEvent in entity.DomainEvents)
        {
            _logger.LogInformation("Dispatching domain event {EventName}", domainEvent.GetType().Name);

            try
            {
                await _mediator.Publish(domainEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dispatching domain event {EventName}", domainEvent.GetType().Name);
                throw;
            }
        }
    }
}