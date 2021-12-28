using System;
using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.EventProcessing
{
    enum EventType
    {
        PlatformPublished, Undetermined
    }

    // take a message and determine what it is to process accordingly
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;       // used to ensure the lifetime of the  service (it's got to be the same lifetime same or greater than the listening service)
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished :      // ultimately we want to add to the DB
                    // process
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        // support method for ProcessEvent
        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("Determining the Event");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch (eventType.Event)
            {
                case "Platform_Published" :
                    Console.WriteLine("Detected Platform_Published event");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("Unknown event!");
                    return EventType.Undetermined;

            }
        }

        private void AddPlatform(string publishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())     // get gnarly!
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();// get ref to repo 
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(publishedMessage);

                try {
                    var plat = _mapper.Map<Platform>(platformPublishedDto);     // get a Plotform model from the one from the wire
                    if (!repo.ExternalPlatformExists(plat.ExternalId))          // avoid tring to add a dup
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                        Console.WriteLine($"AddPlatform saved platform :{publishedMessage}");
                    }
                    else
                    {
                        Console.WriteLine($"AddPlatform error, dup received :{plat.ExternalId}");
                    }
                    
                } catch (Exception err) {
                    Console.WriteLine($"AddPlatform error:{err.Message}");
                }
            }
        }
    }
}