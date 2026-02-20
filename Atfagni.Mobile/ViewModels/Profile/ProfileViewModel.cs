using Atfagni.Mobile.Services;
using Atfagni.Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Atfagni.Mobile.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty] string fullName;
    [ObservableProperty] string phoneNumber;
    [ObservableProperty] ImageSource profileImageSource = "user_circle.png";
    // Propriété indispensable pour le LoadingOverlay
    [ObservableProperty] private bool isBusy;
    private string base64Photo;

    public ProfileViewModel(ApiService apiService)
    {
        _apiService = apiService;
        LoadUserData();
    }

    private void LoadUserData()
    {
        FullName = Preferences.Get("UserName", "");
        PhoneNumber = Preferences.Get("UserPhone", "");
        // On pourrait aussi charger la photo depuis les préférences si on l'a stockée
    }

    [RelayCommand]
    async Task ChangePhoto()
    {
        try
        {
            // Utilisation de la version moderne (retourne une liste)
            var results = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions
            {
                Title = "Choisir une photo de profil"
            });

            // On prend la première photo de la liste
            var photo = results?.FirstOrDefault();
            if (photo == null) return;

            // Lecture et conversion
            using var stream = await photo.OpenReadAsync();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            byte[] imageBytes = ms.ToArray();

            // Stockage pour l'envoi au serveur
            base64Photo = Convert.ToBase64String(imageBytes);

            // Mise à jour de l'image à l'écran
            ProfileImageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", "Impossible de récupérer la photo", "OK");
        }
    }

    [RelayCommand]
    async Task UpdateProfile()
    {
        var id = int.Parse(Preferences.Get("UserId", "0"));
        var updateDto = new UserDto
        {
            Id = id,
            FullName = FullName,
            PhoneNumber = PhoneNumber,
            ProfilePictureBase64 = base64Photo
        };

        bool success = await _apiService.UpdateProfileAsync(updateDto);

        if (success)
        {
            Preferences.Set("UserName", FullName);
            Preferences.Set("UserPhone", PhoneNumber);
            await Shell.Current.DisplayAlertAsync("Succès", "Profil mis à jour !", "OK");
        }
    }
    [RelayCommand]
    private async Task Logout()
    {
        // 1. Demander confirmation
        bool confirm = await Shell.Current.DisplayAlert(
            "Déconnexion",
            "Êtes-vous sûr de vouloir vous déconnecter ?",
            "Oui",
            "Annuler");

        if (!confirm) return;

        try
        {
            IsBusy = true; // On affiche ton nouveau Spinner

            // Petit délai pour donner un aspect "nettoyage en cours" (optionnel)
            await Task.Delay(500);

            // 2. Effacer TOUTES les préférences de l'utilisateur
            // Cela supprime UserId, UserRole, UserName, etc.
            Preferences.Clear();

            // Si tu veux garder certains réglages (comme la langue), utilise plutôt :
            // Preferences.Remove("UserId");
            // Preferences.Remove("UserRole");

            // 3. Navigation ABSOLUE vers la page de départ
            // L'utilisation de "///" est cruciale : cela efface tout l'historique 
            // de navigation et empêche l'utilisateur de faire "Retour" vers le profil.
            await Shell.Current.GoToAsync("///MainPage");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", "Un problème est survenu lors de la déconnexion.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}