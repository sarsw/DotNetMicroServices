using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            // source -> target
            // map the internal model to the publicly shared DTO
            // AutoMapper will autimatically map fields of the same name in the source and target objects
            CreateMap<Platform/*Source model*/,PlatformReadDto/*Target Dto*/>();    // this is a one-way mapping. To map both ways you need to explicitly code it
            CreateMap<CommandCreateDto, Command>();           // map the creation scenerio from the external dto to the internal representation
            CreateMap<Command, CommandCreateDto>();           // Used when retrieving the command
            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));  // rxplicit rule to map external id in Platform to the Dto id going over the wire
            CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.PlatformId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Commands, opt => opt.Ignore()); // explicitly do not map Commands
        }
    }
}