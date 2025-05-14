using AutoMapper;
using FileMetadataService.Application.DTOs;
using FileMetadataService.Application.Interfaces;
using FileMetadataService.Domain.Entities;
using FileMetadataService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace FileMetadataService.Application.Commands.CreateFile
{
    public class CreateFileCommand : IRequest<Result<FileDto>>
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public string Path { get; set; }
        public Guid OwnerId { get; set; }
        public string Description { get; set; }
    }

    public class CreateFileCommandHandler : IRequestHandler<CreateFileCommand, Result<FileDto>>
    {
        private readonly IFileRepository _fileRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CreateFileCommandHandler(
            IFileRepository fileRepository,
            ICurrentUserService currentUserService,
            AutoMapper.IMapper mapper)
        {
            _fileRepository = fileRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Result<FileDto>> Handle(CreateFileCommand request, CancellationToken cancellationToken)
        {
            // Ensure the current user is the owner
            if (_currentUserService.UserId != request.OwnerId)
            {
                return Result.Failure<FileDto>("You can only create files for yourself");
            }

            // Create file entity
            var fileEntity = new FileEntity(
                request.Name,
                request.ContentType,
                request.Size,
                request.Path,
                request.OwnerId,
                request.Description);

            // Save to repository
            await _fileRepository.AddAsync(fileEntity);
            await _fileRepository.SaveChangesAsync();

            // Map to DTO and return
            var fileDto = _mapper.Map<FileDto>(fileEntity);
            return Result.Success(fileDto);
        }
    }
}