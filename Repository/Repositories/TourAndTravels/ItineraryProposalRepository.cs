using System;
using System.Data;
using System.Linq;
using Dapper;
using System.Collections.Generic;
using Repository.Interfaces.TourAndTravels;
using static Repository.DataModels.TourAndTravels.ItineraryProposalDTO;

namespace Repository.Repositories.TourAndTravels
{
    public class ItineraryProposalRepository : IItineraryProposalRepository
    {
        private readonly IDbConnection _dbConnection;

        public ItineraryProposalRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // ============================================================
        // CREATE PROPOSAL
        // ============================================================
        public ProposalResponse CreateProposal(CreateProposalRequest request, string token)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                string sqlProposal = @"
                    INSERT INTO itinerary_proposals
                    (itinerary_id, token, traveler_name, traveler_email, traveler_phone,
                     status, start_date, end_date, total_amount, notes, created_at, expires_at)
                    VALUES
                    (@ItineraryId, @Token, @TravelerName, @TravelerEmail, @TravelerPhone,
                     'Sent', @StartDate, @EndDate, @TotalAmount, @Notes, NOW(), NOW() + INTERVAL '7 days')
                    RETURNING id;";

                long proposalId = _dbConnection.QuerySingle<long>(sqlProposal, new
                {
                    request.ItineraryId,
                    Token = token,
                    request.TravelerName,
                    request.TravelerEmail,
                    request.TravelerPhone,
                    request.StartDate,
                    request.EndDate,
                    request.TotalAmount,
                    request.Notes
                }, transaction);

                // Insert proposal days
                foreach (var day in request.Days)
                {
                    string sqlDay = @"
                        INSERT INTO itinerary_proposal_days
                        (proposal_id, day_number, title, description, location, accommodation, transport,
                         breakfast_included, lunch_included, dinner_included, activities, daily_cost)
                        VALUES
                        (@ProposalId, @DayNumber, @Title, @Description, @Location, @Accommodation, @Transport,
                         @BreakfastIncluded, @LunchIncluded, @DinnerIncluded, @Activities, @DailyCost);";

                    _dbConnection.Execute(sqlDay, new
                    {
                        ProposalId = proposalId,
                        day.DayNumber,
                        day.Title,
                        day.Description,
                        day.Location,
                        day.Accommodation,
                        day.Transport,
                        day.BreakfastIncluded,
                        day.LunchIncluded,
                        day.DinnerIncluded,
                        Activities = string.Join("||", day.Activities ?? new List<string>()),
                        day.DailyCost
                    }, transaction);
                }

                transaction.Commit();

                return GetProposalById(proposalId)!;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // ============================================================
        // GET ALL PROPOSALS
        // ============================================================
        public List<ProposalListItem> GetAllProposals()
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            string sql = @"
                SELECT p.id, p.token, p.traveler_name, p.traveler_email,
                       t.title AS itinerary_title, p.status, p.total_amount,
                       p.start_date, p.end_date, p.created_at, p.expires_at
                FROM itinerary_proposals p
                JOIN itineraries t ON t.id = p.itinerary_id
                ORDER BY p.created_at DESC;";

            var rows = _dbConnection.Query(sql);
            return rows.Select(r => new ProposalListItem
            {
                Id = r.id,
                Token = r.token,
                TravelerName = r.traveler_name,
                TravelerEmail = r.traveler_email,
                ItineraryTitle = r.itinerary_title,
                Status = r.status,
                TotalAmount = r.total_amount,
                StartDate = r.start_date is DateOnly sd ? sd.ToDateTime(TimeOnly.MinValue) : (DateTime)r.start_date,
                EndDate = r.end_date is DateOnly ed ? ed.ToDateTime(TimeOnly.MinValue) : (DateTime)r.end_date,
                CreatedAt = r.created_at is DateOnly ca ? ca.ToDateTime(TimeOnly.MinValue) : (DateTime)r.created_at,
                ExpiresAt = r.expires_at is DateOnly ea ? ea.ToDateTime(TimeOnly.MinValue) : (DateTime)r.expires_at
            }).ToList();
        }

        // ============================================================
        // GET PROPOSAL BY ID
        // ============================================================
        public ProposalResponse? GetProposalById(long id)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            string sql = @"
                SELECT p.*, t.title AS itinerary_title
                FROM itinerary_proposals p
                JOIN itineraries t ON t.id = p.itinerary_id
                WHERE p.id = @Id;";

            var row = _dbConnection.QueryFirstOrDefault(sql, new { Id = id });
            if (row == null) return null;

            return MapProposalResponse(row, id);
        }

        // ============================================================
        // GET PROPOSAL BY TOKEN (public)
        // ============================================================
        public ProposalResponse? GetProposalByToken(string token)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            string sql = @"
                SELECT p.*, t.title AS itinerary_title
                FROM itinerary_proposals p
                JOIN itineraries t ON t.id = p.itinerary_id
                WHERE p.token = @Token;";

            var row = _dbConnection.QueryFirstOrDefault(sql, new { Token = token });
            if (row == null) return null;

            return MapProposalResponse(row, (long)row.id);
        }

        // ============================================================
        // UPDATE STATUS
        // ============================================================
        public bool UpdateProposalStatus(long id, string status)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            string sql = @"UPDATE itinerary_proposals SET status = @Status WHERE id = @Id;";
            int affected = _dbConnection.Execute(sql, new { Id = id, Status = status });
            return affected > 0;
        }

        // ============================================================
        // SUBMIT PAYMENT
        // ============================================================
        public ProposalPaymentResponse SubmitPayment(long proposalId, SubmitProposalPaymentRequest request)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            string sql = @"
                INSERT INTO itinerary_proposal_payments
                (proposal_id, payment_type, amount, screenshot_path, transaction_reference, status, created_at)
                VALUES
                (@ProposalId, @PaymentType, @Amount, @ScreenshotPath, @TransactionReference, 'Pending', NOW())
                RETURNING id, proposal_id, payment_type, amount, screenshot_path, transaction_reference, status, created_at;";

            var row = _dbConnection.QuerySingle(sql, new
            {
                ProposalId = proposalId,
                request.PaymentType,
                request.Amount,
                request.ScreenshotPath,
                request.TransactionReference
            });

            // Update proposal status to PaymentSubmitted
            _dbConnection.Execute(
                "UPDATE itinerary_proposals SET status = 'PaymentSubmitted' WHERE id = @Id;",
                new { Id = proposalId });

            return new ProposalPaymentResponse
            {
                Id = row.id,
                ProposalId = row.proposal_id,
                PaymentType = row.payment_type,
                Amount = row.amount,
                ScreenshotPath = row.screenshot_path,
                TransactionReference = row.transaction_reference,
                Status = row.status,
                CreatedAt = row.created_at
            };
        }

        // ============================================================
        // GET PAYMENTS
        // ============================================================
        public List<ProposalPaymentResponse> GetPayments(long proposalId)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            string sql = @"
                SELECT id, proposal_id, payment_type, amount, screenshot_path,
                       transaction_reference, status, verified_by, created_at, verified_at
                FROM itinerary_proposal_payments
                WHERE proposal_id = @ProposalId
                ORDER BY created_at DESC;";

            var rows = _dbConnection.Query(sql, new { ProposalId = proposalId });
            return rows.Select(r => new ProposalPaymentResponse
            {
                Id = r.id,
                ProposalId = r.proposal_id,
                PaymentType = r.payment_type,
                Amount = r.amount,
                ScreenshotPath = r.screenshot_path,
                TransactionReference = r.transaction_reference,
                Status = r.status,
                VerifiedBy = r.verified_by,
                CreatedAt = r.created_at,
                VerifiedAt = r.verified_at
            }).ToList();
        }

        // ============================================================
        // VERIFY PAYMENT
        // ============================================================
        public bool VerifyPayment(long paymentId, string verifiedBy, bool approved, string? remarks)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                string status = approved ? "Verified" : "Rejected";
                string sql = @"
                    UPDATE itinerary_proposal_payments
                    SET status = @Status, verified_by = @VerifiedBy, verified_at = NOW()
                    WHERE id = @PaymentId
                    RETURNING proposal_id;";

                var proposalId = _dbConnection.QuerySingleOrDefault<long?>(sql, new
                {
                    PaymentId = paymentId,
                    Status = status,
                    VerifiedBy = verifiedBy
                }, transaction);

                if (proposalId == null)
                {
                    transaction.Rollback();
                    return false;
                }

                if (approved)
                {
                    // Update the proposal status to Confirmed and create a booking
                    _dbConnection.Execute(
                        "UPDATE itinerary_proposals SET status = 'Confirmed' WHERE id = @Id;",
                        new { Id = proposalId.Value }, transaction);
                }
                else
                {
                    _dbConnection.Execute(
                        "UPDATE itinerary_proposals SET status = 'PaymentRejected' WHERE id = @Id;",
                        new { Id = proposalId.Value }, transaction);
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
        // SUBMIT FEEDBACK
        // ============================================================
        public ProposalFeedbackResponse SubmitFeedback(long proposalId, SubmitFeedbackRequest request)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            string sql = @"
                INSERT INTO itinerary_proposal_feedback
                (proposal_id, message, created_at)
                VALUES
                (@ProposalId, @Message, NOW())
                RETURNING id, proposal_id, message, created_at;";

            var row = _dbConnection.QuerySingle(sql, new
            {
                ProposalId = proposalId,
                request.Message
            });

            // Update proposal status to FeedbackReceived
            _dbConnection.Execute(
                "UPDATE itinerary_proposals SET status = 'FeedbackReceived' WHERE id = @Id;",
                new { Id = proposalId });

            return new ProposalFeedbackResponse
            {
                Id = row.id,
                ProposalId = row.proposal_id,
                Message = row.message,
                CreatedAt = row.created_at
            };
        }

        // ============================================================
        // GET FEEDBACK
        // ============================================================
        public List<ProposalFeedbackResponse> GetFeedback(long proposalId)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            string sql = @"
                SELECT id, proposal_id, message, created_at
                FROM itinerary_proposal_feedback
                WHERE proposal_id = @ProposalId
                ORDER BY created_at DESC;";

            var rows = _dbConnection.Query(sql, new { ProposalId = proposalId });
            return rows.Select(r => new ProposalFeedbackResponse
            {
                Id = r.id,
                ProposalId = r.proposal_id,
                Message = r.message,
                CreatedAt = r.created_at
            }).ToList();
        }

        // ============================================================
        // UPDATE PAYMENT SCREENSHOT
        // ============================================================
        public bool UpdatePaymentScreenshot(long paymentId, string screenshotPath)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            string sql = @"UPDATE itinerary_proposal_payments SET screenshot_path = @Path WHERE id = @Id;";
            int affected = _dbConnection.Execute(sql, new { Id = paymentId, Path = screenshotPath });
            return affected > 0;
        }

        // ============================================================
        // UPDATE PROPOSAL (re-customize and re-send)
        // ============================================================
        public ProposalResponse? UpdateProposal(long id, CreateProposalRequest request)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                // Update proposal header
                string sqlUpdate = @"
                    UPDATE itinerary_proposals
                    SET traveler_name = @TravelerName, traveler_email = @TravelerEmail,
                        traveler_phone = @TravelerPhone, start_date = @StartDate,
                        end_date = @EndDate, total_amount = @TotalAmount,
                        notes = @Notes, status = 'Sent', expires_at = NOW() + INTERVAL '7 days'
                    WHERE id = @Id;";

                int affected = _dbConnection.Execute(sqlUpdate, new
                {
                    Id = id,
                    request.TravelerName,
                    request.TravelerEmail,
                    request.TravelerPhone,
                    request.StartDate,
                    request.EndDate,
                    request.TotalAmount,
                    request.Notes
                }, transaction);

                if (affected == 0)
                {
                    transaction.Rollback();
                    return null;
                }

                // Delete old days and insert new
                _dbConnection.Execute("DELETE FROM itinerary_proposal_days WHERE proposal_id = @Id;", new { Id = id }, transaction);

                foreach (var day in request.Days)
                {
                    string sqlDay = @"
                        INSERT INTO itinerary_proposal_days
                        (proposal_id, day_number, title, description, location, accommodation, transport,
                         breakfast_included, lunch_included, dinner_included, activities, daily_cost)
                        VALUES
                        (@ProposalId, @DayNumber, @Title, @Description, @Location, @Accommodation, @Transport,
                         @BreakfastIncluded, @LunchIncluded, @DinnerIncluded, @Activities, @DailyCost);";

                    _dbConnection.Execute(sqlDay, new
                    {
                        ProposalId = id,
                        day.DayNumber,
                        day.Title,
                        day.Description,
                        day.Location,
                        day.Accommodation,
                        day.Transport,
                        day.BreakfastIncluded,
                        day.LunchIncluded,
                        day.DinnerIncluded,
                        Activities = string.Join("||", day.Activities ?? new List<string>()),
                        day.DailyCost
                    }, transaction);
                }

                transaction.Commit();
                return GetProposalById(id);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // ============================================================
        // HELPERS
        // ============================================================
        private ProposalResponse MapProposalResponse(dynamic row, long proposalId)
        {
            var response = new ProposalResponse
            {
                Id = row.id,
                ItineraryId = row.itinerary_id,
                Token = row.token,
                TravelerName = row.traveler_name,
                TravelerEmail = row.traveler_email,
                TravelerPhone = row.traveler_phone,
                ItineraryTitle = row.itinerary_title,
                Status = row.status,
                StartDate = row.start_date is DateOnly sd ? sd.ToDateTime(TimeOnly.MinValue) : (DateTime)row.start_date,
                EndDate = row.end_date is DateOnly ed ? ed.ToDateTime(TimeOnly.MinValue) : (DateTime)row.end_date,
                TotalAmount = row.total_amount,
                Notes = row.notes,
                CreatedAt = row.created_at is DateOnly ca ? ca.ToDateTime(TimeOnly.MinValue) : (DateTime)row.created_at,
                ExpiresAt = row.expires_at is DateOnly ea ? ea.ToDateTime(TimeOnly.MinValue) : (DateTime)row.expires_at
            };

            // Load days
            string sqlDays = @"
                SELECT id, day_number, title, description, location, accommodation, transport,
                       breakfast_included, lunch_included, dinner_included, activities, daily_cost
                FROM itinerary_proposal_days
                WHERE proposal_id = @ProposalId
                ORDER BY day_number;";

            var dayRows = _dbConnection.Query(sqlDays, new { ProposalId = proposalId });
            response.Days = dayRows.Select(d => new ProposalDayResponse
            {
                Id = d.id,
                DayNumber = d.day_number,
                Title = d.title,
                Description = d.description,
                Location = d.location,
                Accommodation = d.accommodation,
                Transport = d.transport,
                BreakfastIncluded = d.breakfast_included,
                LunchIncluded = d.lunch_included,
                DinnerIncluded = d.dinner_included,
                Activities = ((string?)d.activities)?.Split("||", StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                DailyCost = d.daily_cost
            }).ToList();

            return response;
        }
    }
}
