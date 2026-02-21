// LocalDriverRequest.cs (Les passagers qui demandent à monter)
using SQLite;
namespace Atfagni.Mobile.Models.Local;

public class LocalDriverRequest
{
    [PrimaryKey]
    public int Id { get; set; } // BookingId
    public string PassengerName { get; set; }
    public string PassengerPhone { get; set; }
    public string TripDescription { get; set; } // "Tindouf ➝ Alger"
    public string Type { get; set; } // "Passenger" ou "Package"
    public string PackageDescription { get; set; }
    public int SeatsRequested { get; set; }
}