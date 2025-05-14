using FileMetadataService.Domain.Enums;

namespace FileMetadataService.Application.DTOs;

public class FileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public string Path { get; set; }
    public Guid OwnerId { get; set; }
    public FileStatus Status { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<FileShareDto> Shares { get; set; }
    public List<FileVersionDto> Versions { get; set; }
    public List<FileActivityDto> Activities { get; set; }
}