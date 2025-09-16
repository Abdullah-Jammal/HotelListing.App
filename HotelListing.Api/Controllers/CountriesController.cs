using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Country;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController(ICountriesService countriesService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCountriesDto>>> GetCountries()
    {
        var countries = await countriesService.GetCountriesAsync();
        return Ok(countries);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCountryDto>> GetCountry(int id)
    {
        var country = await countriesService.GetCountryAsync(id);
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
        await countriesService.UpdateCountryAsync(id, updateDto);
        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Country>> PostCountry(CreateCountryDto countryDto)
    {
        var resultDto = await countriesService.CreateContryAsync(countryDto);
        return CreatedAtAction(nameof(GetCountry), new { id = resultDto.Id }, resultDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var country = await countriesService.GetCountryAsync(id);
        if (country == null)
        {
            return NotFound();
        }
        await countriesService.DeleteCountryAsync(id);
        return NoContent();
    }
}
