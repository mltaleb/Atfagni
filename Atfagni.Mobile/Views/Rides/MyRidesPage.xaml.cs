using Atfagni.Mobile.ViewModels.Rides;

namespace Atfagni.Mobile.Views.Rides;

public partial class MyRidesPage : ContentPage
{
    private readonly MyRidesViewModel _viewModel;

    public MyRidesPage(MyRidesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    // C'est ICI qu'il faut charger les données
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // On lance la commande proprement quand la page est visible
        if (_viewModel.LoadRidesCommand.CanExecute(null))
        {
            await _viewModel.LoadRidesCommand.ExecuteAsync(null);
        }
    }
}