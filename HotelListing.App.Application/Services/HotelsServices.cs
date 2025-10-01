using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Common.Models.Extentions;
using HotelListing.Api.Common.Models.Paging;
using HotelListing.App.Application.Contracts;
using HotelListing.App.Application.DTOs.Hotel;
using HotelListing.App.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.App.Application.Services;

public class HotelsServices(HotelListingDbContext context, IMapper mapper) : IHotelsServices
{
    public async Task<PagedResult<GetHotelDto>> GetHotelsAsync(PaginationParameters paginationParameters)
    {
        var hotels = await context.Hotels
            .Include(q => q.Country)
            .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
            .ToPageResultAsync(paginationParameters);
        return hotels;
    }

    public async Task<ActionResult<GetHotelDto>> GetHotelAsync(int id)
    {
        var hotel = await context.Hotels
            .Where(h => h.Id == id)
            .Include(q => q.Country)
            .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
        if (hotel == null)
        {
            throw new KeyNotFoundException("Hotel not found");
        }
        return hotel;
    }

    public async Task UpdateHotelAsync(int id, UpdateHotelDto updateDto)
    {
        if (id != updateDto.Id)
        {
            throw new ArgumentException("ID mismatch");
        }
        var hotelExist = await context.Hotels.FindAsync(id);
        if (hotelExist == null)
        {
            throw new KeyNotFoundException("Hotel not found");
        }
        mapper.Map(updateDto, hotelExist);
        await context.SaveChangesAsync();
    }

    public async Task<GetHotelDto> CreateHotelAsync(CreateHotelDto hotelDto)
    {
        var hotel = mapper.Map<Hotel>(hotelDto);
        context.Hotels.Add(hotel);
        await context.SaveChangesAsync();
        var countryName = await context.Countries
            .Where(c => c.Id == hotel.CountryId)
            .Select(c => c.Name)
            .FirstOrDefaultAsync();

        var returnObj = mapper.Map<GetHotelDto>(hotel);
        return returnObj;
    }

    public async Task DeleteHotelAsync(int id)
    {
        var hotel = await context.Hotels.FindAsync(id);
        if (hotel == null)
        {
            throw new KeyNotFoundException("Hotel not found");
        }
        context.Hotels.Remove(hotel);
        await context.SaveChangesAsync();
    }
}
