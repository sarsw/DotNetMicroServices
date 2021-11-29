using System.ComponentModel.DataAnnotations;

namespace PlatformService.Dtos
{
    public class PlatformCreateDto      // fields of data external party would provide to create a new item (note Id is something this we would create)
    {
        [Required]                      // left this in to allow for data validation
        public string Name { get; set; }
        [Required]
        public string Publisher { get; set; }
        [Required]
        public string Cost { get; set; }        
    }
}