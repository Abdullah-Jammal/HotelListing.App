using HotelListing.Api.DTOs.Country;
using HotelListing.Api.Results;

namespace HotelListing.Api.Contracts;

public interface ICountriesService
{
    Task<Result<GetCountryDto>> CreateContryAsync(CreateCountryDto createCountryDto);
    Task<Result> DeleteCountryAsync(int id);
    Task<Result<IEnumerable<GetCountriesDto>>> GetCountriesAsync();
    Task<Result<GetCountryDto>> GetCountryAsync(int id);
    Task<Result> UpdateCountryAsync(int id, UpdateCountryDto updateDto);
}