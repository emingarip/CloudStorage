using System;
using System.Threading;
using System.Threading.Tasks;
using FileStorageService.Application.Interfaces;
using FileStorageService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace FileStorageService.Application.Commands.DeleteFile;

public class DeleteFileCommand : IRequest<Result>
{
    public Guid FileId { get; set; }
}

public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, Result>
{
    private readonly IStoredFileRepository _storedFileRepository;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly ICurrentUserService _currentUserService;

    public DeleteFileCommandHandler(
        IStoredFileRepository storedFileRepository,
        IFileStorageProvider fileStorageProvider,
        ICurrentUserService currentUserService)
    {
        _storedFileRepository = storedFileRepository;
        _fileStorageProvider = fileStorageProvider;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        // Get stored file
        var storedFile = await _storedFileRepository.GetByFileIdAsync(request.FileId);
        if (storedFile == null)
        {
            return Result.Failure("File not found");
        }

        // Check if user has permission to delete the file
        var userId = _currentUserService.UserId;
        if (storedFile.OwnerId != userId && !_currentUserService.IsAdmin)
        {
            return Result.Failure("You don't have permission to delete this file");
        }

        // Mark as deleted in database
        storedFile.Delete();
        await _storedFileRepository.UpdateAsync(storedFile);
        await _storedFileRepository.SaveChangesAsync();

        // Note: We're not physically deleting the file from storage here
        // This is a soft delete. For permanent deletion, we would call:
        // await _fileStorageProvider.DeleteFileAsync(storedFile.Path);

        return Result.Success();
    }
}