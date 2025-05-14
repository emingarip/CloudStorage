using AutoMapper;
using FileStorageService.Application.DTOs;
using FileStorageService.Application.Interfaces;
using FileStorageService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace FileStorageService.Application.Queries.GetFileById;

public class GetFileByIdQuery : IRequest<Result<StoredFileDto>>
{
    public Guid FileId { get; set; }
}

public class GetFileByIdQueryHandler : IRequestHandler<GetFileByIdQuery, Result<StoredFileDto>>
{
    private readonly IStoredFileRepository _storedFileRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetFileByIdQueryHandler(
        IStoredFileRepository storedFileRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _storedFileRepository = storedFileRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<StoredFileDto>> Handle(GetFileByIdQuery request, CancellationToken cancellationToken)
    {
        // Get stored file
        var storedFile = await _storedFileRepository.GetByFileIdAsync(request.FileId);
        if (storedFile == null)
        {
            return Result.Failure<StoredFileDto>("File not found");
        }

        // Check if user has permission to view the file
        var userId = _currentUserService.UserId;
        if (storedFile.OwnerId != userId && !_currentUserService.IsAdmin)
        {
            return Result.Failure<StoredFileDto>("You don't have permission to view this file");
        }

        // Map to DTO and return
        var storedFileDto = _mapper.Map<StoredFileDto>(storedFile);
        return Result.Success(storedFileDto);
    }
}