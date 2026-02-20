namespace Atfagni.Mobile.Controls;

public partial class LoadingOverlay : ContentView
{
    public static readonly BindableProperty IsBusyProperty =
        BindableProperty.Create(nameof(IsBusy), typeof(bool), typeof(LoadingOverlay), false);

    public static readonly BindableProperty LoadingMessageProperty =
        BindableProperty.Create(nameof(LoadingMessage), typeof(string), typeof(LoadingOverlay), "Chargement...");

    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public string LoadingMessage
    {
        get => (string)GetValue(LoadingMessageProperty);
        set => SetValue(LoadingMessageProperty, value);
    }

    public LoadingOverlay()
    {
        InitializeComponent();
    }
}