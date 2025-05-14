using FileMetadataService.Domain.Enums;

namespace FileMetadataService.Application.DTOs;

public class FileActivityDto
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public FileActivityType ActivityType { get; set; }
    public Guid UserId { get; set; }
    public string Details { get; set; }
    public DateTime CreatedAt { get; set; }
}