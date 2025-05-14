using SharedKernel;

namespace FileStorageService.Domain.Events;

public class FileRestoredEvent : DomainEventBase
{
    public Guid StoredFileId { get; }
    public Guid FileId { get; }
    public Guid OwnerId { get; }

    public FileRestoredEvent(Guid storedFileId, Guid fileId, Guid ownerId)
    {
        StoredFileId = storedFileId;
        FileId = fileId;
        OwnerId = ownerId;
    }
}