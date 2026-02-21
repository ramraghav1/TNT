using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Repository.Interfaces.TourAndTravels;
using System.Data;
using static Repository.DataModels.TourAndTravels.BookingDTO;
using static Repository.DataModels.TourAndTravels.ItineraryDTO;

namespace Repository.Repositories.TourAndTravels
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IDbConnection _dbConnection;

        public BookingRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // ----------------------------
        // Booking CRUD
        // ----------------------------
        public BookingResponse CreateBooking(CreateBookingRequest request)
        {
            string insertBooking = @"
                INSERT INTO Booking (StartDate, EndDate, Status, TotalAmount, AmountPaid, BalanceAmount)
                VALUES (@StartDate, @EndDate, 'Draft', 0, 0, 0);
                SELECT CAST(SCOPE_IDENTITY() as bigint);
            ";

            var bookingId = _dbConnection.Query<long>(insertBooking, request).Single();

            // Insert travelers
            foreach (var traveler in request.Travelers)
            {
                string insertTraveler = @"
                    INSERT INTO BookingTraveler
                    (BookingId, FullName, Email, ContactNumber, Adults, Children, Seniors)
                    VALUES (@BookingId, @FullName, @Email, @ContactNumber, @Adults, @Children, @Seniors);
                ";
                _dbConnection.Execute(insertTraveler, new
                {
                    BookingId = bookingId,
                    traveler.FullName,
                    traveler.Email,
                    traveler.ContactNumber,
                    traveler.Adults,
                    traveler.Children,
                    traveler.Seniors
                });
            }

            // TODO: Insert BookingDay snapshot from Itinerary if needed

            return new BookingResponse
            {
                InstanceId = bookingId,
                Status = "Draft",
                TotalAmount = 0,
                AmountPaid = 0,
                BalanceAmount = 0
            };
        }

        public List<BookingResponse> GetAllBookings()
        {
            string query = @"SELECT Id as InstanceId, Status, TotalAmount, AmountPaid, BalanceAmount FROM Booking";
            return _dbConnection.Query<BookingResponse>(query).ToList();
        }

        public BookingDetailResponse? GetBookingById(long id)
        {
            string bookingQuery = @"SELECT * FROM Booking WHERE Id = @Id";
            var booking = _dbConnection.QuerySingleOrDefault(bookingQuery, new { Id = id });
            if (booking == null) return null;

            var travelers = _dbConnection.Query<TravelerRequest>(
                @"SELECT FullName, ContactNumber, Email, Adults, Children, Seniors 
                  FROM BookingTraveler WHERE BookingId = @BookingId",
                new { BookingId = id }).ToList();

            var days = _dbConnection.Query<ItineraryDayResponse>(
                @"SELECT * FROM BookingDay WHERE BookingId = @BookingId",
                new { BookingId = id }).ToList();

            return new BookingDetailResponse
            {
                //InstanceId = Id,
                //Status = Status,
                //TotalAmount = TotalAmount,
                //AmountPaid = AmountPaid,
                //BalanceAmount = BalanceAmount,
                //Travelers = travelers,
                Days = days
            };
        }

        public BookingResponse? UpdateBooking(long id, UpdateBookingRequest request)
        {
            string updateBooking = @"
                UPDATE Booking SET
                    SpecialRequests = @SpecialRequests
                WHERE Id = @Id;
            ";

            _dbConnection.Execute(updateBooking, new { request.SpecialRequests, Id = id });

            // Optionally update travelers
            if (request.TravelerName != null || request.TravelerEmail != null || request.TravelerPhone != null)
            {
                string updateTraveler = @"
                    UPDATE BookingTraveler SET
                        FullName = COALESCE(@FullName, FullName),
                        Email = COALESCE(@Email, Email),
                        ContactNumber = COALESCE(@ContactNumber, ContactNumber),
                        Adults = COALESCE(@Adults, Adults),
                        Children = COALESCE(@Children, Children),
                        Seniors = COALESCE(@Seniors, Seniors)
                    WHERE BookingId = @BookingId
                ";
                _dbConnection.Execute(updateTraveler, new
                {
                    FullName = request.TravelerName,
                    Email = request.TravelerEmail,
                    ContactNumber = request.TravelerPhone,
                    Adults = request.Adults,
                    Children = request.Children,
                    Seniors = request.Seniors,
                    BookingId = id
                });
            }

            return GetBookingById(id);
        }

        public bool DeleteBooking(long id)
        {
            string deletePayments = @"DELETE FROM BookingPayment WHERE BookingId = @Id";
            string deleteDays = @"DELETE FROM BookingDay WHERE BookingId = @Id";
            string deleteTravelers = @"DELETE FROM BookingTraveler WHERE BookingId = @Id";
            string deleteBooking = @"DELETE FROM Booking WHERE Id = @Id";

            _dbConnection.Execute(deletePayments, new { Id = id });
            _dbConnection.Execute(deleteDays, new { Id = id });
            _dbConnection.Execute(deleteTravelers, new { Id = id });
            var rows = _dbConnection.Execute(deleteBooking, new { Id = id });

            return rows > 0;
        }

        // ----------------------------
        // Booking Day Operations
        // ----------------------------
        public BookingDayResponse? AddDayToBooking(long bookingId, CustomizeDayRequest request)
        {
            string insertDay = @"
                INSERT INTO BookingDay
                (BookingId, Title, Location, Accommodation, Transport, BreakfastIncluded, LunchIncluded, DinnerIncluded)
                VALUES
                (@BookingId, @Title, @Location, @Accommodation, @Transport, @BreakfastIncluded, @LunchIncluded, @DinnerIncluded);
                SELECT CAST(SCOPE_IDENTITY() as bigint);
            ";
            var dayId = _dbConnection.Query<long>(insertDay, new
            {
                BookingId = bookingId,
                request.Title,
                request.Location,
                request.Accommodation,
                request.Transport,
                request.BreakfastIncluded,
                request.LunchIncluded,
                request.DinnerIncluded
            }).Single();

            return _dbConnection.QuerySingle<BookingDayResponse>(
                @"SELECT * FROM BookingDay WHERE Id = @Id", new { Id = dayId });
        }

        public BookingDayResponse? UpdateBookingDay(long bookingId, long dayId, UpdateBookingDayRequest request)
        {
            string updateDay = @"
                UPDATE BookingDay SET
                    Title = COALESCE(@Title, Title),
                    Location = COALESCE(@Location, Location),
                    Accommodation = COALESCE(@Accommodation, Accommodation),
                    Transport = COALESCE(@Transport, Transport),
                    BreakfastIncluded = COALESCE(@BreakfastIncluded, BreakfastIncluded),
                    LunchIncluded = COALESCE(@LunchIncluded, LunchIncluded),
                    DinnerIncluded = COALESCE(@DinnerIncluded, DinnerIncluded)
                WHERE Id = @DayId AND BookingId = @BookingId;
            ";
            _dbConnection.Execute(updateDay, new
            {
               // request.Title,
                //request.Location,
                request.Accommodation,
                request.Transport,
                request.BreakfastIncluded,
                request.LunchIncluded,
                request.DinnerIncluded,
                DayId = dayId,
                BookingId = bookingId
            });

            // Optionally update activities
            if (request.Activities != null)
            {
                string deleteActivities = @"DELETE FROM BookingDayActivity WHERE BookingDayId = @DayId";
                _dbConnection.Execute(deleteActivities, new { DayId = dayId });

                foreach (var activity in request.Activities)
                {
                    string insertActivity = @"INSERT INTO BookingDayActivity (BookingDayId, ActivityName) VALUES (@DayId, @Activity)";
                    _dbConnection.Execute(insertActivity, new { DayId = dayId, Activity = activity });
                }
            }

            return _dbConnection.QuerySingle<BookingDayResponse>(
                @"SELECT * FROM BookingDay WHERE Id = @Id", new { Id = dayId });
        }

        // ----------------------------
        // Payment Operations
        // ----------------------------
        public bool AddPayment(long bookingId, decimal amount, string paymentMethod)
        {
            string insertPayment = @"
                INSERT INTO BookingPayment (BookingId, Amount, PaymentMethod, PaymentDate)
                VALUES (@BookingId, @Amount, @PaymentMethod, GETDATE());
            ";
            var rows = _dbConnection.Execute(insertPayment, new { BookingId = bookingId, Amount = amount, PaymentMethod = paymentMethod });

            // Update booking totals
            string updateBooking = @"
                UPDATE Booking
                SET AmountPaid = ISNULL((SELECT SUM(Amount) FROM BookingPayment WHERE BookingId = @BookingId), 0),
                    BalanceAmount = TotalAmount - ISNULL((SELECT SUM(Amount) FROM BookingPayment WHERE BookingId = @BookingId), 0)
                WHERE Id = @BookingId;
            ";
            _dbConnection.Execute(updateBooking, new { BookingId = bookingId });

            return rows > 0;
        }

        public List<BookingPaymentDTO> GetPaymentsForBooking(long bookingId)
        {
            string query = @"SELECT Id, Amount, PaymentMethod, PaymentDate FROM BookingPayment WHERE BookingId = @BookingId";
            return _dbConnection.Query<BookingPaymentDTO>(query, new { BookingId = bookingId }).ToList();
        }
    }
}