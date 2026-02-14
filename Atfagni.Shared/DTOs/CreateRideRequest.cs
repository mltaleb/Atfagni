namespace Atfagni.Shared.DTOs
{
    public class CreateRideRequest
    {
        public int DriverId { get; set; } // L'ID du chauffeur connecté

        public string StartLocation { get; set; } = string.Empty;
        public string EndLocation { get; set; } = string.Empty;

        public DateTime DepartureTime { get; set; }

        // --- Option Passagers ---
        public bool AcceptsPassengers { get; set; } = true;
        public decimal PricePerSeat { get; set; }
        public int TotalSeats { get; set; }

        // --- Option Colis ---
        public bool AcceptsPackages { get; set; } = false;
        public decimal? PricePerPackage { get; set; } // Prix indicatif
        public string? PackageDescription { get; set; } // "Max 50kg, pas de frigo"
    }
}
