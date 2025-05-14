using FileMetadataService.Domain.Enums;
using SharedKernel;

namespace FileMetadataService.Domain.Entities;

public class FileActivity : BaseEntity
{
    public Guid FileId { get; private set; }
    public FileActivityType ActivityType { get; private set; }
    public Guid UserId { get; private set; }
    public string Details { get; private set; }

    // For EF Core
    private FileActivity() { }

    public FileActivity(Guid fileId, FileActivityType activityType, Guid userId, string details = null)
    {
        FileId = fileId;
        ActivityType = activityType;
        UserId = userId;
        Details = details;
    }
}