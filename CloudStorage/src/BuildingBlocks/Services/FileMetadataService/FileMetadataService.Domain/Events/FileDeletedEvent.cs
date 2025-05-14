using SharedKernel;

namespace FileMetadataService.Domain.Events;

public class FileDeletedEvent : DomainEventBase
{
    public Guid FileId { get; }
    public Guid OwnerId { get; }

    public FileDeletedEvent(Guid fileId, Guid ownerId)
    {
        FileId = fileId;
        OwnerId = ownerId;
    }
}