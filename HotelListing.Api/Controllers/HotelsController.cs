using HotelListing.Api.Common.Constants;
using HotelListing.Api.Common.Models.Paging;
using HotelListing.App.Application.Contracts;
using HotelListing.App.Application.DTOs.Hotel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class HotelsController(IHotelsServices hotelServices) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<GetHotelsDto>>> GetHotels([FromQuery] PaginationParameters paginationParameters)
    {
        var hotels = await hotelServices.GetHotelsAsync(paginationParameters);
        return Ok(hotels);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<GetHotelDto>>> GetHotel(int id)
    {
        var hotel = await hotelServices.GetHotelAsync(id);
        return Ok(hotel);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<IActionResult> PutHotel(int id, UpdateHotelDto hotelDto)
    {
        await hotelServices.UpdateHotelAsync(id, hotelDto);
        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<ActionResult<GetHotelDto>> PostHotel(CreateHotelDto hotelDto)
    {
        var hotel = await hotelServices.CreateHotelAsync(hotelDto);
        return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        await hotelServices.DeleteHotelAsync(id);
        return NoContent();
    }
}
