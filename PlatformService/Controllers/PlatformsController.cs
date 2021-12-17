using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]     // [controller] will equate to [PlatForms]Controller below
    [ApiController] // decorate to bring in some out of the box API functionality
    public class PlatformsController : ControllerBase       // use the API Base
    {
        private readonly IPlatformRepo _repo;
        private IMapper _mapper { get; }
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _msgBusClient;

        public PlatformsController(IPlatformRepo repo, 
            IMapper mapper,
            ICommandDataClient commandDataClient,
            IMessageBusClient msgBusClient)
        {
            // Inject a few useful instances of objects needed here, such as the Command Data Client
            _repo = repo;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _msgBusClient = msgBusClient;
        }

        // simple action to get all platforms
        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()        //only Dtos go out
        {
            Console.WriteLine("GETting Platforms!");

            var platformItem = _repo.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto/*to*/>>(platformItem));       // Automapper uses the map setup in profiles
        }

        [HttpGet("{id}", Name = "GetPlatformById")]     // http signatures must be unique in the api
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platformItem = _repo.GetPlatformById(id);

            Console.WriteLine("GETting Platform "+id);

            if (null==platformItem)
            {
                return NotFound();                                                          // REST best practise is to return 404 if not found
            }
            else
            {
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));                      // Return the found item
            }
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)        // REST expects something back for a create so give a DTO
        {
            var platformModel = _mapper.Map<Platform/*to*/>(platformCreateDto/*from*/);     // as defined in the automapping setup profile
            _repo.CreatePlatform(platformModel);
            _repo.SaveChanges();        // must do this
            
            // use automapper from model to dto
            var platformReadDto = _mapper.Map<PlatformReadDto/*to*/>(platformModel/*from*/);

            // send Sync msg
            try {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);    // could be long running so need to do this aynchronously
            } catch (Exception e) {
                Console.WriteLine($"Error {e.Message} in CreatePlatform Sync when SendPlatformToCommand used");                
            }

            // send ASync msg
            try {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event = "Platform_Published";          // need to add in Event
                _msgBusClient.PublishNewPlatform(platformPublishedDto);     // go tell the world
                await _commandDataClient.SendPlatformToCommand(platformReadDto);    // could be long running so need to do this aynchronously
            } catch (Exception e) {
                Console.WriteLine($"Error {e.Message} in CreatePlatform Async when SendPlatformToCommand used");                
            }

            // return a http 201 with a route uri location to get the item - let's be REST compliant! Example uri https://localhost:5001/api/Platforms/4
            return CreatedAtRoute(nameof(GetPlatformById/*the Named api above!*/), new { Id = platformReadDto.Id}/*generate an id*/, platformReadDto/*the body payload*/);
        }
    }
}