namespace FileStorageService.Application.DTOs;

public class StoredFileDto
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public string Path { get; set; }
    public Guid OwnerId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}