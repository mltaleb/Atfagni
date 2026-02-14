using Atfagni.Shared.Enums;

namespace Atfagni.Shared.DTOs
{
    public class RideDto
    {
        public int Id { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string StartLocation { get; set; } = string.Empty;
        public string EndLocation { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }
        public RideStatus Status { get; set; }
        public bool AcceptsPassengers { get; set; }
        public bool AcceptsPackages { get; set; }
        public string? PackageDescription { get; set; }
        public decimal? PricePerPackage { get; set; }
    }
}
