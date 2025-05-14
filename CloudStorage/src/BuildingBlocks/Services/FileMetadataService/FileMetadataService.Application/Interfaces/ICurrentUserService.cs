namespace FileMetadataService.Application.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    bool IsAdmin { get; }
}