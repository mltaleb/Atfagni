namespace Atfagni.Mobile.Views;

public partial class HomePage : ContentPage
{
    private readonly ViewModels.HomeViewModel _viewModel;

    public HomePage(ViewModels.HomeViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    // Cette méthode est appelée à chaque fois que la page devient visible
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // On force le rechargement des données (Dashboard ou Recherche)
        // Cela mettra à jour "Prochain départ" instantanément
        if (_viewModel.InitializeCommand.CanExecute(null))
        {
            await _viewModel.InitializeCommand.ExecuteAsync(null);
            await _viewModel.GetLatestRidesCommand.ExecuteAsync(null);
        }
    }
}