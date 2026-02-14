using Atfagni.Shared.Enums; // On va devoir mettre à jour l'enum aussi

namespace Atfagni.API.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        public int RideId { get; set; }
        public Ride? Ride { get; set; }

        public int PassengerId { get; set; } // Qui réserve ?
        public User? Passenger { get; set; }

        public int SeatsBooked { get; set; }

        // LE FAMEUX CODE DE CONFIANCE
        public string ValidationCode { get; set; } = string.Empty; // Le passager le reçoit, le chauffeur le rentre

        public BookingStatus Status { get; set; } = BookingStatus.Pending; // "Confirmed", "Cancelled"
        public DateTime BookedAt { get; set; } = DateTime.UtcNow;
        public BookingType Type { get; set; } = BookingType.Passenger;

        // Si c'est un colis, on décrit ce que c'est (ex: "Un sac de riz et deux valises")
        public string? PackageDetails { get; set; }

        // Le prix peut être différent du prix du siège (négocié ou calculé)
        public decimal AgreedPrice { get; set; }
    }
}