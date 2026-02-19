namespace Atfagni.Mobile.Views.Rides;

public partial class RideDetailPage : ContentPage
{
    public RideDetailPage(ViewModels.Rides.RideDetailViewModel viewModel)
    {
        InitializeComponent();
        // SANS CETTE LIGNE, LE BINDING NE MARCHE PAS
        BindingContext = viewModel;
    }
}