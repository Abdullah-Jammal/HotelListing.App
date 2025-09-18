using HotelListing.Api.Contracts;
using HotelListing.Api.DTOs.Hotel;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelsController(IHotelsServices hotelServices) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetHotelsDto>>> GetHotels()
    {
        var hotels = await hotelServices.GetHotelsAsync();
        return Ok(hotels);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<GetHotelDto>>> GetHotel(int id)
    {
        var hotel = await hotelServices.GetHotelAsync(id);
        return Ok(hotel);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutHotel(int id, UpdateHotelDto hotelDto)
    {
        await hotelServices.UpdateHotelAsync(id, hotelDto);
        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<GetHotelDto>> PostHotel(CreateHotelDto hotelDto)
    {
        var hotel = await hotelServices.CreateHotelAsync(hotelDto);
        return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        await hotelServices.DeleteHotelAsync(id);
        return NoContent();
    }

}
