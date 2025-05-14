namespace FileMetadataService.Application.DTOs;

public class FileVersionDto
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public string Path { get; set; }
    public long Size { get; set; }
    public string ContentType { get; set; }
    public int VersionNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}