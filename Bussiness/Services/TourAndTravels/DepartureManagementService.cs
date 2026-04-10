using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Domain.Models.TourAndTravels;
using Repository.Interfaces.TourAndTravels;
using static Domain.Models.TourAndTravels.DepartureManagement;
using RepDTO = Repository.DataModels.TourAndTravels.DepartureManagementDTO;

namespace Bussiness.Services.TourAndTravels
{
    // ================================================================
    // Service Interface
    // ================================================================
    public interface IDepartureManagementService
    {
        List<BookingGroupByItinerary> GetUnassignedBookingsGrouped();
        List<SuggestedDepartureItem> GetSuggestedDepartures(long itineraryId, int requiredSeats);
        DepartureDetailResponse CreateDeparture(CreateDepartureRequest request);
        DepartureDetailResponse AssignBookingsToDeparture(AssignBookingsToDepartureRequest request);
        DepartureDetailResponse? GetDepartureById(long departureId);
        List<DepartureListItem> GetAllDepartures();
    }

    // ================================================================
    // Service Implementation
    // ================================================================
    public class DepartureManagementService : IDepartureManagementService
    {
        private readonly IDepartureManagementRepository _repository;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public DepartureManagementService(
            IDepartureManagementRepository repository,
            IMapper mapper,
            INotificationService notificationService)
        {
            _repository = repository;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        // ─── Get all unassigned bookings grouped by itinerary ────────────
        public List<BookingGroupByItinerary> GetUnassignedBookingsGrouped()
        {
            var rows = _repository.GetUnassignedBookings();
            var domainRows = _mapper.Map<List<UnassignedBookingItem>>(rows);

            return domainRows
                .GroupBy(b => new { b.ItineraryId, b.ItineraryTitle })
                .Select(g => new BookingGroupByItinerary
                {
                    ItineraryId    = g.Key.ItineraryId,
                    ItineraryTitle = g.Key.ItineraryTitle,
                    TotalBookings  = g.Count(),
                    TotalPersons   = g.Sum(b => b.TotalPerson),
                    Bookings       = g.ToList()
                })
                .OrderBy(g => g.ItineraryTitle)
                .ToList();
        }

        // ─── Suggest departures with enough capacity ─────────────────────
        public List<SuggestedDepartureItem> GetSuggestedDepartures(long itineraryId, int requiredSeats)
        {
            var rows = _repository.GetSuggestedDepartures(itineraryId, requiredSeats);
            return _mapper.Map<List<SuggestedDepartureItem>>(rows);
        }

        // ─── Create new departure + assign bookings ───────────────────────
        public DepartureDetailResponse CreateDeparture(CreateDepartureRequest request)
        {
            var repoRequest = _mapper.Map<RepDTO.CreateDepartureRequest>(request);
            var repoResponse = _repository.CreateDeparture(repoRequest);
            var result = _mapper.Map<DepartureDetailResponse>(repoResponse);

            // Admin real-time notification
            _ = _notificationService.CreateAndBroadcastAsync(new Domain.Models.CreateNotification
            {
                Type    = "departure",
                Title   = "Departure Created",
                Message = $"Departure for '{result.ItineraryTitle}' on {result.DepartureDate:dd MMM yyyy} created with {result.BookedCount} traveler(s).",
                Link    = "/manage-bookings",
                Icon    = "pi-send"
            });

            // Notify each traveler
            NotifyTravelers(result.Id);

            return result;
        }

        // ─── Assign bookings to existing departure ────────────────────────
        public DepartureDetailResponse AssignBookingsToDeparture(AssignBookingsToDepartureRequest request)
        {
            var repoRequest = _mapper.Map<RepDTO.AssignBookingsToDepartureRequest>(request);
            var repoResponse = _repository.AssignBookingsToDeparture(repoRequest);
            var result = _mapper.Map<DepartureDetailResponse>(repoResponse);

            _ = _notificationService.CreateAndBroadcastAsync(new Domain.Models.CreateNotification
            {
                Type    = "departure",
                Title   = "Bookings Assigned to Departure",
                Message = $"{request.BookingIds.Count} booking(s) assigned to departure for '{result.ItineraryTitle}'.",
                Link    = "/manage-bookings",
                Icon    = "pi-users"
            });

            NotifyTravelers(result.Id);
            return result;
        }

        // ─── Get departure by id ──────────────────────────────────────────
        public DepartureDetailResponse? GetDepartureById(long departureId)
        {
            var repoResponse = _repository.GetDepartureById(departureId);
            return repoResponse == null ? null : _mapper.Map<DepartureDetailResponse>(repoResponse);
        }

        // ─── Get all departures with computed status ──────────────────────
        public List<DepartureListItem> GetAllDepartures()
        {
            var rows = _repository.GetAllDepartures();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return rows.Select(r =>
            {
                var item = _mapper.Map<DepartureListItem>(r);
                item.HasInventory = r.GuideId.HasValue || r.VehicleId.HasValue;

                item.ComputedStatus = r.Status == "Cancelled" ? "Cancelled"
                    : r.DepartureDate > today ? "Upcoming"
                    : r.DepartureDate == today ? "Ongoing"
                    : (r.EndDate.HasValue && r.EndDate.Value >= today) ? "Ongoing"
                    : "Completed";

                return item;
            }).ToList();
        }

        // ─── Internal: fire-and-forget traveler notifications ─────────────
        private void NotifyTravelers(long departureId)
        {
            try
            {
                var travelers = _repository.GetTravelerNotifications(departureId);
                foreach (var t in travelers)
                {
                    var endLabel = t.EndDate.HasValue ? t.EndDate.Value.ToString("dd MMM yyyy") : "TBD";
                    var msg = $"Your '{t.ItineraryTitle}' trip from {t.StartDate.ToString("dd MMM yyyy")} to {endLabel} is confirmed." +
                              (t.GuideName != null ? $" Guide: {t.GuideName}." : "") +
                              (t.VehicleInfo != null ? $" Vehicle: {t.VehicleInfo}." : "") +
                              $" Total group size: {t.TotalGroupSize} traveler(s).";

                    _ = _notificationService.CreateAndBroadcastAsync(new Domain.Models.CreateNotification
                    {
                        Type    = "trip-confirmed",
                        Title   = "Your Trip is Confirmed! ✈️",
                        Message = msg,
                        Link    = $"/booking-detail/{t.BookingInstanceId}",
                        Icon    = "pi-check-circle"
                    });
                }
            }
            catch
            {
                // Non-blocking: notification failures must not break the main flow
            }
        }
    }
}
