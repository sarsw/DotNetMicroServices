using AutoMapper;
using PlatformService.Models;
using PlatformService.Dtos;

namespace PlatformService.Profiles 
{
    public class PlatformsProfile : Profile
    {
        public PlatformsProfile()
        {
            // map the internal model to the publicly shared DTO
            // AutoMapper will autimatically map fields of the same name in the source and target objects
            CreateMap<Platform/*Source model*/,PlatformReadDto/*Target Dto*/>();    // this is a one-way mapping. To map both ways you need to explicitly code it
            CreateMap<PlatformCreateDto, Platform>();           // map the creation scenerio from the external dto to the internal representation
        }
    }
}