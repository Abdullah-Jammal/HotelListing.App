using System.ComponentModel.DataAnnotations;

namespace HotelListing.App.Application.DTOs.Booking;

public record UpdateBookingDto
(
    DateOnly CheckIn,
    DateOnly CheckOut,
    [Required][Range(1, 10)] int Guests
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckOut <= CheckIn)
        {
            yield return new ValidationResult(
                "Check-out date must be after than check-in date.",
                [nameof(CheckOut), nameof(CheckIn)]);
        }
    }
}
