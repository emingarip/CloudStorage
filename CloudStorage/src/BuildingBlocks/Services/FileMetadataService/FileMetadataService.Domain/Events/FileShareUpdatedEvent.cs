using System;
using FileMetadataService.Domain.Enums;
using SharedKernel;

namespace FileMetadataService.Domain.Events
{
    public class FileShareUpdatedEvent : DomainEventBase
    {
        public Guid FileId { get; }
        public Guid OwnerId { get; }
        public Guid SharedWithUserId { get; }
        public FilePermission Permission { get; }

        public FileShareUpdatedEvent(Guid fileId, Guid ownerId, Guid sharedWithUserId, FilePermission permission)
        {
            FileId = fileId;
            OwnerId = ownerId;
            SharedWithUserId = sharedWithUserId;
            Permission = permission;
        }
    }
}