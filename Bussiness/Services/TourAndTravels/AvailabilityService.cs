using System;
using System.Collections.Generic;
using Domain.Models.TourAndTravels;
using Repository.Interfaces.TourAndTravels;

namespace Bussiness.Services.TourAndTravels
{
    public interface IAvailabilityService
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

    public class AvailabilityService : IAvailabilityService
    {
        private readonly IAvailabilityRepository _repository;
        private readonly INotificationService _notificationService;

        public AvailabilityService(IAvailabilityRepository repository, INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        // ==================================================
        // AVAILABILITY CHECKING
        // ==================================================
        public List<Availability.AvailabilityResponse> CheckAvailability(Availability.CheckAvailabilityRequest request)
        {
            return _repository.CheckAvailability(request);
        }

        // ==================================================
        // AVAILABILITY BLOCKING
        // ==================================================
        public Availability.AvailabilityBlockResponse CreateBlock(Availability.BlockAvailabilityRequest request, long userId)
        {
            var result = _repository.CreateBlock(request, userId);

            // Push real-time notification
            _ = _notificationService.CreateAndBroadcastAsync(new Domain.Models.CreateNotification
            {
                Type = "availability",
                Title = "Availability Blocked",
                Message = $"{request.InventoryType} blocked from {request.StartDate:MM/dd} to {request.EndDate:MM/dd}: {request.Reason}",
                Link = "/calendar",
                Icon = "pi-ban"
            });

            return result;
        }

        public List<Availability.AvailabilityBlockResponse> GetBlocks(DateTime? startDate = null, DateTime? endDate = null)
        {
            return _repository.GetBlocks(startDate, endDate);
        }

        public bool DeleteBlock(long blockId)
        {
            var result = _repository.DeleteBlock(blockId);

            if (result)
            {
                // Push real-time notification
                _ = _notificationService.CreateAndBroadcastAsync(new Domain.Models.CreateNotification
                {
                    Type = "availability",
                    Title = "Block Removed",
                    Message = "Availability block has been removed",
                    Link = "/calendar",
                    Icon = "pi-check"
                });
            }

            return result;
        }

        // ==================================================
        // CALENDAR VIEW
        // ==================================================
        public Availability.CalendarViewResponse GetCalendarView(Availability.CalendarViewRequest request)
        {
            return _repository.GetCalendarView(request);
        }

        // ==================================================
        // PACKAGE DEPARTURES
        // ==================================================
        public Availability.PackageDepartureResponse CreatePackageDeparture(Availability.CreatePackageDepartureRequest request)
        {
            var result = _repository.CreatePackageDeparture(request);

            // Push real-time notification
            _ = _notificationService.CreateAndBroadcastAsync(new Domain.Models.CreateNotification
            {
                Type = "departure",
                Title = "New Package Departure",
                Message = $"New departure scheduled for {request.DepartureDate:MM/dd/yyyy}",
                Link = "/departures",
                Icon = "pi-calendar-plus"
            });

            return result;
        }

        public List<Availability.PackageDepartureResponse> GetPackageDepartures(long? itineraryId = null)
        {
            return _repository.GetPackageDepartures(itineraryId);
        }

        public Availability.PackageDepartureResponse? GetPackageDepartureById(long id)
        {
            return _repository.GetPackageDepartureById(id);
        }

        public Availability.PackageDepartureResponse? UpdatePackageDeparture(long id, Availability.UpdatePackageDepartureRequest request)
        {
            return _repository.UpdatePackageDeparture(id, request);
        }

        public bool DeletePackageDeparture(long id)
        {
            return _repository.DeletePackageDeparture(id);
        }

        // ==================================================
        // BOOKING INVENTORY LINKS
        // ==================================================
        public Availability.BookingInventoryResponse AssignInventoryToBooking(Availability.AssignInventoryToBookingRequest request)
        {
            var result = _repository.AssignInventoryToBooking(request);

            // Push real-time notification
            _ = _notificationService.CreateAndBroadcastAsync(new Domain.Models.CreateNotification
            {
                Type = "booking",
                Title = "Inventory Assigned",
                Message = $"{request.InventoryType} assigned to booking for {request.StartDate:MM/dd} to {request.EndDate:MM/dd}",
                Link = "/bookings",
                Icon = "pi-link"
            });

            return result;
        }

        public List<Availability.BookingInventoryResponse> GetBookingInventory(long bookingInstanceId)
        {
            return _repository.GetBookingInventory(bookingInstanceId);
        }

        public bool RemoveInventoryFromBooking(long id)
        {
            return _repository.RemoveInventoryFromBooking(id);
        }

        // ==================================================
        // CAPACITY MANAGEMENT
        // ==================================================
        public bool UpdateCapacity(string inventoryType, long inventoryId, DateTime date, int change)
        {
            return _repository.UpdateCapacity(inventoryType, inventoryId, date, change);
        }

        public bool InitializeAvailability(string inventoryType, long inventoryId, DateTime startDate, DateTime endDate, int capacity)
        {
            return _repository.InitializeAvailability(inventoryType, inventoryId, startDate, endDate, capacity);
        }
    }
}
