using SharedKernel;

namespace FileMetadataService.Domain.Events;

public class FileRestoredEvent : DomainEventBase
{
    public Guid FileId { get; }
    public Guid OwnerId { get; }

    public FileRestoredEvent(Guid fileId, Guid ownerId)
    {
        FileId = fileId;
        OwnerId = ownerId;
    }
}