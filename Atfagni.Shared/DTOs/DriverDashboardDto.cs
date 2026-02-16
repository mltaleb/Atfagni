namespace Atfagni.Shared.DTOs
{
    public class DriverDashboardDto
    {
        public decimal WalletBalance { get; set; }
        public int PendingRequestsCount { get; set; } // La bulle rouge de notification
        public RideDto? NextRide { get; set; } // Le prochain trajet prévu (peut être null)
        public int CompletedRidesCount { get; set; } // Statistique "Total voyages"
    }
}