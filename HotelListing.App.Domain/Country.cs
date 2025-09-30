namespace HotelListing.App.Domain;

public class Country
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string ShortName { get; set; }
    public IList<Hotel> Hotels { get; set; } = [];
}
