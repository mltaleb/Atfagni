using Atfagni.Shared.Enums; // N'oublie pas l'using

namespace Atfagni.Shared.DTOs
{
    public class BookingRequestDto
    {
        public int Id { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public string PassengerPhone { get; set; } = string.Empty;
        public string Type { get; set; } = "Passenger";

        // Vraie description (ex: "Sac de dattes")
        public string? PackageDescription { get; set; }

        // On utilise ton ENUM ici !
        public BookingStatus Status { get; set; }

        public int SeatsRequested { get; set; }
        public string TripDescription { get; set; } = string.Empty;

        public DateTime DepartureDate { get; set; }
    }
}