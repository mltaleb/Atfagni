using Microsoft.Extensions.DependencyInjection;

namespace Atfagni.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Dispatcher.Dispatch(async () =>
            {
                await CheckUserLogin();
            });
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
        private async Task CheckUserLogin()
        {
            string userId = Preferences.Get("UserId", string.Empty);

            if (!string.IsNullOrEmpty(userId))
            {
                // UTILISATEUR CONNECTÉ
                // On utilise "//" pour dire "C'est la nouvelle racine", on efface l'historique
                try
                {
                    await Shell.Current.GoToAsync("//HomePage");
                }
                catch (Exception ex)
                {
                    // Juste au cas où la route n'est pas encore prête
                    Console.WriteLine($"Erreur nav: {ex.Message}");
                }
            }
            else
            {
                // UTILISATEUR NON CONNECTÉ
                // ⚠️ Pas besoin de faire GoToAsync("MainPage") ici !
                // Par défaut, le Shell affiche déjà la première page déclarée dans AppShell.xaml.
                // Donc on ne fait RIEN, on laisse l'utilisateur sur l'écran de bienvenue.
            }
        }
    }
}