using HotelListing.Api.Common.Results;
using HotelListing.App.Application.DTOs.Auth;

namespace HotelListing.App.Application.Contracts
{
    public interface IUsersService
    {
        string UserId { get; }

        Task<Result<string>> LoginAsync(LoginDto loginDto);
        Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto);
    }
}