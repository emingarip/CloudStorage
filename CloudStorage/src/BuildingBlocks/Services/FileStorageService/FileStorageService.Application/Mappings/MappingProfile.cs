using AutoMapper;
using FileStorageService.Application.DTOs;
using FileStorageService.Domain.Entities;

namespace FileStorageService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<StoredFile, StoredFileDto>();
    }
}