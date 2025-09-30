﻿using HotelListing.Api.Common.Constants;
using HotelListing.App.Application.Contracts;
using HotelListing.App.Application.DTOs.Country;
using HotelListing.App.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CountriesController(ICountriesService countriesService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCountriesDto>>> GetCountries()
    {
        var result = await countriesService.GetCountriesAsync();
        return ToActionResult(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCountryDto>> GetCountry(int id)
    {
        var result = await countriesService.GetCountryAsync(id);
        return ToActionResult(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateDto)
    {
        var result = await countriesService.UpdateCountryAsync(id, updateDto);
        return ToActionResult(result);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<ActionResult<Country>> PostCountry(CreateCountryDto countryDto)
    {
        var result = await countriesService.CreateContryAsync(countryDto);
        if (!result.IsSuccess) return MapErrorsToResponse(result.Errors);
        return CreatedAtAction(nameof(GetCountry), new { id = result.Value!.Id }, result.Value);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var result = await countriesService.DeleteCountryAsync(id);
        return ToActionResult(result);
    }

}
