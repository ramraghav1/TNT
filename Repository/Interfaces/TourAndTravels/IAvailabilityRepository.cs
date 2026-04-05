using System;
using System.Collections.Generic;
using Domain.Models.TourAndTravels;

namespace Repository.Interfaces.TourAndTravels
{
    public interface IAvailabilityRepository
    {
        // Availability Checking
        List<Availability.AvailabilityResponse> CheckAvailability(Availability.CheckAvailabilityRequest request);
        
        // Availability Blocking
        Availability.AvailabilityBlockResponse CreateBlock(Availability.BlockAvailabilityRequest request, long userId);
        List<Availability.AvailabilityBlockResponse> GetBlocks(DateTime? startDate = null, DateTime? endDate = null);
        bool DeleteBlock(long blockId);
        
        // Calendar View
        Availability.CalendarViewResponse GetCalendarView(Availability.CalendarViewRequest request);
        
        // Package Departures
        Availability.PackageDepartureResponse CreatePackageDeparture(Availability.CreatePackageDepartureRequest request);
        List<Availability.PackageDepartureResponse> GetPackageDepartures(long? itineraryId = null);
        Availability.PackageDepartureResponse? GetPackageDepartureById(long id);
        Availability.PackageDepartureResponse? UpdatePackageDeparture(long id, Availability.UpdatePackageDepartureRequest request);
        bool DeletePackageDeparture(long id);
        
        // Booking Inventory Links
        Availability.BookingInventoryResponse AssignInventoryToBooking(Availability.AssignInventoryToBookingRequest request);
        List<Availability.BookingInventoryResponse> GetBookingInventory(long bookingInstanceId);
        bool RemoveInventoryFromBooking(long id);
        
        // Capacity Management
        bool UpdateCapacity(string inventoryType, long inventoryId, DateTime date, int change);
        bool InitializeAvailability(string inventoryType, long inventoryId, DateTime startDate, DateTime endDate, int capacity);
    }
}
