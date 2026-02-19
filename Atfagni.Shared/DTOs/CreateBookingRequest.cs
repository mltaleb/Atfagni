using Atfagni.Shared.Enums;

namespace Atfagni.Shared.DTOs
{
    public class CreateBookingRequest
    {
        public int RideId { get; set; }
        public int PassengerId { get; set; }
        public int Seats { get; set; }
        public BookingType Type { get; set; } // Passenger ou Package
        public string? PackageDescription { get; set; } // La description tapée par l'user
    }
}