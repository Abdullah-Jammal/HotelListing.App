using System.ComponentModel.DataAnnotations;

namespace HotelListing.App.Application.DTOs.Country
{
    public class CreateCountryDto
    {
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }
        [Required]
        [MaxLength(5)]
        public required string ShortName { get; set; }
    }
}
