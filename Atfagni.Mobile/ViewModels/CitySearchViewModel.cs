using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Atfagni.Mobile.Services;
using Atfagni.Mobile.Models.Local;
using Atfagni.Mobile.Messages;
using System.Collections.ObjectModel;

namespace Atfagni.Mobile.ViewModels;

[QueryProperty(nameof(TargetType), "target")] // Reçoit si c'est pour Départ ou Arrivée
public partial class CitySearchViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;

    [ObservableProperty] string targetType;
    [ObservableProperty] string searchText;
    public ObservableCollection<LocalCity> Cities { get; } = new();

    public CitySearchViewModel(DatabaseService dbService)
    {
        _dbService = dbService;
        _ = LoadCities(); // Chargement initial
    }

    // Filtrage automatique quand on tape
    partial void OnSearchTextChanged(string value) => _ = LoadCities(value);

    private async Task LoadCities(string filter = "")
    {
        var list = await _dbService.GetCitiesAsync(filter);
        Cities.Clear();
        foreach (var c in list) Cities.Add(c);
    }

    [RelayCommand]
    private async Task SelectCity(LocalCity city)
    {
        if (city == null) return;

        // ON ENVOIE LE MESSAGE (MVVM pur)
        WeakReferenceMessenger.Default.Send(new CitySelectedMessage(city.Name, TargetType));

        // On ferme la page de recherche
        await Shell.Current.Navigation.PopModalAsync();
    }
}