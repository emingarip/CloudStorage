using SharedKernel;

namespace FileStorageService.Domain.Events;

public class FileDeletedEvent : DomainEventBase
{
    public Guid StoredFileId { get; }
    public Guid FileId { get; }
    public Guid OwnerId { get; }

    public FileDeletedEvent(Guid storedFileId, Guid fileId, Guid ownerId)
    {
        StoredFileId = storedFileId;
        FileId = fileId;
        OwnerId = ownerId;
    }
}