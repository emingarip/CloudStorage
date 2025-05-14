using System;
using SharedKernel;

namespace FileMetadataService.Domain.Events
{
    public class FileVersionAddedEvent : DomainEventBase
    {
        public Guid FileId { get; }
        public Guid VersionId { get; }
        public Guid OwnerId { get; }

        public FileVersionAddedEvent(Guid fileId, Guid versionId, Guid ownerId)
        {
            FileId = fileId;
            VersionId = versionId;
            OwnerId = ownerId;
        }
    }
}