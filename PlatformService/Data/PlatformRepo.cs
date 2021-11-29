using System;
using System.Collections.Generic;
using System.Linq;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly AppDbContext _ctx;

        public PlatformRepo(AppDbContext ctx)
        {
            _ctx = ctx;     // store the injected context
        }
        public void CreatePlatform(Platform plat)
        {
            if (null == plat)
            {
                throw new ArgumentNullException(nameof(plat));
            }

            _ctx.Platforms.Add(plat);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _ctx.Platforms.ToList();
        }

        public Platform GetPlatformById(int id)
        {
            return _ctx.Platforms.FirstOrDefault(p => p.Id == id);
        }

        public bool SaveChanges()
        {
            return (_ctx.SaveChanges() >= 0);
        }
    }
}