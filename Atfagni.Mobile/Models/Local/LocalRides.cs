using Atfagni.Shared.DTOs;
using SQLite;

namespace Atfagni.Mobile.Models.Local;

public class LocalRide
{
    [PrimaryKey]
    public int Id { get; set; }
    public string StartLocation { get; set; }
    public string EndLocation { get; set; }
    public decimal Price { get; set; }
    public DateTime DepartureTime { get; set; }
    public string DriverName { get; set; }
    public bool AcceptsPackages { get; set; }
    public int AvailableSeats { get; set; }

    // LA MÉTHODE DE MAPPING (Sans impact sur SQLite)
    public RideDto ToDto()
    {
        return new RideDto
        {
            Id = this.Id,
            StartLocation = this.StartLocation,
            EndLocation = this.EndLocation,
            Price = this.Price,
            DepartureTime = this.DepartureTime,
            DriverName = this.DriverName,
            AcceptsPackages = this.AcceptsPackages,
            AvailableSeats = this.AvailableSeats
        };
    }
}