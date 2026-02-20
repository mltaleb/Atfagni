using Atfagni.Mobile.Services;
using Atfagni.Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Atfagni.Mobile.ViewModels.Auth;

public partial class LoginViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty] private string phone;
    [ObservableProperty] private string password;

    // Propriété indispensable pour le LoadingOverlay
    [ObservableProperty] private bool isBusy;

    public LoginViewModel(ApiService apiService) => _apiService = apiService;

    [RelayCommand]
    private async Task Login()
    {
        // Sécurité anti-double clic
        if (IsBusy) return;

        // 1. Validation des champs
        if (string.IsNullOrWhiteSpace(Phone) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Erreur", "Veuillez remplir tous les champs.", "OK");
            return;
        }

        try
        {
            // ON ACTIVE LE SPINNER
            IsBusy = true;

            var request = new LoginRequest { PhoneNumber = Phone, Password = Password };

            // 2. Appel à l'API (Cela peut prendre du temps sur Render)
            var user = await _apiService.LoginAsync(request);

            // 3. VÉRIFICATION
            if (user != null)
            {
                Preferences.Set("UserId", user.Id.ToString());
                Preferences.Set("UserRole", user.Role);
                Preferences.Set("UserName", user.FullName);
                Preferences.Set("UserPhone", user.PhoneNumber); // Utile pour le profil

                await Shell.Current.GoToAsync("//HomePage");
            }
            else
            {
                await Shell.Current.DisplayAlert("Échec", "Numéro ou mot de passe incorrect.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CRASH LOGIN : {ex.Message}");
            await Shell.Current.DisplayAlert("Erreur Technique", "Vérifiez votre connexion internet.", "OK");
        }
        finally
        {
            // ON CACHE LE SPINNER (toujours exécuté, même en cas d'erreur)
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToRegister()
    {
        await Shell.Current.GoToAsync("RegisterPage");
    }
}