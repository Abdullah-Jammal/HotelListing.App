using HotelListing.Api.Common.Models.Paging;
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
        Task<Result<PagedResult<GetBookingDto>>> GetBookingForHotelAsync(int hotelId, PaginationParameters paginationParameters);
        Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto updateBookingDto);
        Task<Result<PagedResult<GetBookingDto>>> GetUserBookingForHotelAsync(int hotelId, PaginationParameters paginationParameters);
    }
}