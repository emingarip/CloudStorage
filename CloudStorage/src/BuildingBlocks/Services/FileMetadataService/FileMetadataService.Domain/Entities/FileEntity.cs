using FileMetadataService.Domain.Enums;
using FileMetadataService.Domain.Events;
using SharedKernel;

namespace FileMetadataService.Domain.Entities;

public class FileEntity : BaseEntity
{
    public string Name { get; private set; }
    public string ContentType { get; private set; }
    public long Size { get; private set; }
    public string Path { get; private set; }
    public Guid OwnerId { get; private set; }
    public FileStatus Status { get; set; }
    public string Description { get; private set; }
    public List<FileShare> Shares { get; private set; } = new List<FileShare>();
    public List<FileVersion> Versions { get; private set; } = new List<FileVersion>();
    public List<FileActivity> Activities { get; private set; } = new List<FileActivity>();

    private FileEntity() { }

    public FileEntity(string name, string contentType, long size, string path, Guid ownerId, string description = null)
    {
        Name = name;
        ContentType = contentType;
        Size = size;
        Path = path;
        OwnerId = ownerId;
        Status = FileStatus.Active;
        Description = description ?? string.Empty; // Null ise boş string kullan;

        AddDomainEvent(new FileCreatedEvent(Id, name, ownerId));
        AddActivity(FileActivityType.Created, ownerId);
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
        MarkAsUpdated();

        AddDomainEvent(new FileUpdatedEvent(Id, name, OwnerId));
        AddActivity(FileActivityType.Updated, OwnerId);
    }

    public void Delete()
    {
        Status = FileStatus.Deleted;
        MarkAsUpdated();
        MarkAsDeleted();


        AddDomainEvent(new FileDeletedEvent(Id, OwnerId));
        AddActivity(FileActivityType.Deleted, OwnerId);
    }

    public void Restore()
    {
        Status = FileStatus.Active;
        MarkAsUpdated();

        AddDomainEvent(new FileRestoredEvent(Id, OwnerId));
        AddActivity(FileActivityType.Restored, OwnerId);
    }

    public FileShare ShareWith(Guid userId, FilePermission permission)
    {
        var share = new FileShare(Id, userId, permission);
        Shares.Add(share);
        MarkAsUpdated();

        AddDomainEvent(new FileSharedEvent(Id, OwnerId, userId, permission));
        AddActivity(FileActivityType.Shared, OwnerId, $"Shared with user {userId} with {permission} permission");

        return share;
    }

    public void RemoveShare(Guid shareId)
    {
        var share = Shares.FirstOrDefault(s => s.Id == shareId);
        if (share != null)
        {
            Shares.Remove(share);
            MarkAsUpdated();

            AddDomainEvent(new FileUnsharedEvent(Id, OwnerId, share.UserId));
            AddActivity(FileActivityType.Unshared, OwnerId, $"Removed share for user {share.UserId}");
        }
    }

    public void UpdateShare(Guid shareId, FilePermission permission)
    {
        var share = Shares.FirstOrDefault(s => s.Id == shareId);
        if (share != null)
        {
            share.UpdatePermission(permission);
            MarkAsUpdated();

            AddDomainEvent(new FileShareUpdatedEvent(Id, OwnerId, share.UserId, permission));
            AddActivity(FileActivityType.ShareUpdated, OwnerId, $"Updated share for user {share.UserId} to {permission} permission");
        }
    }

    public FileVersion AddVersion(string path, long size, string contentType, Guid userId)
    {
        var version = new FileVersion(Id, path, size, contentType);
        Versions.Add(version);

        // Update file properties
        Path = path;
        Size = size;
        ContentType = contentType;
        MarkAsUpdated();

        AddDomainEvent(new FileVersionAddedEvent(Id, version.Id, OwnerId));
        AddActivity(FileActivityType.VersionAdded, userId, $"Added new version {version.Id}");

        return version;
    }

    private void AddActivity(FileActivityType activityType, Guid userId, string details = null)
    {
        // Aktivite tipi ve kullanıcı ID'sine göre varsayılan detay metni oluştur
        if (string.IsNullOrEmpty(details))
        {
            details = activityType switch
            {
                FileActivityType.Created => "File created",
                FileActivityType.Updated => "File updated",
                FileActivityType.Deleted => "File deleted",
                FileActivityType.Restored => "File restored",
                FileActivityType.Shared => $"File shared with user {userId}",
                FileActivityType.Unshared => $"File share removed for user {userId}",
                FileActivityType.ShareUpdated => $"File share updated for user {userId}",
                FileActivityType.VersionAdded => "New version added",
                _ => $"Activity: {activityType}"
            };
        }

        var activity = new FileActivity(Id, activityType, userId, details);
        Activities.Add(activity);
    }

    public bool CanUserAccess(Guid userId, FilePermission requiredPermission)
    {
        if (OwnerId == userId)
            return true;

        var share = Shares.FirstOrDefault(s => s.UserId == userId && s.IsActive);
        if (share == null)
            return false;

        return share.Permission >= requiredPermission;
    }
}