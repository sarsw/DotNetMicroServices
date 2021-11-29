namespace PlatformService.Dtos
{
    public class PlatformReadDto    // this is the public represemtation of our data
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Publisher { get; set; }
        public string Cost { get; set; }
    }
}