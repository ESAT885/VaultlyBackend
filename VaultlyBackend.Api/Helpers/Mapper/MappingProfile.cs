using AutoMapper;
using VaultlyBackend.Api.Models.Dtos.Users;
using VaultlyBackend.Api.Models.Dtos.Videos;
using VaultlyBackend.Api.Models.Entites;

namespace VaultlyBackend.Api.Helpers.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Video, VideoDto>().ReverseMap();
        }
    }
}
