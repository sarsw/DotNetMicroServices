using System;
using System.Collections.Generic;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]   // [controller] will get replaced with "commands".  The platformId param will get passed to the handling apis
    [ApiController]
    public class CommandsController : ControllerBase 
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repo, 
            IMapper mapper)
        {
            // Inject a few useful instances of objects needed here, such as the Command Data Client
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)        // example url http://localhost:6000/api/c/platforms/1/commands
        {
            Console.WriteLine($"Getting GetCommandsForPlatform {platformId}");
            
            if (!_repo.PlatformExists(platformId))
            {
                return NotFound();  // http 404 (e.g. {"type": "https://tools.ietf.org/html/rfc7231#section-6.5.4","title": "Not Found", "status": 404,"traceId": "00-e429107877bf27429c5d8f88572bf96f-c71641837a9fe44b-00"})
            }
            
            var commands = _repo.GetCommandsForPlatforms(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto/*map to dto*/>>(commands/*src platform*/));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId) // e.g. /api/c/platforms/1/commands/3
        {
            Console.WriteLine($"Getting GetCommandForPlatform platformId:{platformId} commandId:{commandId}");
            if (!_repo.PlatformExists(platformId))
            {
                return NotFound();  // http 404 (e.g. {"type": "https://tools.ietf.org/html/rfc7231#section-6.5.4","title": "Not Found", "status": 404,"traceId": "00-e429107877bf27429c5d8f88572bf96f-c71641837a9fe44b-00"})
            }
            
            var command = _repo.GetCommand(platformId, commandId);

            if (null == command)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandCreateDto>(command));
        }

        [HttpPost]      // can use same URL as above, just a different verb
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto cmdDto)   // the annotations force some validation to ensure we have the properties of the cmdDto
        {
            // example payload matches the CommandCreateDto: {"HowTo" : "Build a .net project","CommandLine" : "dotnet build"}
            Console.WriteLine($"Getting CreateCommandForPlatform platformId:{platformId}");
            if (!_repo.PlatformExists(platformId))
            {
                return NotFound();  // http 404
            }
 
            var cmd = _mapper.Map<Command>(cmdDto);     // create a command object from the Dto. It should be a good Dto. Could warp this in a try-catch to be sure

            _repo.CreateCommand(platformId, cmd);
            _repo.SaveChanges();        // commit the object & create an id

            var cmdReadDto = _mapper.Map<CommandReadDto>(cmd);

            return CreatedAtRoute(/*endpoint that'll host our resource, as named above*/nameof(GetCommandForPlatform),
                new { platformId = platformId, commandId = cmdReadDto.Id}, 
                cmdReadDto);
        } 
    }
}