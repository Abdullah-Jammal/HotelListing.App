
using HotelListing.Api.DTOs.Hotel;

public record GetCountryDto (
    int Id,
    string Name,
    string ShortName,
    List<GetHotelsTestDto>? Hotels
);
