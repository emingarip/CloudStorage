using FileMetadataService.Domain.Enums;

namespace FileMetadataService.Application.DTOs;

public class FileShareDto
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public Guid UserId { get; set; }
    public FilePermission Permission { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}