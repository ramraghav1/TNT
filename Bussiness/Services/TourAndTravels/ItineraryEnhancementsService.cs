using System;
using System.Collections.Generic;
using System.Linq;
using Repository.Repositories.TourAndTravels;
using Repository.Interfaces.TourAndTravels;
using static Domain.Models.TourAndTravels.ItineraryEnhancements;
using static Domain.Models.TourAndTravels.Booking;

namespace Bussiness.Services.TourAndTravels
{
    public interface IItineraryEnhancementsService
    {
        // Media Management
        long AddMedia(long itineraryId, CreateMediaRequest request);
        List<MediaResponse> GetMedia(long itineraryId);
        bool DeleteMedia(long mediaId);
        bool ReorderMedia(long itineraryId, List<long> mediaIds);

        // Assignment Management
        BookingWithAssignmentsResponse CreateBookingWithAssignments(CreateBookingWithAssignmentsRequest request);
        long CreateAssignment(CreateAssignmentRequest request);
        List<AssignmentResponse> GetAssignmentsForInstance(long instanceId);
        bool UpdateAssignment(long assignmentId, CreateAssignmentRequest request);
        bool DeleteAssignment(long assignmentId);

        // Availability
        AvailabilityResponse CheckAvailability(CheckAvailabilityRequest request);
        List<AssignmentConflict> ValidateAssignments(long instanceId, List<DayAssignmentInput> assignments);

        // Booking Requests
        long CreateBookingRequest(CreateBookingRequestRequest request);
        List<BookingRequestResponse> GetBookingRequests(string? status = null);
        BookingRequestResponse? GetBookingRequest(long id);
        bool RespondToBookingRequest(long id, string status, string respondedBy);
        long ConvertBookingRequestToInstance(long requestId, CreateBookingWithAssignmentsRequest bookingRequest);

        // Public API
        PublicItineraryResponse? GetPublicItinerary(long itineraryId);
        List<PublicItineraryResponse> GetPublishedItineraries();
    }

    public class ItineraryEnhancementsService : IItineraryEnhancementsService
    {
        private readonly IItineraryEnhancementsRepository _enhancementsRepo;
        private readonly IItineraryRepository _itineraryRepo;
        private readonly IBookingService _bookingService;

        public ItineraryEnhancementsService(
            IItineraryEnhancementsRepository enhancementsRepo,
            IItineraryRepository itineraryRepo,
            IBookingService bookingService)
        {
            _enhancementsRepo = enhancementsRepo;
            _itineraryRepo = itineraryRepo;
            _bookingService = bookingService;
        }

        // ============================================================
        // MEDIA MANAGEMENT
        // ============================================================

        public long AddMedia(long itineraryId, CreateMediaRequest request)
        {
            // Validate media type
            if (!IsValidMediaType(request.MediaType))
            {
                throw new ArgumentException("Invalid media type. Must be 'Image', 'Video', or 'VideoLink'.");
            }

            return _enhancementsRepo.AddMedia(itineraryId, request);
        }

        public List<MediaResponse> GetMedia(long itineraryId)
        {
            return _enhancementsRepo.GetMedia(itineraryId);
        }

        public bool DeleteMedia(long mediaId)
        {
            return _enhancementsRepo.DeleteMedia(mediaId);
        }

        public bool ReorderMedia(long itineraryId, List<long> mediaIds)
        {
            var currentMedia = _enhancementsRepo.GetMedia(itineraryId);
            var currentIds = currentMedia.Select(m => m.Id).ToList();

            // Validate all media IDs belong to this itinerary
            if (mediaIds.Any(id => !currentIds.Contains(id)))
            {
                throw new ArgumentException("Some media IDs do not belong to this itinerary.");
            }

            for (int i = 0; i < mediaIds.Count; i++)
            {
                _enhancementsRepo.UpdateMediaOrder(mediaIds[i], i);
            }

            return true;
        }

        // ============================================================
        // ASSIGNMENT MANAGEMENT
        // ============================================================

        public BookingWithAssignmentsResponse CreateBookingWithAssignments(CreateBookingWithAssignmentsRequest request)
        {
            var response = new BookingWithAssignmentsResponse
            {
                BookingReference = GenerateBookingReference(),
                Status = "Pending",
                Conflicts = new List<AssignmentConflict>()
            };

            // Validate date range
            if (request.StartDate >= request.EndDate)
            {
                throw new ArgumentException("Start date must be before end date.");
            }

            // Check availability for all assignments
            var conflicts = ValidateAssignments(0, request.DayAssignments, request.StartDate);
            if (conflicts.Any())
            {
                response.Conflicts = conflicts;
                response.Status = "ConflictsDetected";
                return response;
            }

            // Create instance using existing BookingService
            var bookingRequest = new CreateBookingRequest
            {
                ItineraryId = request.TemplateItineraryId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                SpecialRequests = request.SpecialRequests
            };

            // Add traveler if provided
            if (request.Traveler != null)
            {
                bookingRequest.Travelers.Add(new TravelerRequest
                {
                    FullName = request.Traveler.FullName,
                    ContactNumber = request.Traveler.ContactNumber,
                    Email = request.Traveler.Email,
                    Nationality = request.Traveler.Nationality,
                    Adults = request.Traveler.Adults,
                    Children = request.Traveler.Children,
                    Seniors = request.Traveler.Seniors
                });
            }

            var bookingResult = _bookingService.CreateBooking(bookingRequest);
            var instanceId = bookingResult.InstanceId;
            response.InstanceId = instanceId;
            response.BookingReference = bookingResult.BookingReference ?? GenerateBookingReference();

            // Create assignments for each day
            var instanceDays = GetInstanceDays(instanceId);
            foreach (var assignment in request.DayAssignments)
            {
                var instanceDay = instanceDays.FirstOrDefault(d => d.DayNumber == assignment.DayNumber);
                if (instanceDay.Id > 0)
                {
                    var createRequest = new CreateAssignmentRequest
                    {
                        InstanceDayId = instanceDay.Id,
                        HotelId = assignment.HotelId,
                        RoomType = assignment.RoomType,
                        NumberOfRooms = assignment.NumberOfRooms,
                        GuideId = assignment.GuideId,
                        VehicleId = assignment.VehicleId,
                        Notes = assignment.Notes
                    };

                    _enhancementsRepo.CreateAssignment(createRequest);
                }
            }

            response.Status = "Confirmed";
            return response;
        }

        public long CreateAssignment(CreateAssignmentRequest request)
        {
            return _enhancementsRepo.CreateAssignment(request);
        }

        public List<AssignmentResponse> GetAssignmentsForInstance(long instanceId)
        {
            return _enhancementsRepo.GetAssignmentsForInstance(instanceId);
        }

        public bool UpdateAssignment(long assignmentId, CreateAssignmentRequest request)
        {
            return _enhancementsRepo.UpdateAssignment(assignmentId, request);
        }

        public bool DeleteAssignment(long assignmentId)
        {
            return _enhancementsRepo.DeleteAssignment(assignmentId);
        }

        // ============================================================
        // AVAILABILITY
        // ============================================================

        public AvailabilityResponse CheckAvailability(CheckAvailabilityRequest request)
        {
            return _enhancementsRepo.CheckAvailability(request);
        }

        public List<AssignmentConflict> ValidateAssignments(long instanceId, List<DayAssignmentInput> assignments)
        {
            // Get instance start date if instanceId > 0
            DateTime startDate = DateTime.Today;
            if (instanceId > 0)
            {
                // TODO: Fetch instance start date
                // var instance = _itineraryRepo.GetInstanceById(instanceId);
                // startDate = instance?.StartDate ?? DateTime.Today;
            }

            return ValidateAssignments(instanceId, assignments, startDate);
        }

        private List<AssignmentConflict> ValidateAssignments(
            long instanceId, 
            List<DayAssignmentInput> assignments, 
            DateTime startDate)
        {
            var conflicts = new List<AssignmentConflict>();

            // Group assignments by day
            foreach (var assignment in assignments)
            {
                var assignmentDate = startDate.AddDays(assignment.DayNumber - 1);

                // Check hotel availability
                if (assignment.HotelId.HasValue)
                {
                    var hotelConflicts = _enhancementsRepo.GetConflictingHotels(
                        assignmentDate, 
                        assignmentDate, 
                        new List<long> { assignment.HotelId.Value });

                    if (hotelConflicts.Any())
                    {
                        conflicts.Add(new AssignmentConflict
                        {
                            DayNumber = assignment.DayNumber,
                            Date = assignmentDate,
                            ResourceType = "Hotel",
                            ResourceId = assignment.HotelId.Value,
                            ResourceName = GetHotelName(assignment.HotelId.Value),
                            ConflictReason = "Hotel is already booked for this date"
                        });
                    }
                }

                // Check guide availability
                if (assignment.GuideId.HasValue)
                {
                    var guideConflicts = _enhancementsRepo.GetConflictingGuides(
                        assignmentDate, 
                        assignmentDate, 
                        new List<long> { assignment.GuideId.Value });

                    if (guideConflicts.Any())
                    {
                        conflicts.Add(new AssignmentConflict
                        {
                            DayNumber = assignment.DayNumber,
                            Date = assignmentDate,
                            ResourceType = "Guide",
                            ResourceId = assignment.GuideId.Value,
                            ResourceName = GetGuideName(assignment.GuideId.Value),
                            ConflictReason = "Guide is already assigned to another booking"
                        });
                    }
                }

                // Check vehicle availability
                if (assignment.VehicleId.HasValue)
                {
                    var vehicleConflicts = _enhancementsRepo.GetConflictingVehicles(
                        assignmentDate, 
                        assignmentDate, 
                        new List<long> { assignment.VehicleId.Value });

                    if (vehicleConflicts.Any())
                    {
                        conflicts.Add(new AssignmentConflict
                        {
                            DayNumber = assignment.DayNumber,
                            Date = assignmentDate,
                            ResourceType = "Vehicle",
                            ResourceId = assignment.VehicleId.Value,
                            ResourceName = GetVehicleName(assignment.VehicleId.Value),
                            ConflictReason = "Vehicle is already assigned to another booking"
                        });
                    }
                }
            }

            return conflicts;
        }

        // ============================================================
        // BOOKING REQUESTS
        // ============================================================

        public long CreateBookingRequest(CreateBookingRequestRequest request)
        {
            // Validate email format
            if (!IsValidEmail(request.CustomerEmail))
            {
                throw new ArgumentException("Invalid email address.");
            }

            // Validate traveler count
            if (request.NumberOfTravelers < 1)
            {
                throw new ArgumentException("Number of travelers must be at least 1.");
            }

            return _enhancementsRepo.CreateBookingRequest(request);
        }

        public List<BookingRequestResponse> GetBookingRequests(string? status = null)
        {
            return _enhancementsRepo.GetBookingRequests(status);
        }

        public BookingRequestResponse? GetBookingRequest(long id)
        {
            return _enhancementsRepo.GetBookingRequest(id);
        }

        public bool RespondToBookingRequest(long id, string status, string respondedBy)
        {
            var validStatuses = new[] { "Pending", "Contacted", "Converted", "Rejected" };
            if (!validStatuses.Contains(status))
            {
                throw new ArgumentException($"Invalid status. Must be one of: {string.Join(", ", validStatuses)}");
            }

            return _enhancementsRepo.UpdateBookingRequestStatus(id, status, respondedBy);
        }

        public long ConvertBookingRequestToInstance(long requestId, CreateBookingWithAssignmentsRequest bookingRequest)
        {
            var request = _enhancementsRepo.GetBookingRequest(requestId);
            if (request == null)
            {
                throw new ArgumentException("Booking request not found.");
            }

            if (request.Status == "Converted")
            {
                throw new InvalidOperationException("This booking request has already been converted.");
            }

            // Create the booking
            var response = CreateBookingWithAssignments(bookingRequest);

            if (response.Status == "Confirmed")
            {
                // Update the booking request status
                _enhancementsRepo.ConvertBookingRequest(requestId, response.InstanceId);
                return response.InstanceId;
            }

            throw new InvalidOperationException("Failed to create booking due to conflicts.");
        }

        // ============================================================
        // PUBLIC API
        // ============================================================

        public PublicItineraryResponse? GetPublicItinerary(long itineraryId)
        {
            return _enhancementsRepo.GetPublicItinerary(itineraryId);
        }

        public List<PublicItineraryResponse> GetPublishedItineraries()
        {
            return _enhancementsRepo.GetPublishedItineraries();
        }

        // ============================================================
        // HELPER METHODS
        // ============================================================

        private bool IsValidMediaType(string mediaType)
        {
            var validTypes = new[] { "Image", "Video", "VideoLink" };
            return validTypes.Contains(mediaType);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateBookingReference()
        {
            return $"BK{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(100, 999)}";
        }

        private List<(long Id, int DayNumber, DateTime? Date)> GetInstanceDays(long instanceId)
        {
            return _enhancementsRepo.GetInstanceDays(instanceId);
        }

        private string GetHotelName(long hotelId)
        {
            return _enhancementsRepo.GetHotelName(hotelId);
        }

        private string GetGuideName(long guideId)
        {
            return _enhancementsRepo.GetGuideName(guideId);
        }

        private string GetVehicleName(long vehicleId)
        {
            return _enhancementsRepo.GetVehicleName(vehicleId);
        }
    }
}
