using Atfagni.Shared.DTOs;
using Atfagni.Shared.Enums;
using SQLite;

namespace Atfagni.Mobile.Models.Local;

public class LocalBooking
{
    [PrimaryKey] // SQLite a besoin d'une clé unique
    public int Id { get; set; }

    public string DriverName { get; set; }
    public string DriverPhone { get; set; }
    public string TripDescription { get; set; }
    public string Status { get; set; } // On le stocke en string pour la simplicité
    public DateTime DepartureTime { get; set; }
    public string PackageDescription { get; set; }

    // Dans Models/Local/LocalBooking.cs
    public BookingRequestDto ToDto()
    {
        return new BookingRequestDto
        {
            Id = this.Id,
            PassengerName = this.DriverName,
            PassengerPhone = this.DriverPhone,
            TripDescription = this.TripDescription,
            PackageDescription = this.PackageDescription,
            Status = Enum.Parse<BookingStatus>(this.Status)
        };
    }
}
