using System;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService 
{
    [Route("api/c/[controller]")]   // make this route different to PlatformService (easier when using with a gateway)
    [ApiController]
    public class PlatformsController : ControllerBase 
    {
        public PlatformsController()
        {
            
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("Command Service being hit by a POST");
            return Ok("POST received");
        }
    }
}