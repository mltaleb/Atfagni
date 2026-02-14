using System.ComponentModel.DataAnnotations.Schema;

namespace Atfagni.API.Entities
{
    public class WalletTransaction
    {
        public int Id { get; set; }

        public int UserId { get; set; } // Le chauffeur concerné
        public User? User { get; set; }

        public decimal Amount { get; set; } // Négatif (-2€) si commission, Positif (+50€) si remboursement
        public string Description { get; set; } = string.Empty; // "Commission trajet #42"
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
}