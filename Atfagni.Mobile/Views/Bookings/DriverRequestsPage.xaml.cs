using Atfagni.Mobile.ViewModels.Bookings;

namespace Atfagni.Mobile.Views.Bookings;

public partial class DriverRequestsPage : ContentPage
{
    private readonly DriverRequestsViewModel _viewModel;
    public DriverRequestsPage(DriverRequestsViewModel viewModel)
	{
		InitializeComponent();
		
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // On lance la commande de chargement
        // On vérifie si elle n'est pas déjà en train de tourner (IsBusy)
        if (_viewModel.LoadRequestsCommand.CanExecute(null))
        {
            await _viewModel.LoadRequestsCommand.ExecuteAsync(null);
        }
    }
}