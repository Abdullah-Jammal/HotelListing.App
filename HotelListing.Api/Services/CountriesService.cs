using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Country;
using HotelListing.Api.DTOs.Hotel;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services;

public class CountriesService(HotelListingDbContext context) : ICountriesService
{
    public async Task<IEnumerable<GetCountriesDto>> GetCountriesAsync()
    {
        return await context.Countries.Select(c => new GetCountriesDto(c.Id, c.Name, c.ShortName)).ToListAsync();
    }
    public async Task<GetCountryDto?> GetCountryAsync(int id)
    {
        var country = await context.Countries.Where(c => c.Id == id).Select(c => new GetCountryDto(
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
    public async Task UpdateCountryAsync(int id, UpdateCountryDto updateDto)
    {
        var country = await context.Countries.FindAsync(id) ?? throw new KeyNotFoundException("Country not found");
        country.Name = updateDto.Name;
        country.ShortName = updateDto.ShortName;
        context.Countries.Update(country);
        await context.SaveChangesAsync();
    }
    public async Task<GetCountryDto> CreateContryAsync(CreateCountryDto createCountryDto)
    {
        var country = new Country { Name = createCountryDto.Name, ShortName = createCountryDto.ShortName };
        context.Countries.Add(country);
        await context.SaveChangesAsync();
        var createdCountry = new GetCountryDto(country.Id, country.Name, country.ShortName, null);
        return createdCountry;
    }
    public async Task DeleteCountryAsync(int id)
    {
        var country = await context.Countries.FindAsync(id) ?? throw new KeyNotFoundException("Country not found");
        context.Countries.Remove(country);
        await context.SaveChangesAsync();
    }
}
