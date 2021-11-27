// internal data representation of our item
using System.ComponentModel.DataAnnotations;

namespace PlatformService.Models
{
    public class Platform
    {
        [Key]   //  EntityFramework annotation to identify Id as a key - not strictly needed as it's the default annyway
        [Required]      // not needed as the key is mandatory annyway
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Publisher { get; set; }
        [Required]
        public string Cost { get; set; }
    }
}