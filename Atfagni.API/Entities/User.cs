namespace Atfagni.API.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Passenger";

    // Assurez-vous d'avoir mis le '?' pour les rendre optionnels (pour les passagers)
    public string? CarModel { get; set; }
    public string? CarLicensePlate { get; set; }
    public int? DefaultSeats { get; set; }

    public decimal WalletBalance { get; set; }
} // <-- Vérifiez que cette accolade est bien fermée