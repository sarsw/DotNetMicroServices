namespace CommandsService.Dtos
{
    public class PlatformPublishedDto       // this is intended for the RabbitMQ bus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Event { get; set; }
    }
}