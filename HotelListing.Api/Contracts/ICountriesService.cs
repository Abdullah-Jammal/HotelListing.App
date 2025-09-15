namespace HotelListing.Api.Contracts
{
    public interface ICountriesService
    {
        Task<List<GetCountriesDto>> GetCountriesAsync();
    }
}