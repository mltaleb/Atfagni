using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Atfagni.Shared.DTOs;
using Atfagni.Mobile.Services;

namespace Atfagni.Mobile.ViewModels.Auth;

public partial class RegisterViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    public RegisterViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [ObservableProperty]
    private string fullName;

    [ObservableProperty]
    private string phoneNumber;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private string selectedRole = "Passenger";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDriver))]
    private bool isDriverRole; // Pour l'UI

    public bool IsDriver => IsDriverRole;

    [ObservableProperty]
    private string carModel;

    [ObservableProperty]
    private string seats;

    // Commande pour changer de rôle
    [RelayCommand]
    private void SelectRole(string role)
    {
        SelectedRole = role;
        IsDriverRole = (role == "Driver");
    }

    // Commande pour s'inscrire
    [RelayCommand]
    private async Task Register()
    {
        var request = new RegisterRequest
        {
            FullName = FullName,
            PhoneNumber = PhoneNumber,
            Password = Password,
            Role = SelectedRole,
            CarModel = IsDriverRole ? CarModel : null,
            DefaultSeats = IsDriverRole && int.TryParse(Seats, out var s) ? s : null
        };

        bool success = await _apiService.RegisterAsync(request);

        if (success)
            await Shell.Current.DisplayAlert("Succès", "Compte créé !", "OK");
        else
            await Shell.Current.DisplayAlert("Erreur", "Vérifiez vos informations", "OK");
    }
}