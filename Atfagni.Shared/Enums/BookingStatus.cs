namespace Atfagni.Shared.Enums
{
    public enum BookingStatus
    {
        Pending,   // En attente (Le passager a demandé)
        Accepted,  // Accepté (Le chauffeur a dit Oui)
        Rejected,  // Refusé (Le chauffeur a dit Non, ou plus de place)
        Completed, // Terminé (Le trajet est fini)
        Cancelled  // Annulé par le passager
    }
}