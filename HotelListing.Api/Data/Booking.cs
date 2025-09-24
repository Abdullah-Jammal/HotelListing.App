﻿using HotelListing.Api.Data.Configurations;

namespace HotelListing.Api.Data;

public class Booking
{
    public int Id { get; set; }
    public required string UserId {  get; set; }
    public ApplicationUser? User { get; set; }
    public Hotel? Hotel { get; set; }
    public required int HotelId { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int Guests { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public BookingConfiguration Status { get; set; } = BookingConfiguration.Pending;
}
