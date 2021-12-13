using System;
using System.Collections.Generic;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/[controller]")]   // make this route different to PlatformService (easier when using with a gateway)
    [ApiController]
    public class PlatformsController : ControllerBase 
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepo repo, 
            IMapper mapper)
        {
            // Inject a few useful instances of objects needed here, such as the Command Data Client
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()        // GET /api/c/platforms
        {
            Console.WriteLine("Getting platforms from CommandsService");
            var platformItems = _repo.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto/*map to dto*/>>(platformItems/*src platform*/));
        }
        
        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("Command Service being hit by a POST");
            return Ok("POST received");
        }
    }
}