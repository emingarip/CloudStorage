using FileMetadataService.Domain.Enums;
using SharedKernel;

namespace FileMetadataService.Domain.Events;

public class FileSharedEvent : DomainEventBase
{
    public Guid FileId { get; }
    public Guid OwnerId { get; }
    public Guid SharedWithUserId { get; }
    public FilePermission Permission { get; }

    public FileSharedEvent(Guid fileId, Guid ownerId, Guid sharedWithUserId, FilePermission permission)
    {
        FileId = fileId;
        OwnerId = ownerId;
        SharedWithUserId = sharedWithUserId;
        Permission = permission;
    }
}