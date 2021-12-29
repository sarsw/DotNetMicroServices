using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc
{
    public class     GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepo _repo;
        private readonly IMapper _mapper;

        public GrpcPlatformService(IPlatformRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest req, ServerCallContext ctx)
        {
            var resp = new PlatformResponse();
            var platforms = _repo.GetAllPlatforms();

            foreach (var plat in platforms)
            {
                resp.Platform.Add(_mapper.Map<GrpcPlatformModel>(plat));    // use automapper to populate the collection
            }

            return Task.FromResult(resp);
        }
    }
}