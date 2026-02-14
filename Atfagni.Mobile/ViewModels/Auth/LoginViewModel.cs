using Atfagni.Mobile.Services;
using Atfagni.Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace Atfagni.Mobile.ViewModels.Auth;

public partial class LoginViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty] private string phone;
    [ObservableProperty] private string password;

    public LoginViewModel(ApiService apiService) => _apiService = apiService;

    [RelayCommand]
    private async Task Login()
    {
        // 1. Validation des champs avec message utilisateur
        if (string.IsNullOrWhiteSpace(Phone) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlertAsync("Erreur", "Veuillez remplir tous les champs.", "OK");
            return;
        }

        try
        {
            var request = new LoginRequest { PhoneNumber = Phone, Password = Password };

            // 2. Appel à l'API
            var user = await _apiService.LoginAsync(request);

            // 3. VÉRIFICATION D'ABORD !
            if (user != null)
            {
                // Maintenant on peut utiliser 'user' sans danger
                Console.WriteLine($"Connexion réussie : {user.FullName}");

                Preferences.Set("UserId", user.Id.ToString());
                Preferences.Set("UserRole", user.Role);
                Preferences.Set("UserName", user.FullName);

                // Redirection vers l'accueil
                await Shell.Current.GoToAsync("//HomePage");
            }
            else
            {
                // Si user est null, c'est que les identifiants sont faux ou le serveur injoignable
                await Shell.Current.DisplayAlertAsync("Échec", "Numéro ou mot de passe incorrect.", "OK");
            }
        }
        catch (Exception ex)
        {
            // 4. Filet de sécurité : Si ça plante, on affiche l'erreur au lieu de fermer l'appli
            Console.WriteLine($"CRASH LOGIN : {ex.Message}");
            await Shell.Current.DisplayAlertAsync("Erreur Technique", "Une erreur est survenue : " + ex.Message, "OK");
        }
    }
}