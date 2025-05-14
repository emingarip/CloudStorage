using AutoMapper;
using FileStorageService.Application.DTOs;
using FileStorageService.Application.Interfaces;
using FileStorageService.Domain.Entities;
using FileStorageService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using SharedKernel;

namespace FileStorageService.Application.Commands.UploadFile;

public class UploadFileCommand : IRequest<Result<StoredFileDto>>
{
    public Guid FileId { get; set; } = new Guid();
    public IFormFile File { get; set; }
}

public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, Result<StoredFileDto>>
{
    private readonly IStoredFileRepository _storedFileRepository;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UploadFileCommandHandler(
        IStoredFileRepository storedFileRepository,
        IFileStorageProvider fileStorageProvider,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _storedFileRepository = storedFileRepository;
        _fileStorageProvider = fileStorageProvider;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<StoredFileDto>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        // Store file using provider
        var storeResult = await _fileStorageProvider.StoreFileAsync(request.File, request.FileId, userId);
        if (storeResult.IsFailure)
        {
            return Result.Failure<StoredFileDto>(storeResult.Errors.FirstOrDefault());
        }

        var (path, size) = storeResult.Value;

        // Create stored file entity
        var storedFile = new StoredFile(
            request.FileId,
            request.File.FileName,
            request.File.ContentType,
            size,
            path,
            userId);

        // Save to repository
        await _storedFileRepository.AddAsync(storedFile);
        await _storedFileRepository.SaveChangesAsync();

        // Map to DTO and return
        var storedFileDto = _mapper.Map<StoredFileDto>(storedFile);
        return Result.Success(storedFileDto);
    }
}