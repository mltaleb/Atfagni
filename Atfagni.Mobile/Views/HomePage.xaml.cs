using Atfagni.Mobile.ViewModels;

namespace Atfagni.Mobile.Views;

public partial class HomePage : ContentPage
{
    readonly HomeViewModel _viewModel;
	public HomePage(HomeViewModel ViewModel)
	{
        _viewModel = ViewModel;
		InitializeComponent();
		BindingContext = ViewModel;
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
        }
    }
}