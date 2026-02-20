namespace Atfagni.Shared.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "Driver" ou "Passenger"
    public string PhoneNumber { get; set; } = string.Empty;
    public decimal WalletBalance { get; set; }
    public string? ProfilePictureBase64 { get; set; } // Champ ajouté

}