using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Atfagni.Mobile.Messages;

// Un message simple qui transporte une chaîne (le nom de la ville)
public class CitySelectedMessage : ValueChangedMessage<string>
{
    public string Target { get; } // "Departure" ou "Arrival"

    public CitySelectedMessage(string city, string target) : base(city)
    {
        Target = target;
    }
}