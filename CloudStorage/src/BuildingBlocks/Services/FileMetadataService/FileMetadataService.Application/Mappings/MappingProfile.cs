using AutoMapper;
using FileMetadataService.Application.DTOs;
using FileMetadataService.Domain.Entities;

namespace FileMetadataService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<FileEntity, FileDto>();
        CreateMap<Domain.Entities.FileShare, FileShareDto>();
        CreateMap<FileVersion, FileVersionDto>();
        CreateMap<FileActivity, FileActivityDto>();
    }
}