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
            string sql = @"
                INSERT INTO Itineraries (Title, Description, DurationDays, DifficultyLevel)
                VALUES (@Title, @Description, @DurationDays, @DifficultyLevel);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var id = _dbConnection.ExecuteScalar<long>(sql, request);

            return new ItineraryResponse
            {
                Id = id,
                Title = request.Title,
                Description = request.Description,
                DurationDays = request.DurationDays,
                DifficultyLevel = request.DifficultyLevel
            };
        }

        // ============================================================
        // GET ALL ITINERARIES
        // ============================================================
        public List<ItineraryResponse> GetAllItineraries()
        {
            string sql = "SELECT Id, Title, Description, DurationDays, DifficultyLevel FROM Itineraries";
            return _dbConnection.Query<ItineraryResponse>(sql).ToList();
        }

        // ============================================================
        // GET ITINERARY BY ID (DETAIL)
        // ============================================================
        public ItineraryDetailResponse? GetItineraryById(long id)
        {
            string sqlItinerary = "SELECT Id, Title, Description, DurationDays, DifficultyLevel FROM Itineraries WHERE Id = @Id";
            var itinerary = _dbConnection.QuerySingleOrDefault<ItineraryDetailResponse>(sqlItinerary, new { Id = id });

            if (itinerary == null)
                return null;

            string sqlDays = @"
                SELECT Id, DayNumber, Title, Location, Accommodation, Transport, BreakfastIncluded, LunchIncluded, DinnerIncluded
                FROM ItineraryDays
                WHERE ItineraryId = @ItineraryId
                ORDER BY DayNumber";

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
                UPDATE Itineraries
                SET Title = COALESCE(@Title, Title),
                    Description = COALESCE(@Description, Description),
                    DurationDays = COALESCE(@DurationDays, DurationDays),
                    DifficultyLevel = COALESCE(@DifficultyLevel, DifficultyLevel)
                WHERE Id = @Id";

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
            string sqlDays = "DELETE FROM ItineraryDays WHERE ItineraryId = @Id";
            _dbConnection.Execute(sqlDays, new { Id = id });

            string sql = "DELETE FROM Itineraries WHERE Id = @Id";
            var affected = _dbConnection.Execute(sql, new { Id = id });
            return affected > 0;
        }

        // ============================================================
        // ADD DAY TO ITINERARY
        // ============================================================
        public ItineraryDayResponse? AddDayToItinerary(long itineraryId, CreateItineraryDayRequest request)
        {
            string sql = @"
                INSERT INTO ItineraryDays 
                    (ItineraryId, DayNumber, Title, Location, Accommodation, Transport, BreakfastIncluded, LunchIncluded, DinnerIncluded)
                VALUES 
                    (@ItineraryId, @DayNumber, @Title, @Location, @Accommodation, @Transport, @BreakfastIncluded, @LunchIncluded, @DinnerIncluded);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

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
                UPDATE ItineraryDays
                SET Title = COALESCE(@Title, Title),
                    Location = COALESCE(@Location, Location),
                    Accommodation = COALESCE(@Accommodation, Accommodation),
                    Transport = COALESCE(@Transport, Transport),
                    BreakfastIncluded = COALESCE(@BreakfastIncluded, BreakfastIncluded),
                    LunchIncluded = COALESCE(@LunchIncluded, LunchIncluded),
                    DinnerIncluded = COALESCE(@DinnerIncluded, DinnerIncluded)
                WHERE Id = @DayId AND ItineraryId = @ItineraryId";

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

            string sqlGet = "SELECT * FROM ItineraryDays WHERE Id = @Id";
            return _dbConnection.QuerySingleOrDefault<ItineraryDayResponse>(sqlGet, new { Id = dayId });
        }
    }
}