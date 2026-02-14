namespace Atfagni.Shared.DTOs
{
    public class UpdateRideRequest : CreateRideRequest
    {
        public int Id { get; set; } // L'ID du trajet à modifier
    }
}