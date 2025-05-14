using System;
using FileMetadataService.Domain.Enums;
using SharedKernel;

namespace FileMetadataService.Domain.Entities
{
    public class FileShare : BaseEntity
    {
        public Guid FileId { get; private set; }
        public Guid UserId { get; private set; }
        public FilePermission Permission { get; private set; }
        public bool IsActive { get; private set; }

        // For EF Core
        private FileShare() { }

        public FileShare(Guid fileId, Guid userId, FilePermission permission)
        {
            FileId = fileId;
            UserId = userId;
            Permission = permission;
            IsActive = true;
        }

        public void UpdatePermission(FilePermission permission)
        {
            Permission = permission;
            MarkAsUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkAsUpdated();
        }

        public void Activate()
        {
            IsActive = true;
            MarkAsUpdated();
        }
    }
}