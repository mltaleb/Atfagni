namespace Atfagni.Shared.DTOs
{
    public class BookingRequestDto
    {
        public int Id { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public string PassengerPhone { get; set; } = string.Empty;
        public string Type { get; set; } = "Passenger"; // "Passenger" ou "Package"

        // Attention : On uniformise le nom ici pour éviter la confusion
        public string? PackageDescription { get; set; }

        public int SeatsRequested { get; set; }
        public string TripDescription { get; set; } = string.Empty; // Ex: "Tindouf -> Zouerate"
    }
}