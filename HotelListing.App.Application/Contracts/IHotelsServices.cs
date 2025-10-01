using HotelListing.Api.Common.Models.Paging;
using HotelListing.App.Application.DTOs.Hotel;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.App.Application.Contracts
{
    public interface IHotelsServices
    {
        Task<GetHotelDto> CreateHotelAsync(CreateHotelDto hotelDto);
        Task DeleteHotelAsync(int id);
        Task<ActionResult<GetHotelDto>> GetHotelAsync(int id);
        Task<PagedResult<GetHotelDto>> GetHotelsAsync(PaginationParameters paginationParameters);
        Task UpdateHotelAsync(int id, UpdateHotelDto updateDto);
    }
}
