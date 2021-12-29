using System;
using System.Collections.Generic;
using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _cfg;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration cfg, IMapper mapper)
        {
            _cfg = cfg;
            _mapper = mapper;
        }
        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine($"Calling Grpc service {_cfg["GrpcPlatform"]}");
            var channel = GrpcChannel.ForAddress(_cfg["GrpcPlatform"]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var req = new GetAllRequest();
            try
            {
                var reply = client.GetAllPlatforms(req);    // do the remote call
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to call Grpc, error {e.Message}");
            }
            return null;    // for failure
        }
    }
}