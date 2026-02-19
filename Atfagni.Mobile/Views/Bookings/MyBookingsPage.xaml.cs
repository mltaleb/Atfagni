using Atfagni.Mobile.ViewModels.Bookings;
namespace Atfagni.Mobile.Views.Bookings;

public partial class MyBookingsPage : ContentPage
{
    private readonly MyBookingsViewModel _viewModel;

    public MyBookingsPage(MyBookingsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    // Cette méthode est appelée à chaque fois que la page apparaît
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // On lance la commande de chargement
        // On vérifie si elle n'est pas déjà en train de tourner (IsBusy)
        if (_viewModel.LoadBookingsCommand.CanExecute(null))
        {
            await _viewModel.LoadBookingsCommand.ExecuteAsync(null);
        }
    }
}