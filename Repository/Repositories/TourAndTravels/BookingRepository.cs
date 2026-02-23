using System;
using System.Data;
using System.Linq;
using Dapper;
using System.Collections.Generic;
using static Repository.DataModels.TourAndTravels.BookingDTO;
using Repository.Interfaces.TourAndTravels;

namespace Repository.Repositories.TourAndTravels
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IDbConnection _dbConnection;

        public BookingRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // ============================================================
        // CREATE BOOKING
        // ============================================================
        public BookingResponse CreateBooking(CreateBookingRequest request)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                string bookingRef = $"BK-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

                string sqlInstance = @"
                    INSERT INTO itinerary_instances
                    (template_itinerary_id, source_instance_id, status, start_date, end_date,
                     is_customized, booking_reference, special_requests,
                     payment_status, total_amount, amount_paid, balance_amount,
                     traveler_approved, admin_approved, created_at)
                    VALUES
                    (@TemplateId, @SourceInstanceId, 'Draft', @StartDate, @EndDate,
                     false, @BookingRef, @SpecialRequests,
                     'Unpaid', 0, 0, 0,
                     false, false, NOW())
                    RETURNING id;";

                long instanceId = _dbConnection.QuerySingle<long>(sqlInstance, new
                {
                    TemplateId = request.ItineraryId,
                    SourceInstanceId = request.SourceInstanceId,
                    request.StartDate,
                    request.EndDate,
                    BookingRef = bookingRef,
                    request.SpecialRequests
                }, transaction);

                if (request.SourceInstanceId.HasValue && request.SourceInstanceId.Value > 0)
                {
                    CopyDaysFromInstance(request.SourceInstanceId.Value, instanceId, request.StartDate, transaction);
                }
                else
                {
                    CopyDaysFromTemplate(request.ItineraryId, instanceId, request.StartDate, transaction);
                }

                foreach (var traveler in request.Travelers)
                {
                    string sqlTraveler = @"
                        INSERT INTO travelers
                        (itinerary_instance_id, full_name, contact_number, email, nationality, adults, children, seniors)
                        VALUES
                        (@InstanceId, @FullName, @ContactNumber, @Email, @Nationality, @Adults, @Children, @Seniors);";

                    _dbConnection.Execute(sqlTraveler, new
                    {
                        InstanceId = instanceId,
                        traveler.FullName,
                        traveler.ContactNumber,
                        traveler.Email,
                        traveler.Nationality,
                        traveler.Adults,
                        traveler.Children,
                        traveler.Seniors
                    }, transaction);
                }

                transaction.Commit();

                return new BookingResponse
                {
                    InstanceId = instanceId,
                    TemplateId = request.ItineraryId,
                    SourceInstanceId = request.SourceInstanceId,
                    BookingReference = bookingRef,
                    Status = "Draft",
                    IsCustomized = false,
                    TotalAmount = 0,
                    AmountPaid = 0,
                    BalanceAmount = 0,
                    PaymentStatus = "Unpaid",
                    TravelerApproved = false,
                    AdminApproved = false,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // ============================================================
        // COPY DAYS FROM TEMPLATE
        // ============================================================
        private void CopyDaysFromTemplate(long templateId, long instanceId, DateTime startDate, IDbTransaction transaction)
        {
            string sqlGetDays = @"
                SELECT id, day_number, title, location, accommodation, transport,
                       breakfast_included, lunch_included, dinner_included
                FROM itinerary_days
                WHERE itinerary_id = @TemplateId
                ORDER BY day_number;";

            var templateDays = _dbConnection.Query(sqlGetDays, new { TemplateId = templateId }, transaction).ToList();

            foreach (var day in templateDays)
            {
                DateTime? dayDate = startDate != default ? startDate.AddDays((int)day.day_number - 1) : null;

                string sqlInsertDay = @"
                    INSERT INTO itinerary_instance_days
                    (itinerary_instance_id, day_number, date, title, location, accommodation, transport,
                     breakfast_included, lunch_included, dinner_included)
                    VALUES
                    (@InstanceId, @DayNumber, @Date, @Title, @Location, @Accommodation, @Transport,
                     @BreakfastIncluded, @LunchIncluded, @DinnerIncluded)
                    RETURNING id;";

                long instanceDayId = _dbConnection.QuerySingle<long>(sqlInsertDay, new
                {
                    InstanceId = instanceId,
                    DayNumber = (int)day.day_number,
                    Date = dayDate,
                    Title = (string?)day.title,
                    Location = (string?)day.location,
                    Accommodation = (string?)day.accommodation,
                    Transport = (string?)day.transport,
                    BreakfastIncluded = (bool)day.breakfast_included,
                    LunchIncluded = (bool)day.lunch_included,
                    DinnerIncluded = (bool)day.dinner_included
                }, transaction);

                string sqlGetActivities = @"
                    SELECT activity FROM itinerary_day_activities
                    WHERE itinerary_day_id = @DayId;";

                var activities = _dbConnection.Query<string>(sqlGetActivities, new { DayId = (long)day.id }, transaction).ToList();

                foreach (var activity in activities)
                {
                    string sqlInsertActivity = @"
                        INSERT INTO itinerary_instance_day_activities
                        (instance_day_id, activity)
                        VALUES (@InstanceDayId, @Activity);";

                    _dbConnection.Execute(sqlInsertActivity, new
                    {
                        InstanceDayId = instanceDayId,
                        Activity = activity
                    }, transaction);
                }
            }
        }

        // ============================================================
        // COPY DAYS FROM INSTANCE (for reusability)
        // ============================================================
        private void CopyDaysFromInstance(long sourceInstanceId, long newInstanceId, DateTime startDate, IDbTransaction transaction)
        {
            string sqlGetDays = @"
                SELECT id, day_number, title, location, accommodation, transport,
                       breakfast_included, lunch_included, dinner_included
                FROM itinerary_instance_days
                WHERE itinerary_instance_id = @SourceInstanceId
                ORDER BY day_number;";

            var sourceDays = _dbConnection.Query(sqlGetDays, new { SourceInstanceId = sourceInstanceId }, transaction).ToList();

            foreach (var day in sourceDays)
            {
                DateTime? dayDate = startDate != default ? startDate.AddDays((int)day.day_number - 1) : null;

                string sqlInsertDay = @"
                    INSERT INTO itinerary_instance_days
                    (itinerary_instance_id, day_number, date, title, location, accommodation, transport,
                     breakfast_included, lunch_included, dinner_included)
                    VALUES
                    (@InstanceId, @DayNumber, @Date, @Title, @Location, @Accommodation, @Transport,
                     @BreakfastIncluded, @LunchIncluded, @DinnerIncluded)
                    RETURNING id;";

                long newDayId = _dbConnection.QuerySingle<long>(sqlInsertDay, new
                {
                    InstanceId = newInstanceId,
                    DayNumber = (int)day.day_number,
                    Date = dayDate,
                    Title = (string?)day.title,
                    Location = (string?)day.location,
                    Accommodation = (string?)day.accommodation,
                    Transport = (string?)day.transport,
                    BreakfastIncluded = (bool)day.breakfast_included,
                    LunchIncluded = (bool)day.lunch_included,
                    DinnerIncluded = (bool)day.dinner_included
                }, transaction);

                string sqlGetActivities = @"
                    SELECT activity FROM itinerary_instance_day_activities
                    WHERE instance_day_id = @DayId;";

                var activities = _dbConnection.Query<string>(sqlGetActivities, new { DayId = (long)day.id }, transaction).ToList();

                foreach (var activity in activities)
                {
                    string sqlInsertActivity = @"
                        INSERT INTO itinerary_instance_day_activities
                        (instance_day_id, activity)
                        VALUES (@InstanceDayId, @Activity);";

                    _dbConnection.Execute(sqlInsertActivity, new
                    {
                        InstanceDayId = newDayId,
                        Activity = activity
                    }, transaction);
                }
            }
        }

        // ============================================================
        // GET ALL BOOKINGS (lightweight list)
        // ============================================================
        public List<BookingListItem> GetAllBookings()
        {
            string sql = @"
                SELECT
                    i.id AS instance_id,
                    i.booking_reference,
                    t.title AS template_title,
                    i.status,
                    i.start_date,
                    i.end_date,
                    i.total_amount,
                    i.payment_status,
                    i.created_at
                FROM itinerary_instances i
                JOIN itineraries t ON t.id = i.template_itinerary_id
                ORDER BY i.created_at DESC;";

            return _dbConnection.Query<BookingListItem>(sql).ToList();
        }

        // ============================================================
        // GET BOOKING BY ID (full detail)
        // ============================================================
        public BookingDetailResponse? GetBookingById(long instanceId)
        {
            string sqlInstance = @"
                SELECT
                    i.id AS instance_id,
                    i.template_itinerary_id AS template_id,
                    i.source_instance_id,
                    i.booking_reference,
                    i.status,
                    i.is_customized,
                    i.total_amount,
                    i.amount_paid,
                    i.balance_amount,
                    i.payment_status,
                    i.traveler_approved,
                    i.admin_approved,
                    i.start_date,
                    i.end_date,
                    i.created_at,
                    i.special_requests,
                    t.title AS template_title
                FROM itinerary_instances i
                JOIN itineraries t ON t.id = i.template_itinerary_id
                WHERE i.id = @Id;";

            var booking = _dbConnection.QuerySingleOrDefault<BookingDetailResponse>(sqlInstance, new { Id = instanceId });
            if (booking == null) return null;

            string sqlDays = @"
                SELECT id, day_number, date, title, location, accommodation, transport,
                       breakfast_included, lunch_included, dinner_included
                FROM itinerary_instance_days
                WHERE itinerary_instance_id = @InstanceId
                ORDER BY day_number;";

            var days = _dbConnection.Query<BookingDayResponse>(sqlDays, new { InstanceId = instanceId }).ToList();

            foreach (var day in days)
            {
                string sqlActivities = @"
                    SELECT activity FROM itinerary_instance_day_activities
                    WHERE instance_day_id = @DayId;";

                day.Activities = _dbConnection.Query<string>(sqlActivities, new { DayId = day.Id }).ToList();
            }
            booking.Days = days;

            string sqlTravelers = @"
                SELECT id, full_name, contact_number, email, nationality, adults, children, seniors
                FROM travelers
                WHERE itinerary_instance_id = @InstanceId;";

            booking.Travelers = _dbConnection.Query<TravelerResponse>(sqlTravelers, new { InstanceId = instanceId }).ToList();

            string sqlPayments = @"
                SELECT id AS payment_id, amount, currency, payment_method, payment_date, status, transaction_reference
                FROM payments
                WHERE itinerary_instance_id = @InstanceId
                ORDER BY payment_date DESC;";

            booking.Payments = _dbConnection.Query<PaymentResponse>(sqlPayments, new { InstanceId = instanceId }).ToList();

            return booking;
        }

        // ============================================================
        // CUSTOMIZE A SINGLE INSTANCE DAY
        // ============================================================
        public BookingDayResponse? CustomizeDay(long instanceId, CustomizeDayRequest request)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                string sqlCheck = @"
                    SELECT COUNT(*) FROM itinerary_instance_days
                    WHERE id = @DayId AND itinerary_instance_id = @InstanceId;";

                int count = _dbConnection.ExecuteScalar<int>(sqlCheck, new
                {
                    DayId = request.InstanceDayId,
                    InstanceId = instanceId
                }, transaction);

                if (count == 0) return null;

                string sqlUpdate = @"
                    UPDATE itinerary_instance_days
                    SET title = COALESCE(@Title, title),
                        location = COALESCE(@Location, location),
                        accommodation = COALESCE(@Accommodation, accommodation),
                        transport = COALESCE(@Transport, transport),
                        breakfast_included = COALESCE(@BreakfastIncluded, breakfast_included),
                        lunch_included = COALESCE(@LunchIncluded, lunch_included),
                        dinner_included = COALESCE(@DinnerIncluded, dinner_included)
                    WHERE id = @DayId AND itinerary_instance_id = @InstanceId;";

                _dbConnection.Execute(sqlUpdate, new
                {
                    DayId = request.InstanceDayId,
                    InstanceId = instanceId,
                    request.Title,
                    request.Location,
                    request.Accommodation,
                    request.Transport,
                    request.BreakfastIncluded,
                    request.LunchIncluded,
                    request.DinnerIncluded
                }, transaction);

                if (request.Activities != null)
                {
                    string sqlDeleteActivities = @"
                        DELETE FROM itinerary_instance_day_activities
                        WHERE instance_day_id = @DayId;";
                    _dbConnection.Execute(sqlDeleteActivities, new { DayId = request.InstanceDayId }, transaction);

                    foreach (var activity in request.Activities)
                    {
                        string sqlInsertActivity = @"
                            INSERT INTO itinerary_instance_day_activities (instance_day_id, activity)
                            VALUES (@DayId, @Activity);";
                        _dbConnection.Execute(sqlInsertActivity, new
                        {
                            DayId = request.InstanceDayId,
                            Activity = activity
                        }, transaction);
                    }
                }

                string sqlMarkCustomized = @"
                    UPDATE itinerary_instances
                    SET is_customized = true
                    WHERE id = @InstanceId;";
                _dbConnection.Execute(sqlMarkCustomized, new { InstanceId = instanceId }, transaction);

                transaction.Commit();

                return GetDayById(request.InstanceDayId);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // ============================================================
        // APPROVE BOOKING
        // ============================================================
        public bool ApproveBooking(long instanceId, ApproveBookingRequest request)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                string sqlApproval = @"
                    INSERT INTO itinerary_approvals
                    (itinerary_instance_id, approved_by, approved, remarks, approved_at)
                    VALUES (@InstanceId, @ApprovedBy, @Approved, @Remarks, NOW());";

                _dbConnection.Execute(sqlApproval, new
                {
                    InstanceId = instanceId,
                    request.ApprovedBy,
                    request.Approved,
                    request.Remarks
                }, transaction);

                string approvedByLower = request.ApprovedBy.ToLower();
                if (approvedByLower == "traveler")
                {
                    _dbConnection.Execute(
                        "UPDATE itinerary_instances SET traveler_approved = @Approved WHERE id = @Id",
                        new { request.Approved, Id = instanceId }, transaction);
                }
                else if (approvedByLower == "admin")
                {
                    _dbConnection.Execute(
                        "UPDATE itinerary_instances SET admin_approved = @Approved WHERE id = @Id",
                        new { request.Approved, Id = instanceId }, transaction);
                }

                var instance = _dbConnection.QuerySingleOrDefault<dynamic>(
                    "SELECT traveler_approved, admin_approved FROM itinerary_instances WHERE id = @Id",
                    new { Id = instanceId }, transaction);

                if (instance != null && (bool)instance.traveler_approved && (bool)instance.admin_approved)
                {
                    _dbConnection.Execute(
                        "UPDATE itinerary_instances SET status = 'Confirmed', confirmed_at = NOW() WHERE id = @Id",
                        new { Id = instanceId }, transaction);
                }
                else if (request.Approved)
                {
                    _dbConnection.Execute(
                        "UPDATE itinerary_instances SET status = 'PartiallyApproved' WHERE id = @Id",
                        new { Id = instanceId }, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // ============================================================
        // ADD PAYMENT
        // ============================================================
        public PaymentResponse AddPayment(long instanceId, AddPaymentRequest request)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                string sqlPayment = @"
                    INSERT INTO payments
                    (itinerary_instance_id, payment_method, amount, currency, payment_date, transaction_reference, status)
                    VALUES
                    (@InstanceId, @PaymentMethod, @Amount, @Currency, NOW(), @TransactionReference, 'Success')
                    RETURNING id;";

                long paymentId = _dbConnection.QuerySingle<long>(sqlPayment, new
                {
                    InstanceId = instanceId,
                    request.PaymentMethod,
                    request.Amount,
                    request.Currency,
                    request.TransactionReference
                }, transaction);

                decimal totalPaid = _dbConnection.ExecuteScalar<decimal>(
                    "SELECT COALESCE(SUM(amount), 0) FROM payments WHERE itinerary_instance_id = @Id AND status = 'Success'",
                    new { Id = instanceId }, transaction);

                decimal totalAmount = _dbConnection.ExecuteScalar<decimal>(
                    "SELECT total_amount FROM itinerary_instances WHERE id = @Id",
                    new { Id = instanceId }, transaction);

                string paymentStatus = totalPaid >= totalAmount && totalAmount > 0 ? "Paid" :
                                       totalPaid > 0 ? "PartiallyPaid" : "Unpaid";

                _dbConnection.Execute(@"
                    UPDATE itinerary_instances
                    SET amount_paid = @Paid, balance_amount = @Balance, payment_status = @PaymentStatus
                    WHERE id = @Id",
                    new
                    {
                        Paid = totalPaid,
                        Balance = totalAmount - totalPaid,
                        PaymentStatus = paymentStatus,
                        Id = instanceId
                    }, transaction);

                transaction.Commit();

                return new PaymentResponse
                {
                    PaymentId = paymentId,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    PaymentMethod = request.PaymentMethod,
                    PaymentDate = DateTime.UtcNow,
                    Status = "Success",
                    TransactionReference = request.TransactionReference
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // ============================================================
        // GET PAYMENTS
        // ============================================================
        public List<PaymentResponse> GetPayments(long instanceId)
        {
            string sql = @"
                SELECT id AS payment_id, amount, currency, payment_method, payment_date, status, transaction_reference
                FROM payments
                WHERE itinerary_instance_id = @InstanceId
                ORDER BY payment_date DESC;";

            return _dbConnection.Query<PaymentResponse>(sql, new { InstanceId = instanceId }).ToList();
        }

        // ============================================================
        // UPDATE BOOKING STATUS
        // ============================================================
        public bool UpdateStatus(long instanceId, string status)
        {
            string sql = "UPDATE itinerary_instances SET status = @Status WHERE id = @Id";
            int affected = _dbConnection.Execute(sql, new { Status = status, Id = instanceId });
            return affected > 0;
        }

        // ============================================================
        // HELPER: Get a single day by ID
        // ============================================================
        private BookingDayResponse? GetDayById(long dayId)
        {
            string sql = @"
                SELECT id, day_number, date, title, location, accommodation, transport,
                       breakfast_included, lunch_included, dinner_included
                FROM itinerary_instance_days
                WHERE id = @Id;";

            var day = _dbConnection.QuerySingleOrDefault<BookingDayResponse>(sql, new { Id = dayId });
            if (day == null) return null;

            string sqlActivities = "SELECT activity FROM itinerary_instance_day_activities WHERE instance_day_id = @DayId";
            day.Activities = _dbConnection.Query<string>(sqlActivities, new { DayId = dayId }).ToList();

            return day;
        }
    }
}
