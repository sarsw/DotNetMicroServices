using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]     // [controller] will equate to [PlatForms]Controller below
    [ApiController] // decorate to bring in some out of the box API functionality
    public class PlatformsController : ControllerBase       // use the API Base
    {
        private readonly IPlatformRepo _repo;

        public IMapper _mapper { get; }

        public PlatformsController(IPlatformRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
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
        public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto platformCreateDto)        // REST expects something back for a create so give a DTO
        {
            var platformModel = _mapper.Map<Platform/*to*/>(platformCreateDto/*from*/);     // as defined in the automapping setup profile
            _repo.CreatePlatform(platformModel);
            _repo.SaveChanges();        // must do this
            
            // use automapper from model to dto
            var platformReadDto = _mapper.Map<PlatformReadDto/*to*/>(platformModel/*from*/);

            // return a http 201 with a route uri location to get the item - let's be REST compliant! Example uri https://localhost:5001/api/Platforms/4
            return CreatedAtRoute(nameof(GetPlatformById/*the Named api above!*/), new { Id = platformReadDto.Id}/*generate an id*/, platformReadDto/*the body payload*/);
        }
    }
}