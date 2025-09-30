using HotelListing.Api.Common.Results;
using HotelListing.App.Application.DTOs.Booking;

namespace HotelListing.App.Application.Contracts
{
    public interface IBookingService
    {
        Task<Result> AdminCancelBookingAsync(int hotelId, int bookingId);
        Task<Result> CancelBookingAsync(int hotelId, int bookingId);
        Task<Result> AdminConfirmBookingAsync(int hotelId, int bookingId);
        Task<Result<GetBookingDto>> CreateBookingAsync(CreateBookingDto createBookingDto);
        Task<Result<IEnumerable<GetBookingDto>>> GetBookingForHotelAsync(int hotelId);
        Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto updateBookingDto);
        Task<Result<IEnumerable<GetBookingDto>>> GetUserBookingForHotelAsync(int hotelId);
    }
}