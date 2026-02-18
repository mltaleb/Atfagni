using Atfagni.Mobile.ViewModels.Rides;

namespace Atfagni.Mobile.Views.Rides;

public partial class BookingHistoryPage : ContentPage
{
    private readonly BookingHistoryViewModel _viewModel;

    public BookingHistoryPage(BookingHistoryViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Charge les données à chaque fois qu'on ouvre la page
        await _viewModel.LoadHistoryCommand.ExecuteAsync(null);
    }
}