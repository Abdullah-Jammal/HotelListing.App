using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Hotel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services;

public class HotelsServices(HotelListingDbContext context) : IHotelsServices
{
    public async Task<IEnumerable<GetHotelsDto>> GetHotelsAsync()
    {
        var hotels = await context.Hotels
            .Select(h => new GetHotelsDto(
                h.Id,
                h.Name,
                h.Address,
                h.Rating,
                h.CountryId
                )).ToListAsync();

        return hotels;
    }

    public async Task<ActionResult<GetHotelDto>> GetHotelAsync(int id)
    {
        var hotel = await context.Hotels
            .Where(h => h.Id == id)
            .Select(h => new GetHotelDto(
                    h.Id,
                    h.Name,
                    h.Address,
                    h.Rating,
                    h.Country!.Name
                ))
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
        var hotel = await context.Hotels.FindAsync(id);
        if (hotel == null)
        {
            throw new KeyNotFoundException("Hotel not found");
        }
        hotel.Name = updateDto.Name;
        hotel.Address = updateDto.Address;
        hotel.Rating = updateDto.Rating;
        hotel.CountryId = updateDto.Id;
        context.Hotels.Update(hotel);
        await context.SaveChangesAsync();

    }

    public async Task<GetHotelDto> CreateHotelAsync(CreateHotelDto hotelDto)
    {
        var hotel = new Hotel
        {
            Name = hotelDto.Name,
            Address = hotelDto.Address,
            Rating = hotelDto.Rating,
            CountryId = hotelDto.CountryId
        };

        context.Hotels.Add(hotel);
        await context.SaveChangesAsync();
        var countryName = await context.Countries
            .Where(c => c.Id == hotel.CountryId)
            .Select(c => c.Name)
            .FirstOrDefaultAsync();

        return new GetHotelDto(
            hotel.Id,
            hotel.Name,
            hotel.Address,
            hotel.Rating,
            countryName ?? ""
        );
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
