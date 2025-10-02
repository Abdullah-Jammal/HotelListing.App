using HotelListing.Api.Common.Models.Filtering;
using HotelListing.Api.Common.Models.Paging;
using HotelListing.Api.Common.Results;
using HotelListing.App.Application.DTOs.Hotel;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.App.Application.Contracts
{
    public interface IHotelsServices
    {
        Task<GetHotelDto> CreateHotelAsync(CreateHotelDto hotelDto);
        Task DeleteHotelAsync(int id);
        Task<ActionResult<GetHotelDto>> GetHotelAsync(int id);
        Task<Result<PagedResult<GetHotelDto>>> GetHotelsAsync(PaginationParameters paginationParameters, HotelFilterParameters filters);
        Task UpdateHotelAsync(int id, UpdateHotelDto updateDto);
    }
}
