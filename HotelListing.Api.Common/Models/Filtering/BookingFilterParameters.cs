using HotelListing.Api.Common.Enums;

namespace HotelListing.Api.Common.Models.Filtering;

public class  BookingFilterParameters : BaseFilterParameters
{
    public int? HotelId { get; set; }
    public int? UserId { get; set; }
    public BookingStatus? Status { get; set; }
    public DateTime? CheckInFrom { get; set; }
    public DateTime? CheckInTo { get; set; }
    public DateTime? CheckOutFrom { get; set; }
    public DateTime? CheckOutTo { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinGuests { get; set; }
    public int? MaxGuests { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
}
