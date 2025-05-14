using AutoMapper;
using FileMetadataService.Application.DTOs;
using FileMetadataService.Application.Interfaces;
using FileMetadataService.Domain.Enums;
using FileMetadataService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace FileMetadataService.Application.Commands.UpdateFile;

public class UpdateFileCommand : IRequest<Result<FileDto>>
{
    public Guid FileId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class UpdateFileCommandHandler : IRequestHandler<UpdateFileCommand, Result<FileDto>>
{
    private readonly IFileRepository _fileRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UpdateFileCommandHandler(
        IFileRepository fileRepository,
        ICurrentUserService currentUserService,
        AutoMapper.IMapper mapper)
    {
        _fileRepository = fileRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<FileDto>> Handle(UpdateFileCommand request, CancellationToken cancellationToken)
    {
        // Get file
        var file = await _fileRepository.GetByIdAsync(request.FileId);
        if (file == null)
        {
            return Result.Failure<FileDto>("File not found");
        }

        // Check if user has permission to update the file
        var userId = _currentUserService.UserId;
        if (!await _fileRepository.UserHasAccessToFileAsync(file.Id, userId, FilePermission.Write))
        {
            return Result.Failure<FileDto>("You don't have permission to update this file");
        }

        // Update file
        file.Update(request.Name, request.Description);

        // Save changes
        await _fileRepository.UpdateAsync(file);
        await _fileRepository.SaveChangesAsync();

        // Map to DTO and return
        var fileDto = _mapper.Map<FileDto>(file);
        return Result.Success(fileDto);
    }
}