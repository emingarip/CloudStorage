using FileStorageService.Application.DTOs;
using FileStorageService.Application.Interfaces;
using FileStorageService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace FileStorageService.Application.Queries.GetDownloadUrl;

public class GetDownloadUrlQuery : IRequest<Result<FileUrlDto>>
{
    public Guid FileId { get; set; }
}

public class GetDownloadUrlQueryHandler : IRequestHandler<GetDownloadUrlQuery, Result<FileUrlDto>>
{
    private readonly IStoredFileRepository _storedFileRepository;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly ICurrentUserService _currentUserService;

    public GetDownloadUrlQueryHandler(
        IStoredFileRepository storedFileRepository,
        IFileStorageProvider fileStorageProvider,
        ICurrentUserService currentUserService)
    {
        _storedFileRepository = storedFileRepository;
        _fileStorageProvider = fileStorageProvider;
        _currentUserService = currentUserService;
    }

    public async Task<Result<FileUrlDto>> Handle(GetDownloadUrlQuery request, CancellationToken cancellationToken)
    {
        // Get stored file
        var storedFile = await _storedFileRepository.GetByFileIdAsync(request.FileId);
        if (storedFile == null)
        {
            return Result.Failure<FileUrlDto>("File not found");
        }

        // Check if file is deleted
        if (storedFile.IsDeleted)
        {
            return Result.Failure<FileUrlDto>("File is deleted");
        }

        // Get download URL from storage provider
        var urlResult = await _fileStorageProvider.GetDownloadUrlAsync(storedFile.Path, storedFile.FileName);
        if (urlResult.IsFailure)
        {
            return Result.Failure<FileUrlDto>(urlResult.Errors.FirstOrDefault());
        }

        return Result.Success(new FileUrlDto { Url = urlResult.Value });
    }
}