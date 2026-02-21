// LocalPublishedRide.cs
using Atfagni.Shared.DTOs;
using Atfagni.Shared.Enums;
using SQLite;
namespace Atfagni.Mobile.Models.Local;

public class LocalPublishedRide
{
    [PrimaryKey] public int Id { get; set; }
    public string StartLocation { get; set; }
    public string EndLocation { get; set; }
    public DateTime DepartureTime { get; set; }
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
    public string Status { get; set; }

    public RideDto ToDto()
    {
        return new RideDto
        {
            Id = this.Id,
            StartLocation = this.StartLocation,
            EndLocation = this.EndLocation,
            DepartureTime = this.DepartureTime,
            Price = this.Price,
            AvailableSeats = this.AvailableSeats,
            // Conversion du texte SQLite vers l'Enum C#
            Status = Enum.Parse<RideStatus>(this.Status)
        };
    }
}

