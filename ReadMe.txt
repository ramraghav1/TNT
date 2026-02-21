
1. Admin Creates Itinerary Template
   └─> Fill in Itinerary details (Title, Description, Duration, Difficulty)
   └─> Add Days (DayNumber, Location, Activities, Meals, Accommodation, Transport)
   └─> Save template → Itinerary + ItineraryDays tables

2. Traveler Browses Itineraries
   └─> Frontend fetches all templates → ItineraryResponse

3. Traveler Selects Itinerary to Book
   └─> Show itinerary details → ItineraryDetailResponse
   └─> Traveler selects start/end date, group size, traveler info

4. Create Booking
   └─> API → BookingService.CreateBooking
   └─> Booking table entry created
   └─> BookingTraveler table entries created
   └─> BookingDay table created as snapshot of ItineraryDays
   └─> Fees applied per day/activity from Pricing tables
   └─> BookingResponse returned with InstanceId

5. Traveler Optional Customization
   └─> API → BookingService.UpdateBooking / CustomizeBooking
   └─> Update BookingDay for this booking (Meals, Accommodation, Transport, Activities)
   └─> Recalculate fees based on changes
   └─> Update Booking.TotalAmount & BalanceAmount

6. Payment Process
   └─> API → BookingService.AddPayment
   └─> BookingPayment table updated
   └─> Booking.AmountPaid & BalanceAmount recalculated
   └─> Partial or full payment supported

7. Approvals
   └─> Traveler approves → Booking.Status = TravelerApproved
   └─> Admin approves → Booking.Status = AdminApproved
   └─> Final confirmation → Booking.Status = Completed

8. Admin / Traveler Views Booking
   └─> API → BookingService.GetBookingById
   └─> Shows BookingDetailResponse (Days, Travelers, Payments, Status)

9. Optional Updates After Approval
   └─> Traveler/Admin can request changes
   └─> BookingDay updates → fees recalculated → new balance
   └─> Status updated if necessary (e.g., Draft again or Reapproved)