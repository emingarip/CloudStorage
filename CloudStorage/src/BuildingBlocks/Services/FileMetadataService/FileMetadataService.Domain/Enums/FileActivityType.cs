namespace FileMetadataService.Domain.Enums;

public enum FileActivityType
{
    Created = 1,
    Updated = 2,
    Deleted = 3,
    Restored = 4,
    Shared = 5,
    Unshared = 6,
    ShareUpdated = 7,
    VersionAdded = 8,
    Downloaded = 9
}