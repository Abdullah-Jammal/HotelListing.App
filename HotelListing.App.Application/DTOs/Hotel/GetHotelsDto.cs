namespace HotelListing.App.Application.DTOs.Hotel;

public record GetHotelsDto(
    int Id,
    string Name,
    string Address,
    double Rating,
    int CountryId
);

public record GetHotelsTestDto(
    int Id,
    string Name,
    string Address,
    double Rating
);
