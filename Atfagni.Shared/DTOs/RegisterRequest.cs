namespace Atfagni.Shared.DTOs;

public class RegisterRequest
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Passenger"; // "Passenger" ou "Driver"

    // Facultatifs pour les passagers
    public string? CarModel { get; set; }
    public string? CarLicensePlate { get; set; }
    public int? DefaultSeats { get; set; }
}

public class LoginRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}