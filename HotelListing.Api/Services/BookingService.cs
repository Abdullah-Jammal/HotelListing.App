using HotelListing.Api.Constants;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.Data.Enums;
using HotelListing.Api.DTOs.Booking;
using HotelListing.Api.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services;

public class BookingService(HotelListingDbContext context, IUsersService usersService) : IBookingService
{
    public async Task<Result<IEnumerable<GetBookingDto>>> GetBookingForHotelAsync(int hotelId)
    {
        var hotelExist = await context.Hotels.AnyAsync(h => h.Id == hotelId);
        if (!hotelExist)
        {
            return Result<IEnumerable<GetBookingDto>>.Failure(new Error(ErrorCodes.NotFound, $"hotel with id {hotelId} not found!"));
        }
        var bookings = await context.Bookings
            .Where(b => b.HotelId == hotelId)
            .OrderBy(b => b.CheckIn)
            .Select(b => new GetBookingDto
            (
                b.Id,
                b.HotelId,
                b.Hotel!.Name,
                b.CheckIn,
                b.CheckOut,
                b.Guests,
                b.TotalPrice,
                b.Status.ToString(),
                b.CreatedAtUtc,
                b.UpdatedAtUtc
            ))
            .ToListAsync();
        return Result<IEnumerable<GetBookingDto>>.Success(bookings);
    }

    public async Task<Result<GetBookingDto>> CreateBookingAsync(CreateBookingDto createBookingDto)
    {
        var userId = usersService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "User is required"));
        }

        var hotel = await context.Hotels.FirstOrDefaultAsync(h => h.Id == createBookingDto.HotelId);
        if (hotel is null)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.NotFound, $"hotel with id {createBookingDto.HotelId} not found!"));
        }

        var overlaps = await context.Bookings
            .AnyAsync(b => b.HotelId == createBookingDto.HotelId &&
                           b.Status != BookingStatus.Cancelled &&
                           createBookingDto.CheckIn < b.CheckOut &&
                           createBookingDto.CheckOut > b.CheckIn &&
                           b.UserId == userId
                      );
        if(overlaps)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict, "You have an overlapping booking for the selected dates."));
        }

        var nights = createBookingDto.CheckOut.DayNumber - createBookingDto.CheckIn.DayNumber;

        var totalPrice = hotel.PerRatingNight * nights;

        var booking = new Booking
        {
            HotelId = createBookingDto.HotelId,
            UserId = userId,
            CheckIn = createBookingDto.CheckIn,
            CheckOut = createBookingDto.CheckOut,
            Guests = createBookingDto.Guests,
            TotalPrice = totalPrice,
            Status = BookingStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();
        var created = new GetBookingDto
        (
            booking.Id,
            booking.HotelId,
            hotel.Name,
            booking.CheckIn,
            booking.CheckOut,
            booking.Guests,
            totalPrice,
            booking.Status.ToString(),
            booking.CreatedAtUtc,
            booking.UpdatedAtUtc
        );
        return Result<GetBookingDto>.Success(created);
    }

    public async Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto updateBookingDto)
    {
        var userId = usersService.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "User is required"));
        }

        var overlaps = await context.Bookings
    .AnyAsync(b => b.Id != bookingId &&
                   b.HotelId == hotelId &&
                   b.Status != BookingStatus.Cancelled &&
                   updateBookingDto.CheckIn < b.CheckOut &&
                   updateBookingDto.CheckOut > b.CheckIn &&
                   b.UserId == userId
              );
        if (overlaps)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict, "You have an overlapping booking for the selected dates."));
        }
        var booking = await context.Bookings.Include(b => b.Hotel).FirstOrDefaultAsync(b => b.Id == bookingId && b.HotelId == hotelId && b.UserId == userId);
        if (booking is null)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.NotFound, $"booking with id {bookingId} for hotel with id {hotelId} not found!"));
        }
        if(booking.Status == BookingStatus.Cancelled)
        {
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "Cancelled bookings cannot be updated."));
        }
        var perNights = booking.Hotel!.PerRatingNight;
        booking.CheckIn = updateBookingDto.CheckIn;
        booking.CheckOut = updateBookingDto.CheckOut;
        booking.Guests = updateBookingDto.Guests;
        booking.TotalPrice = perNights * (updateBookingDto.CheckOut.DayNumber - updateBookingDto.CheckIn.DayNumber);
        booking.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();
        var updated = new GetBookingDto
        (
            booking.Id,
            booking.HotelId,
            booking.Hotel!.Name,
            booking.CheckIn,
            booking.CheckOut,
            booking.Guests,
            booking.TotalPrice,
            booking.Status.ToString(),
            booking.CreatedAtUtc,
            booking.UpdatedAtUtc
        );
        return Result<GetBookingDto>.Success(updated);
    }

    public async Task<Result> CancelBookingAsync(int hotelId, int bookingId)
    {
        var userId = usersService.UserId;
        var booking = await context.Bookings.Include(b => b.Hotel).FirstOrDefaultAsync(b => b.Id == bookingId && b.HotelId == hotelId && b.UserId == userId);
        if (booking is null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"booking with id {bookingId} for hotel with id {hotelId} not found!"));
        }

        if (booking!.Status == BookingStatus.Cancelled)
        {
            return Result.Failure(new Error(ErrorCodes.Validation, "Booking has already been cancelled!"));
        }

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> AdminConfirmBookingAsync(int hotelId, int bookingId)
    {
        var userId = usersService.UserId;
        var isHotelAdminUser = await context.Admins.AnyAsync(q => q.UserId == userId && q.HotelId == hotelId);

        if (isHotelAdminUser)
        {
            return Result.Failure(new Error(ErrorCodes.Forbid, "You are not the admin of the selected hotel"));
        }
        var booking = await context.Bookings.Include(b => b.Hotel).FirstOrDefaultAsync(b => b.Id == bookingId && b.HotelId == hotelId);

        if (booking!.Status == BookingStatus.Confirmed)
        {
            return Result.Failure(new Error(ErrorCodes.Validation, "Booking has already been confirmed!"));
        }
        if (booking!.Status == BookingStatus.Cancelled)
        {
            return Result.Failure(new Error(ErrorCodes.Validation, "Cancelled bookings cannot be confirmed!"));
        }
        booking.Status = BookingStatus.Confirmed;
        booking.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> AdminCancelBookingAsync(int hotelId, int bookingId)
    {
        var userId = usersService.UserId;
        var isHotelAdminUser = await context.Admins.AnyAsync(q => q.UserId == userId && q.HotelId == hotelId);

        if (isHotelAdminUser) {
            return Result.Failure(new Error(ErrorCodes.Forbid, "You are not the admin of the selected hotel"));
        }
        var booking = await context.Bookings.Include(b => b.Hotel).FirstOrDefaultAsync(b => b.Id == bookingId && b.HotelId == hotelId);
        if (booking is null)
        {
            return Result.Failure(new Error(ErrorCodes.NotFound, $"booking with id {bookingId} for hotel with id {hotelId} not found!"));
        }

        if (booking!.Status == BookingStatus.Cancelled)
        {
            return Result.Failure(new Error(ErrorCodes.Validation, "Cancelled bookings cannot be confirmed!"));
        }

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<IEnumerable<GetBookingDto>>> GetUserBookingForHotelAsync(int hotelId)
    {
        var userId = usersService.UserId;
        var hotelExist = await context.Hotels.AnyAsync(h => h.Id == hotelId);
        if (!hotelExist)
        {
            return Result<IEnumerable<GetBookingDto>>.Failure(new Error(ErrorCodes.NotFound, $"hotel with id {hotelId} not found!"));
        }
        var bookings = await context.Bookings
            .Where(b => b.HotelId == hotelId && b.UserId == userId)
            .OrderBy(b => b.CheckIn)
            .Select(b => new GetBookingDto
            (
                b.Id,
                b.HotelId,
                b.Hotel!.Name,
                b.CheckIn,
                b.CheckOut,
                b.Guests,
                b.TotalPrice,
                b.Status.ToString(),
                b.CreatedAtUtc,
                b.UpdatedAtUtc
            ))
            .ToListAsync();
        return Result<IEnumerable<GetBookingDto>>.Success(bookings);
    }
}
