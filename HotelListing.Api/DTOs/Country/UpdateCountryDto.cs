using HotelListing.Api.DTOs.Country;
using System.ComponentModel.DataAnnotations;

public class UpdateCountryDto : CreateCountryDto
{
    [Required]
    public int Id { get; set; }
}