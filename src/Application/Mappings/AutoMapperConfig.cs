using Application.Dto;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public static class AutoMapperConfig
{
    private static MapperConfigurationExpression Config => GetConfig();

    private static MapperConfigurationExpression GetConfig()
    {
        var result = new MapperConfigurationExpression();

        result.CreateMap<Guid, AddFileResultDto>()
            .ForMember(dest => dest.Id, act => act.MapFrom(src => src))
            .ForMember(dest => dest.Success, act => act.MapFrom(src => src != Guid.Empty));

        result.CreateMap<FileMetadata, DownloadFileResultDto>()
            .ForMember(dest => dest.Found, act => act.Ignore())
            .ForMember(dest => dest.SuccessfullyDownloaded, act => act.Ignore())
            .ForMember(dest => dest.Content, act => act.Ignore());

        result.CreateMap<FileMetadata, FileMetadataDto>();

        result.CreateMap<FileDownload, FileDownloadInfoDto>();
        result.CreateMap<ICollection<FileDownload>, FileDownloadInfoListDto>()
            .ForMember(dest => dest.Items, act => act.MapFrom(src => src))
            .ForMember(dest => dest.Count, act => act.MapFrom(src => src.Count));

        return result;
    }

    public static IMapper Initialize()
    {
        return new MapperConfiguration(Config).CreateMapper();
    }
}