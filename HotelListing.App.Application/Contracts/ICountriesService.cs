using HotelListing.Api.Common.Models.Paging;
using HotelListing.Api.Common.Results;
using HotelListing.App.Application.DTOs.Country;

namespace HotelListing.App.Application.Contracts;

public interface ICountriesService
{
    Task<Result<GetCountryDto>> CreateContryAsync(CreateCountryDto createCountryDto);
    Task<Result> DeleteCountryAsync(int id);
    Task<Result<PagedResult<GetCountriesDto>>> GetCountriesAsync(PaginationParameters paginationParameters);
    Task<Result<GetCountryDto>> GetCountryAsync(int id);
    Task<Result> UpdateCountryAsync(int id, UpdateCountryDto updateDto);
}