using System.ComponentModel.DataAnnotations;

namespace HotelListing.App.Application.DTOs.Hotel;

public class UpdateHotelDto : CreateHotelDto
{
    [Required]
    public int Id { get; set; }
}
