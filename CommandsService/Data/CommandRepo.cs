using System;
using System.Collections.Generic;
using System.Linq;
using CommandsService.Models;

namespace CommandsService.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext _ctx;

        public CommandRepo(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        public void CreateCommand(int platformId, Command command)  // used via POST /api/c/platforms/{platformId}/commands/
        {
            if (null == command)
            {
                throw new ArgumentNullException(nameof(command));
            }

            command.PlatformId = platformId;        // complete the command by connecting the platformId
            _ctx.Commands.Add(command);
        }

        public void CreatePlatform(Platform plat)
        {
            if (null == plat)
            {
                throw new ArgumentNullException(nameof(plat));
            }

            _ctx.Platforms.Add(plat);
        }

        public IEnumerable<Platform> GetAllPlatforms()  // used via GET /api/c/platforms/
        {
            return _ctx.Platforms.ToList();
        }

        public Command GetCommand(int platformId, int commandId)  // used via GET /api/c/platforms//{platformId}/commands/{commandsId}
        {
            return _ctx.Commands.Where(c => c.PlatformId == platformId && c.Id == commandId).FirstOrDefault();
        }

        public IEnumerable<Command> GetCommandsForPlatforms(int platformId)  // used via GET /api/c/platforms//{platformId}/commands
        {
            return _ctx.Commands.Where(c => c.PlatformId == platformId).OrderBy(c => c.Platform.Name);
        }

        public bool PlatformExists(int platformId)
        {
            return _ctx.Platforms.Any(p => p.Id == platformId);
        }

        public bool SaveChanges()
        {
            return _ctx.SaveChanges() >= 0;
        }
    }
}