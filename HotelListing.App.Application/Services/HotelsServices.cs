using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Common.Models.Extentions;
using HotelListing.Api.Common.Models.Filtering;
using HotelListing.Api.Common.Models.Paging;
using HotelListing.Api.Common.Results;
using HotelListing.App.Application.Contracts;
using HotelListing.App.Application.DTOs.Hotel;
using HotelListing.App.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.App.Application.Services;

public class HotelsServices(HotelListingDbContext context, IMapper mapper) : IHotelsServices
{
    public async Task<Result<PagedResult<GetHotelDto>>> GetHotelsAsync(PaginationParameters paginationParameters, HotelFilterParameters filters)
    {
        var query = context.Hotels.AsQueryable();

        if (filters.CountryId.HasValue)
        {
            query = query.Where(h => h.CountryId == filters.CountryId);
        }
        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            query = query.Where(h => h.Name.Contains(filters.Search) || h.Address.Contains(filters.Search));
        }
        if (!string.IsNullOrWhiteSpace(filters.Location))
        {
            var lowerCaseLocationString = filters.Location.Trim().ToLower();
            query = query.Where(h => h.Address.Contains(lowerCaseLocationString));
        }
        if (filters.MinRating.HasValue)
        {
            query = query.Where(h => h.Rating >= filters.MinRating.Value);
        }
        if(filters.MaxRating.HasValue)
        {
            query = query.Where(h => h.Rating <= filters.MaxRating.Value);
        }
        if(filters.MinPrice.HasValue)
        {
            query = query.Where(h => h.PerRatingNight >= filters.MinPrice.Value);
        }
        if(filters.MaxPrice.HasValue)
        {
            query = query.Where(h => h.PerRatingNight <= filters.MaxPrice.Value);
        }

        query = filters.SortBy?.ToLower() switch
        {
            "name" => filters.SortDescending ? query.OrderByDescending(h => h.Name) : query.OrderBy(h => h.Name),
            "rating" => filters.SortDescending ? query.OrderByDescending(h => h.Rating) : query.OrderBy(h => h.Rating),
            "price" => filters.SortDescending ? query.OrderByDescending(h => h.PerRatingNight) : query.OrderBy(h => h.PerRatingNight),
            _ => query.OrderBy(h => h.Name)
        };

        var hotels = await query
            .Include(q => q.Country)
            .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
            .ToPageResultAsync(paginationParameters);
        return Result<PagedResult<GetHotelDto>>.Success(hotels);
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
