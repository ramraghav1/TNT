using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Repository.Interfaces.TourAndTravels;

using System.Data;
using static Repository.DataModels.TourAndTravels.ItineraryDTO;

namespace Repository.Repositories.TourAndTravels
{
    public class ItineraryRepository : IItineraryRepository
    {
        private readonly IDbConnection _dbConnection;

        public ItineraryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // ============================================================
        // CREATE ITINERARY
        // ============================================================
        public ItineraryResponse CreateItinerary(CreateItineraryRequest request)
        {
            if (_dbConnection.State != System.Data.ConnectionState.Open)
                _dbConnection.Open();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    string insertItineraryQuery = @"
                INSERT INTO itineraries
                (title, description, duration_days, difficulty_level)
                VALUES
                (@Title, @Description, @DurationDays, @DifficultyLevel)
                RETURNING id;";

                    long itineraryId = _dbConnection.QuerySingle<long>(
                        insertItineraryQuery,
                        request,
                        transaction
                    );

                    if (request.Days != null && request.Days.Any())
                    {
                        foreach (var day in request.Days)
                        {
                            string insertDayQuery = @"
                        INSERT INTO itinerary_days
                        (itinerary_id, day_number, title, location, accommodation, transport,
                         breakfast_included, lunch_included, dinner_included)
                        VALUES
                        (@ItineraryId, @DayNumber, @Title, @Location, @Accommodation, @Transport,
                         @BreakfastIncluded, @LunchIncluded, @DinnerIncluded)
                        RETURNING id;";

                            long dayId = _dbConnection.QuerySingle<long>(
                                insertDayQuery,
                                new
                                {
                                    ItineraryId = itineraryId,
                                    day.DayNumber,
                                    day.Title,
                                    day.Location,
                                    day.Accommodation,
                                    day.Transport,
                                    day.BreakfastIncluded,
                                    day.LunchIncluded,
                                    day.DinnerIncluded
                                },
                                transaction
                            );

                            if (day.Activities != null && day.Activities.Any())
                            {
                                foreach (var activity in day.Activities)
                                {
                                    string insertActivityQuery = @"
                                INSERT INTO itinerary_day_activities
                                (itinerary_day_id, activity)
                                VALUES
                                (@DayId, @Activity);";

                                    _dbConnection.Execute(
                                        insertActivityQuery,
                                        new
                                        {
                                            DayId = dayId,
                                            Activity = activity
                                        },
                                        transaction
                                    );
                                }
                            }
                        }
                    }

                    transaction.Commit();

                    return new ItineraryResponse
                    {
                        Id = itineraryId,
                        Title = request.Title,
                        Description = request.Description,
                        DurationDays = request.DurationDays,
                        DifficultyLevel = request.DifficultyLevel,
                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        // ============================================================
        // GET ALL ITINERARIES
        // ============================================================
        public List<ItineraryResponse> GetAllItineraries()
        {
            string sql = "SELECT id, title, description, duration_days, difficulty_level FROM itineraries";
            return _dbConnection.Query<ItineraryResponse>(sql).ToList();
        }

        // ============================================================
        // GET ITINERARY BY ID (DETAIL)
        // ============================================================
        public ItineraryDetailResponse? GetItineraryById(long id)
        {
            string sqlItinerary = "SELECT id, title, description, duration_days, difficulty_level FROM itineraries WHERE id = @Id";
            var itinerary = _dbConnection.QuerySingleOrDefault<ItineraryDetailResponse>(sqlItinerary, new { Id = id });

            if (itinerary == null)
                return null;

            string sqlDays = @"
                SELECT id, day_number, title, location, accommodation, transport, breakfast_included, lunch_included, dinner_included
                FROM itinerary_days
                WHERE itinerary_id = @ItineraryId
                ORDER BY day_number";

            var days = _dbConnection.Query<ItineraryDayResponse>(sqlDays, new { ItineraryId = id }).ToList();
            itinerary.Days = days;

            return itinerary;
        }

        // ============================================================
        // UPDATE ITINERARY
        // ============================================================
        public ItineraryResponse? UpdateItinerary(long id, UpdateItineraryRequest request)
        {
            string sql = @"
                UPDATE itineraries
                SET title = COALESCE(@Title, title),
                    description = COALESCE(@Description, description),
                    duration_days = COALESCE(@DurationDays, duration_days),
                    difficulty_level = COALESCE(@DifficultyLevel, difficulty_level)
                WHERE id = @Id";

            var affected = _dbConnection.Execute(sql, new { request.Title, request.Description, request.DurationDays, request.DifficultyLevel, Id = id });
            if (affected == 0)
                return null;

            // Return updated entity
            return GetAllItineraries().FirstOrDefault(x => x.Id == id);
        }

        // ============================================================
        // DELETE ITINERARY
        // ============================================================
        public bool DeleteItinerary(long id)
        {
            string sqlDays = "DELETE FROM itinerary_days WHERE itinerary_id = @Id";
            _dbConnection.Execute(sqlDays, new { Id = id });

            string sql = "DELETE FROM itineraries WHERE id = @Id";
            var affected = _dbConnection.Execute(sql, new { Id = id });
            return affected > 0;
        }

        // ============================================================
        // ADD DAY TO ITINERARY
        // ============================================================
        public ItineraryDayResponse? AddDayToItinerary(long itineraryId, CreateItineraryDayRequest request)
        {
            string sql = @"
                INSERT INTO itinerary_days 
                    (itinerary_id, day_number, title, location, accommodation, transport, breakfast_included, lunch_included, dinner_included)
                VALUES 
                    (@ItineraryId, @DayNumber, @Title, @Location, @Accommodation, @Transport, @BreakfastIncluded, @LunchIncluded, @DinnerIncluded)
                RETURNING id;";

            var dayId = _dbConnection.ExecuteScalar<long>(sql, new
            {
                ItineraryId = itineraryId,
                DayNumber = request.DayNumber,
                request.Title,
                request.Location,
                request.Accommodation,
                request.Transport,
                request.BreakfastIncluded,
                request.LunchIncluded,
                request.DinnerIncluded
            });

            return new ItineraryDayResponse
            {
                Id = dayId,
                DayNumber = request.DayNumber,
                Title = request.Title,
                Location = request.Location,
                Accommodation = request.Accommodation,
                Transport = request.Transport,
                BreakfastIncluded = request.BreakfastIncluded,
                LunchIncluded = request.LunchIncluded,
                DinnerIncluded = request.DinnerIncluded,
                Activities = request.Activities
            };
        }

        // ============================================================
        // UPDATE DAY
        // ============================================================
        public ItineraryDayResponse? UpdateDay(long itineraryId, long dayId, UpdateItineraryDayRequest request)
        {
            string sql = @"
                UPDATE itinerary_days
                SET title = COALESCE(@Title, title),
                    location = COALESCE(@Location, location),
                    accommodation = COALESCE(@Accommodation, accommodation),
                    transport = COALESCE(@Transport, transport),
                    breakfast_included = COALESCE(@BreakfastIncluded, breakfast_included),
                    lunch_included = COALESCE(@LunchIncluded, lunch_included),
                    dinner_included = COALESCE(@DinnerIncluded, dinner_included)
                WHERE id = @DayId AND itinerary_id = @ItineraryId";

            var affected = _dbConnection.Execute(sql, new
            {
                request.Title,
                request.Location,
                request.Accommodation,
                request.Transport,
                request.BreakfastIncluded,
                request.LunchIncluded,
                request.DinnerIncluded,
                DayId = dayId,
                ItineraryId = itineraryId
            });

            if (affected == 0)
                return null;

            string sqlGet = "SELECT * FROM itinerary_days WHERE id = @Id";
            return _dbConnection.QuerySingleOrDefault<ItineraryDayResponse>(sqlGet, new { Id = dayId });
        }
    }
}