using System.Collections.Generic;
using static Repository.DataModels.TourAndTravels.DepartureManagementDTO;

namespace Repository.Interfaces.TourAndTravels
{
    public interface IDepartureManagementRepository
    {
        // Fetch bookings not yet assigned to a departure, grouped data returned flat
        List<UnassignedBookingItem> GetUnassignedBookings();

        // Suggest existing departures for an itinerary that have enough capacity
        List<SuggestedDepartureItem> GetSuggestedDepartures(long itineraryId, int requiredSeats);

        // Create a brand-new departure and assign the given bookings to it
        DepartureDetailResponse CreateDeparture(CreateDepartureRequest request);

        // Assign a list of booking-instance ids to an existing departure
        DepartureDetailResponse AssignBookingsToDeparture(AssignBookingsToDepartureRequest request);

        // Get departure detail (with assigned bookings)
        DepartureDetailResponse? GetDepartureById(long departureId);

        // Get traveler notification info for all travelers in a departure (for notifications)
        List<TravelerNotificationInfo> GetTravelerNotifications(long departureId);

        // Get all departures with guide, vehicle, traveler aggregates
        List<DepartureListItem> GetAllDepartures();
    }
}
