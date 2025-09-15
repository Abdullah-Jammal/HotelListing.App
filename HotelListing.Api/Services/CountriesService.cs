using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Hotel;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services;

public class CountriesService(HotelListingDbContext context) : ICountriesService
{
    public async Task<List<GetCountriesDto>> GetCountriesAsync() =>
        await context.Countries
        .Select(c => new GetCountriesDto
        (
            c.Id,
            c.Name,
            c.ShortName
        ))
        .ToListAsync();

    public async Task<GetCountryDto?> GetCountryByIdAsync(int id)
    {
        var country = await context.Countries
            .Where(c => c.Id == id)
            .Select(c => new GetCountryDto(
                c.Id,
                c.Name,
                c.ShortName,
                c.Hotels.Select(h => new GetHotelsTestDto
                (
                    h.Id,
                    h.Name,
                    h.Address,
                    h.Rating
                )).ToList()
             ))
            .FirstOrDefaultAsync();
        return country ?? null;
    }
    
}
