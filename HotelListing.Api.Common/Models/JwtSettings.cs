﻿namespace HotelListing.Api.Common.Models;

public sealed class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int DurationInMinutes { get; init; }
}
