using HotelListing.Api.Common.Constants;
using HotelListing.Api.Common.Results;
using HotelListing.App.Application.Contracts;
using HotelListing.App.Application.DTOs.Country;
using HotelListing.App.Application.DTOs.Hotel;
using HotelListing.App.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.App.Application.Services;

public class CountriesService(HotelListingDbContext context) : ICountriesService
{
    public async Task<Result<IEnumerable<GetCountriesDto>>> GetCountriesAsync()
    {
        var countries = await context.Countries.Select(c => new GetCountriesDto(c.Id, c.Name, c.ShortName)).ToListAsync();

        return Result<IEnumerable<GetCountriesDto>>.Success(countries);
    }

    public async Task<Result<GetCountryDto>> GetCountryAsync(int id)
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
        return country is null ? Result<GetCountryDto>.NotFound() : Result<GetCountryDto>.Success(country);
    }

    public async Task<Result> UpdateCountryAsync(int id, UpdateCountryDto updateDto)
    {
        try
        {
            if (id != updateDto.Id)
            {
                return Result.BadRequest(new Error(ErrorCodes.Validation, "The route id does not match the payload id."));
            }
            var country = await context.Countries.FindAsync(id);
            if (country is null)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Country with id {id} not found."));
            }
            var duplicatedName = await context.Countries.AnyAsync(c => c.Name == updateDto.Name);
            if (duplicatedName)
            {
                return Result.Failure(new Error(ErrorCodes.Validation, $"the '{updateDto.Name}' already exist!"));
            }
            country.Name = updateDto.Name;
            country.ShortName = updateDto.ShortName;
            context.Countries.Update(country);
            await context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure();
        }
    }

    public async Task<Result<GetCountryDto>> CreateContryAsync(CreateCountryDto createCountryDto)
    {
        try
        {
            var exist = await context.Countries.AnyAsync(c => c.Name.ToLower().Trim() == createCountryDto.Name.ToLower().Trim());
            if (exist)
            {
                return Result<GetCountryDto>.Failure(new Error(ErrorCodes.Conflict, $"the name '{createCountryDto.Name}' already exist."));
            }
            var country = new Country { Name = createCountryDto.Name, ShortName = createCountryDto.ShortName };
            context.Countries.Add(country);
            await context.SaveChangesAsync();
            var createdCountry = new GetCountryDto(country.Id, country.Name, country.ShortName, null);
            return Result<GetCountryDto>.Success(createdCountry);
        }
        catch (Exception)
        {
            return Result<GetCountryDto>.Failure();
        }
    }

    public async Task<Result> DeleteCountryAsync(int id)
    {
        try
        {
            var country = await context.Countries.FindAsync(id);
            if (country == null)
            {
                return Result.Failure(new Error(ErrorCodes.NotFound, $"The country with id '{id}' not found!"));
            }
            context.Countries.Remove(country);
            await context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure();
        }
    }
}
