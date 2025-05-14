using SharedKernel;

namespace FileMetadataService.Domain.Events;

public class FileCreatedEvent : DomainEventBase
{
    public Guid FileId { get; }
    public string FileName { get; }
    public Guid OwnerId { get; }

    public FileCreatedEvent(Guid fileId, string fileName, Guid ownerId)
    {
        FileId = fileId;
        FileName = fileName;
        OwnerId = ownerId;
    }
}