using SharedKernel;

namespace FileMetadataService.Domain.Entities;

public class FileVersion : BaseEntity
{
    public Guid FileId { get; private set; }
    public string Path { get; private set; }
    public long Size { get; private set; }
    public string ContentType { get; private set; }
    public int VersionNumber { get; private set; }

    // For EF Core
    private FileVersion() { }

    public FileVersion(Guid fileId, string path, long size, string contentType)
    {
        FileId = fileId;
        Path = path;
        Size = size;
        ContentType = contentType;
        VersionNumber = 1; // This will be updated by the repository
    }
}