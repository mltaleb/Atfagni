using Atfagni.Shared.Enums; // Notez qu'on utilise le projet Shared ici !

namespace Atfagni.API.Entities
{
    public class Ride
    {
        public int Id { get; set; }

        // Relation avec le chauffeur (Clé étrangère)
        public int DriverId { get; set; }
        public User? Driver { get; set; } // La navigation vers l'objet User

        public string StartLocation { get; set; } = string.Empty;
        public string EndLocation { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }

        public decimal PricePerSeat { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public bool AcceptsPassengers { get; set; } = true; // Accepte des gens ?
        public bool AcceptsPackages { get; set; } = false;  // Accepte des colis ?

        // Description libre pour les colis (ex: "J'ai un grand coffre, je peux prendre un frigo" ou "Juste des petites valises")
        public string? PackageDescription { get; set; }

        // Prix indicatif pour un colis standard (ex: "10€ par sac")
        // Le prix final peut être négocié, mais c'est une base.
        public decimal? PricePerPackage { get; set; }

        public RideStatus Status { get; set; } = RideStatus.Scheduled;

        // Code de validation (ex: 4 chiffres) pour confirmer le trajet
        public string ValidationCode { get; set; } = string.Empty;

        // Montant de la commission due à Atfagni pour ce trajet
        public decimal CommissionAmount { get; set; }
        public int? CommissionRuleId { get; set; }
        public CommissionRule? CommissionRule { get; set; }
    }
}