namespace Atfagni.API.Entities
{
    public class CommissionRule
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Ex: "Inter-Camps", "Europe-Vers-Camps"
        public decimal FixedFee { get; set; } // Le montant que le chauffeur doit payer (ex: 1€ ou 100 DA)
        public decimal PercentageFee { get; set; } // Si vous voulez un % (ex: 10%)
    }
}