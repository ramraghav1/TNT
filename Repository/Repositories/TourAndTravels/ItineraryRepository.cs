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
                (title, description, duration_days, difficulty_level, pricing_mode, overall_price)
                VALUES
                (@Title, @Description, @DurationDays, @DifficultyLevel, @PricingMode, @OverallPrice)
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
                         breakfast_included, lunch_included, dinner_included, daily_cost)
                        VALUES
                        (@ItineraryId, @DayNumber, @Title, @Location, @Accommodation, @Transport,
                         @BreakfastIncluded, @LunchIncluded, @DinnerIncluded, @DailyCost)
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
                                    day.DinnerIncluded,
                                    day.DailyCost
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

                            // Save day costs (price stored per-day in itinerary_day_costs)
                            if (day.Costs != null && day.Costs.Any())
                            {
                                foreach (var cost in day.Costs)
                                {
                                    // Upsert cost_item by name
                                    string upsertCostItem = @"
                                INSERT INTO cost_items (name, category, unit_type)
                                VALUES (@Name, @Category, 'per_person')
                                ON CONFLICT (name) DO UPDATE SET category = EXCLUDED.category
                                RETURNING id;";

                                    long costItemId = _dbConnection.QuerySingle<long>(
                                        upsertCostItem,
                                        new { cost.Name, cost.Category },
                                        transaction
                                    );

                                    // Link cost to this day with price
                                    _dbConnection.Execute(
                                        "INSERT INTO itinerary_day_costs (itinerary_day_id, cost_item_id, quantity, price, currency) VALUES (@DayId, @CostItemId, 1, @Price, 'NPR')",
                                        new { DayId = dayId, CostItemId = costItemId, cost.Price },
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
                        PricingMode = request.PricingMode,
                        OverallPrice = request.OverallPrice,
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
            string sql = "SELECT id, title, description, duration_days, difficulty_level, pricing_mode, overall_price FROM itineraries";
            return _dbConnection.Query<ItineraryResponse>(sql).ToList();
        }

        // ============================================================
        // GET ITINERARY BY ID (DETAIL)
        // ============================================================
        public ItineraryDetailResponse? GetItineraryById(long id)
        {
            string sqlItinerary = "SELECT id, title, description, duration_days, difficulty_level, pricing_mode, overall_price FROM itineraries WHERE id = @Id";
            var itinerary = _dbConnection.QuerySingleOrDefault<ItineraryDetailResponse>(sqlItinerary, new { Id = id });

            if (itinerary == null)
                return null;

            string sqlDays = @"
                SELECT id, day_number, title, location, accommodation, transport, breakfast_included, lunch_included, dinner_included, daily_cost
                FROM itinerary_days
                WHERE itinerary_id = @ItineraryId
                ORDER BY day_number";

            var days = _dbConnection.Query<ItineraryDayResponse>(sqlDays, new { ItineraryId = id }).ToList();

            var dayIds = days.Select(d => d.Id).ToArray();

            // Load activities for each day
            string sqlActivities = @"
                SELECT itinerary_day_id AS DayId, activity
                FROM itinerary_day_activities
                WHERE itinerary_day_id = ANY(@DayIds)";

            var allActivities = dayIds.Length > 0
                ? _dbConnection.Query<dynamic>(sqlActivities, new { DayIds = dayIds }).ToList()
                : new List<dynamic>();

            foreach (var day in days)
            {
                day.Activities = allActivities
                    .Where(a => (long)a.dayid == day.Id)
                    .Select(a => (string)a.activity)
                    .ToList();
            }

            // Load costs for each day (price stored directly in itinerary_day_costs)
            string sqlCosts = @"
                SELECT dc.itinerary_day_id AS DayId, ci.name AS Name, ci.category AS Category, dc.price AS Price
                FROM itinerary_day_costs dc
                JOIN cost_items ci ON ci.id = dc.cost_item_id
                WHERE dc.itinerary_day_id = ANY(@DayIds)";

            var allCosts = dayIds.Length > 0
                ? _dbConnection.Query<dynamic>(sqlCosts, new { DayIds = dayIds }).ToList()
                : new List<dynamic>();

            foreach (var day in days)
            {
                day.Costs = allCosts
                    .Where(c => (long)c.dayid == day.Id)
                    .Select(c => new DayCostInput
                    {
                        Name = (string)c.name,
                        Category = (string)(c.category ?? ""),
                        Price = c.price != null ? (decimal)c.price : 0
                    })
                    .ToList();
            }

            itinerary.Days = days;

            return itinerary;
        }

        // ============================================================
        // UPDATE ITINERARY (full replace of days, activities, costs)
        // ============================================================
        public ItineraryResponse? UpdateItinerary(long id, UpdateItineraryRequest request)
        {
            if (_dbConnection.State != System.Data.ConnectionState.Open)
                _dbConnection.Open();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // 1. Update itinerary header
                    string updateSql = @"
                        UPDATE itineraries
                        SET title = @Title,
                            description = @Description,
                            duration_days = @DurationDays,
                            difficulty_level = @DifficultyLevel,
                            pricing_mode = @PricingMode,
                            overall_price = @OverallPrice
                        WHERE id = @Id";

                    var affected = _dbConnection.Execute(updateSql,
                        new { request.Title, request.Description, request.DurationDays, request.DifficultyLevel, request.PricingMode, request.OverallPrice, Id = id },
                        transaction);

                    if (affected == 0)
                    {
                        transaction.Rollback();
                        return null;
                    }

                    // 2. Get existing day IDs for cleanup
                    var existingDayIds = _dbConnection.Query<long>(
                        "SELECT id FROM itinerary_days WHERE itinerary_id = @Id", new { Id = id }, transaction).ToList();

                    if (existingDayIds.Any())
                    {
                        // Delete costs linked to these days
                        _dbConnection.Execute(
                            "DELETE FROM itinerary_day_costs WHERE itinerary_day_id = ANY(@Ids)",
                            new { Ids = existingDayIds.ToArray() }, transaction);

                        // Delete activities
                        _dbConnection.Execute(
                            "DELETE FROM itinerary_day_activities WHERE itinerary_day_id = ANY(@Ids)",
                            new { Ids = existingDayIds.ToArray() }, transaction);

                        // Delete days
                        _dbConnection.Execute(
                            "DELETE FROM itinerary_days WHERE itinerary_id = @Id",
                            new { Id = id }, transaction);
                    }

                    // 3. Re-insert days with activities and costs (same logic as Create)
                    if (request.Days != null && request.Days.Any())
                    {
                        foreach (var day in request.Days)
                        {
                            string insertDayQuery = @"
                                INSERT INTO itinerary_days
                                (itinerary_id, day_number, title, location, accommodation, transport,
                                 breakfast_included, lunch_included, dinner_included, daily_cost)
                                VALUES
                                (@ItineraryId, @DayNumber, @Title, @Location, @Accommodation, @Transport,
                                 @BreakfastIncluded, @LunchIncluded, @DinnerIncluded, @DailyCost)
                                RETURNING id;";

                            long dayId = _dbConnection.QuerySingle<long>(
                                insertDayQuery,
                                new
                                {
                                    ItineraryId = id,
                                    day.DayNumber,
                                    day.Title,
                                    day.Location,
                                    day.Accommodation,
                                    day.Transport,
                                    day.BreakfastIncluded,
                                    day.LunchIncluded,
                                    day.DinnerIncluded,
                                    day.DailyCost
                                },
                                transaction);

                            // Activities
                            if (day.Activities != null && day.Activities.Any())
                            {
                                foreach (var activity in day.Activities)
                                {
                                    _dbConnection.Execute(
                                        "INSERT INTO itinerary_day_activities (itinerary_day_id, activity) VALUES (@DayId, @Activity)",
                                        new { DayId = dayId, Activity = activity },
                                        transaction);
                                }
                            }

                            // Costs
                            if (day.Costs != null && day.Costs.Any())
                            {
                                foreach (var cost in day.Costs)
                                {
                                    string upsertCostItem = @"
                                        INSERT INTO cost_items (name, category, unit_type)
                                        VALUES (@Name, @Category, 'per_person')
                                        ON CONFLICT (name) DO UPDATE SET category = EXCLUDED.category
                                        RETURNING id;";

                                    long costItemId = _dbConnection.QuerySingle<long>(
                                        upsertCostItem, new { cost.Name, cost.Category }, transaction);

                                    _dbConnection.Execute(
                                        "INSERT INTO itinerary_day_costs (itinerary_day_id, cost_item_id, quantity, price, currency) VALUES (@DayId, @CostItemId, 1, @Price, 'NPR')",
                                        new { DayId = dayId, CostItemId = costItemId, cost.Price }, transaction);
                                }
                            }
                        }
                    }

                    transaction.Commit();

                    return new ItineraryResponse
                    {
                        Id = id,
                        Title = request.Title,
                        Description = request.Description,
                        DurationDays = request.DurationDays,
                        DifficultyLevel = request.DifficultyLevel,
                        PricingMode = request.PricingMode,
                        OverallPrice = request.OverallPrice,
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