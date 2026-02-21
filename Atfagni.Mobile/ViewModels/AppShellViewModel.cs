using CommunityToolkit.Mvvm.ComponentModel;

namespace Atfagni.Mobile.ViewModels;

public partial class AppShellViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isDriver; // Sera True si Chauffeur, False si Passager
    [ObservableProperty]
    private bool isPassager; 

    public AppShellViewModel()
    {
        // On vérifie le rôle au démarrage
        CheckRole();
    }

    public void CheckRole()
    {
        string role = Preferences.Get("UserRole", "Passenger");
        string role2 = Preferences.Get("UserRole", "Driver");
        IsDriver = (role2 == "Driver");
        IsPassager = (role == "Passenger");
    }
}