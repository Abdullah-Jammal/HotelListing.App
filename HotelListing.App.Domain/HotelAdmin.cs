namespace HotelListing.App.Domain;

public class HotelAdmin
{
    public int Id { get; set; }
    public ApplicationUser? User { get; set; }
    public required string UserId {  get; set; }
    public Hotel? Hotel { get; set; }
    public int HotelId { get; set; }
}
