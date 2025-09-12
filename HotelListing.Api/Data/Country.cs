namespace HotelListing.Api.Data
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ShortName { get; set; }
        public IList<Hotel> Hotels { get; set; } = [];
    }
}
