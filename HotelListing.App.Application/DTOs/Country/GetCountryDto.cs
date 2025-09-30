using HotelListing.App.Application.DTOs.Hotel;

public record GetCountryDto (
    int Id,
    string Name,
    string ShortName,
    List<GetHotelsTestDto>? Hotels
);
