using SharedKernel;

namespace FileMetadataService.Domain.Events;

public class FileUnsharedEvent : DomainEventBase
{
    public Guid FileId { get; }
    public Guid OwnerId { get; }
    public Guid SharedWithUserId { get; }

    public FileUnsharedEvent(Guid fileId, Guid ownerId, Guid sharedWithUserId)
    {
        FileId = fileId;
        OwnerId = ownerId;
        SharedWithUserId = sharedWithUserId;
    }
}