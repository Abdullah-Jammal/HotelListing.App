using HotelListing.Api.DTOs.Hotel;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Contracts
{
    public interface IHotelsServices
    {
        Task<GetHotelDto> CreateHotelAsync(CreateHotelDto hotelDto);
        Task DeleteHotelAsync(int id);
        Task<ActionResult<GetHotelDto>> GetHotelAsync(int id);
        Task<IEnumerable<GetHotelsDto>> GetHotelsAsync();
        Task UpdateHotelAsync(int id, UpdateHotelDto updateDto);
    }
}
