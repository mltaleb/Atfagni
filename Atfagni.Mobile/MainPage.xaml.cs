using Atfagni.Mobile.ViewModels;

namespace Atfagni.Mobile
{
    public partial class MainPage : ContentPage
    {
        

        public MainPage(MainViewModel ViewModel)
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }

        
    }
}
