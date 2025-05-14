using SharedKernel;

namespace FileStorageService.Domain.Events;

public class FileStoredEvent : DomainEventBase
{
    public Guid StoredFileId { get; }
    public Guid FileId { get; }
    public string FileName { get; }
    public Guid OwnerId { get; }

    public FileStoredEvent(Guid storedFileId, Guid fileId, string fileName, Guid ownerId)
    {
        StoredFileId = storedFileId;
        FileId = fileId;
        FileName = fileName;
        OwnerId = ownerId;
    }
}