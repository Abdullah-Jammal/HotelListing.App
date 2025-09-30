using HotelListing.App.Application.DTOs.Hotel;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.App.Application.Contracts
{
    public interface IHotelsServices
    {
        Task<GetHotelDto> CreateHotelAsync(CreateHotelDto hotelDto);
        Task DeleteHotelAsync(int id);
        Task<ActionResult<GetHotelDto>> GetHotelAsync(int id);
        Task<IEnumerable<GetHotelDto>> GetHotelsAsync();
        Task UpdateHotelAsync(int id, UpdateHotelDto updateDto);
    }
}
