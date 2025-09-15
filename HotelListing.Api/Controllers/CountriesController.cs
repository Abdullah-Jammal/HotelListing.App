using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Country;
using HotelListing.Api.DTOs.Hotel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController(HotelListingDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCountriesDto>>> GetCountries()
    {
        var countries = null;
        return Ok(countries);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCountryDto>> GetCountry(int id)
    {
        var country = null;

        if (country == null)
        {
            return NotFound();
        }

        return Ok(country);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateDto)
    {
        if (id != updateDto.Id)
        {
            return BadRequest();
        }

        //var country = await context.Countries.FindAsync(id);

        //if (country == null)
        //{
        //    return NotFound();
        //}

        //country.Id = countryDto.Id;
        //country.Name = countryDto.Name;
        //country.ShortName = countryDto.ShortName;

        //context.Entry(country).State = EntityState.Modified;

        //try
        //{
        //    await context.SaveChangesAsync();
        //}
        //catch (DbUpdateConcurrencyException)
        //{
        //    if (!await CountryExistsAsync(id))
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        throw;
        //    }
        //}

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Country>> PostCountry(CreateCountryDto countryDto)
    {
        var country = new Country 
        {
            Name  = countryDto.Name,
            ShortName = countryDto.ShortName 
        };

        context.Countries.Add(country);
        await context.SaveChangesAsync();

        var createdCountry = new GetCountryDto(country.Id, country.Name, country.ShortName, null);

        return CreatedAtAction("GetCountry", new { id = country.Id }, createdCountry);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var country = await context.Countries.FindAsync(id);
        if (country == null)
        {
            return NotFound();
        }

        context.Countries.Remove(country);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> CountryExistsAsync(int id)
    {
        return await context.Countries.AnyAsync(e => e.Id == id);
    }
}
