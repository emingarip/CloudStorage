using FileStorageService.Domain.Events;
using SharedKernel;

namespace FileStorageService.Domain.Entities;

public class StoredFile : BaseEntity
{
    public Guid FileId { get; private set; }
    public string FileName { get; private set; }
    public string ContentType { get; private set; }
    public long Size { get; private set; }
    public string Path { get; private set; }
    public Guid OwnerId { get; private set; }
    public bool IsDeleted { get; private set; }

    // For EF Core
    private StoredFile() { }

    public StoredFile(Guid fileId, string fileName, string contentType, long size, string path, Guid ownerId)
    {
        FileId = fileId;
        FileName = fileName;
        ContentType = contentType;
        Size = size;
        Path = path;
        OwnerId = ownerId;
        IsDeleted = false;

        AddDomainEvent(new FileStoredEvent(Id, FileId, FileName, OwnerId));
    }

    public void Delete()
    {
        IsDeleted = true;
        MarkAsUpdated();

        AddDomainEvent(new FileDeletedEvent(Id, FileId, OwnerId));
    }

    public void Restore()
    {
        IsDeleted = false;
        MarkAsUpdated();

        AddDomainEvent(new FileRestoredEvent(Id, FileId, OwnerId));
    }
}