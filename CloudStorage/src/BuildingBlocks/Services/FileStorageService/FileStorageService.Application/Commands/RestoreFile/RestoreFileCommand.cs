using FileStorageService.Application.Interfaces;
using FileStorageService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace FileStorageService.Application.Commands.RestoreFile;

public class RestoreFileCommand : IRequest<Result>
{
    public Guid FileId { get; set; }
}

public class RestoreFileCommandHandler : IRequestHandler<RestoreFileCommand, Result>
{
    private readonly IStoredFileRepository _storedFileRepository;
    private readonly ICurrentUserService _currentUserService;

    public RestoreFileCommandHandler(
        IStoredFileRepository storedFileRepository,
        ICurrentUserService currentUserService)
    {
        _storedFileRepository = storedFileRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(RestoreFileCommand request, CancellationToken cancellationToken)
    {
        // Get stored file
        var storedFile = await _storedFileRepository.GetByFileIdAsync(request.FileId);
        if (storedFile == null)
        {
            return Result.Failure("File not found");
        }

        // Check if user has permission to restore the file
        var userId = _currentUserService.UserId;
        if (storedFile.OwnerId != userId && !_currentUserService.IsAdmin)
        {
            return Result.Failure("You don't have permission to restore this file");
        }

        // Restore file
        storedFile.Restore();
        await _storedFileRepository.UpdateAsync(storedFile);
        await _storedFileRepository.SaveChangesAsync();

        return Result.Success();
    }
}