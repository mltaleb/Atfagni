using Atfagni.Shared.DTOs;
using Atfagni.Shared.Enums;
using SQLite;

namespace Atfagni.Mobile.Models.Local;

public class LocalBooking
{
    [PrimaryKey] public int Id { get; set; }
    public string DriverName { get; set; }
    public string DriverPhone { get; set; } // AJOUTE CE CHAMP
    public string TripDescription { get; set; }
    public string Status { get; set; }
    public string PackageDescription { get; set; }

    public DateTime DepartureDate { get; set; }

    public BookingRequestDto ToDto() => new BookingRequestDto
    {
        Id = this.Id,
        PassengerName = this.DriverName,
        PassengerPhone = this.DriverPhone, // MAPPE LE ICI
        TripDescription = this.TripDescription,
        Status = Enum.Parse<BookingStatus>(this.Status),
        PackageDescription = this.PackageDescription,
        DepartureDate = this.DepartureDate
    };
}