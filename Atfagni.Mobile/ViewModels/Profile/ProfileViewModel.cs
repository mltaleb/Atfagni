using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Atfagni.Mobile.Views.Auth; // Pour rediriger vers le Login

namespace Atfagni.Mobile.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    [ObservableProperty]
    private string userName;

    [ObservableProperty]
    private string userRole;

    [ObservableProperty]
    private string userPhone;

    public ProfileViewModel()
    {
        // On récupère les infos stockées
        UserName = Preferences.Get("UserName", "Utilisateur");
        UserRole = Preferences.Get("UserRole", "Inconnu");
        UserPhone = Preferences.Get("UserPhone", "");
    }

    [RelayCommand]
    private async Task Logout()
    {
        bool confirm = await Shell.Current.DisplayAlertAsync("Déconnexion", "Voulez-vous vraiment vous déconnecter ?", "Oui", "Non");
        if (!confirm) return;

        // 1. On efface les données locales
        Preferences.Clear();

        // 2. On redirige vers la page de Login (ou MainPage)
        // Le "///" réinitialise la pile de navigation
        await Shell.Current.GoToAsync("//MainPage");
    }
}