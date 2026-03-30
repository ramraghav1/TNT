using System;
using System.Data;
using System.Linq;
using Dapper;
using System.Collections.Generic;
using static Repository.DataModels.TourAndTravels.PricingDTO;
using Repository.Interfaces.TourAndTravels;

namespace Repository.Repositories.TourAndTravels
{
    public class PricingRepository : IPricingRepository
    {
        private readonly IDbConnection _dbConnection;

        public PricingRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // ============================================================
        // COST ITEMS CRUD
        // ============================================================
        public CostItemResponse CreateCostItem(CreateCostItemRequest request)
        {
            string sql = @"
                INSERT INTO cost_items (name, category, unit_type, is_active, created_at)
                VALUES (@Name, @Category, @UnitType, true, NOW())
                RETURNING id, name, category, unit_type, is_active, created_at;";

            return _dbConnection.QuerySingle<CostItemResponse>(sql, request);
        }

        public List<CostItemResponse> GetAllCostItems()
        {
            string sql = "SELECT id, name, category, unit_type, is_active, created_at FROM cost_items ORDER BY category, name;";
            return _dbConnection.Query<CostItemResponse>(sql).ToList();
        }

        public CostItemResponse? UpdateCostItem(long id, UpdateCostItemRequest request)
        {
            string sql = @"
                UPDATE cost_items
                SET name = COALESCE(@Name, name),
                    category = COALESCE(@Category, category),
                    unit_type = COALESCE(@UnitType, unit_type),
                    is_active = COALESCE(@IsActive, is_active)
                WHERE id = @Id
                RETURNING id, name, category, unit_type, is_active, created_at;";

            return _dbConnection.QuerySingleOrDefault<CostItemResponse>(sql, new
            {
                request.Name,
                request.Category,
                request.UnitType,
                request.IsActive,
                Id = id
            });
        }

        public bool DeleteCostItem(long id)
        {
            string sql = "DELETE FROM cost_items WHERE id = @Id";
            return _dbConnection.Execute(sql, new { Id = id }) > 0;
        }

        // ============================================================
        // COST ITEM RATES
        // ============================================================
        public CostItemRateResponse CreateRate(CreateCostItemRateRequest request)
        {
            string sql = @"
                INSERT INTO cost_item_rates (cost_item_id, location, itinerary_id, price, currency, effective_from, effective_to)
                VALUES (@CostItemId, @Location, @ItineraryId, @Price, @Currency, @EffectiveFrom, @EffectiveTo)
                RETURNING id;";

            long rateId = _dbConnection.QuerySingle<long>(sql, request);

            string sqlGet = @"
                SELECT r.id, r.cost_item_id, c.name AS cost_item_name, r.location, r.itinerary_id,
                       r.price, r.currency, r.effective_from, r.effective_to
                FROM cost_item_rates r
                JOIN cost_items c ON c.id = r.cost_item_id
                WHERE r.id = @Id;";

            return _dbConnection.QuerySingle<CostItemRateResponse>(sqlGet, new { Id = rateId });
        }

        public List<CostItemRateResponse> GetRatesByCostItem(long costItemId)
        {
            string sql = @"
                SELECT r.id, r.cost_item_id, c.name AS cost_item_name, r.location, r.itinerary_id,
                       r.price, r.currency, r.effective_from, r.effective_to
                FROM cost_item_rates r
                JOIN cost_items c ON c.id = r.cost_item_id
                WHERE r.cost_item_id = @CostItemId
                ORDER BY r.effective_from DESC NULLS LAST;";

            return _dbConnection.Query<CostItemRateResponse>(sql, new { CostItemId = costItemId }).ToList();
        }

        public List<CostItemRateResponse> GetRatesByItinerary(long itineraryId)
        {
            string sql = @"
                SELECT r.id, r.cost_item_id, c.name AS cost_item_name, r.location, r.itinerary_id,
                       r.price, r.currency, r.effective_from, r.effective_to
                FROM cost_item_rates r
                JOIN cost_items c ON c.id = r.cost_item_id
                WHERE r.itinerary_id = @ItineraryId OR r.itinerary_id IS NULL
                ORDER BY c.category, c.name;";

            return _dbConnection.Query<CostItemRateResponse>(sql, new { ItineraryId = itineraryId }).ToList();
        }

        // ============================================================
        // ITINERARY DAY COSTS (Template)
        // ============================================================
        public DayCostResponse AssignDayCost(AssignDayCostRequest request)
        {
            string sql = @"
                INSERT INTO itinerary_day_costs (itinerary_day_id, cost_item_id, quantity)
                VALUES (@ItineraryDayId, @CostItemId, @Quantity)
                RETURNING id;";

            long id = _dbConnection.QuerySingle<long>(sql, request);

            return GetDayCostById(id)!;
        }

        public List<DayCostResponse> GetDayCosts(long itineraryDayId)
        {
            string sql = @"
                SELECT dc.id, dc.itinerary_day_id, dc.cost_item_id, c.name AS cost_item_name,
                       c.category, c.unit_type, dc.quantity,
                       COALESCE(r.price, 0) AS unit_price,
                       dc.quantity * COALESCE(r.price, 0) AS total_price
                FROM itinerary_day_costs dc
                JOIN cost_items c ON c.id = dc.cost_item_id
                LEFT JOIN LATERAL (
                    SELECT price FROM cost_item_rates
                    WHERE cost_item_id = dc.cost_item_id
                    ORDER BY effective_from DESC NULLS LAST
                    LIMIT 1
                ) r ON true
                WHERE dc.itinerary_day_id = @DayId
                ORDER BY c.category, c.name;";

            return _dbConnection.Query<DayCostResponse>(sql, new { DayId = itineraryDayId }).ToList();
        }

        public List<DayCostResponse> GetAllDayCostsByItinerary(long itineraryId)
        {
            string sql = @"
                SELECT dc.id, dc.itinerary_day_id, dc.cost_item_id, c.name AS cost_item_name,
                       c.category, c.unit_type, dc.quantity,
                       COALESCE(r.price, 0) AS unit_price,
                       dc.quantity * COALESCE(r.price, 0) AS total_price
                FROM itinerary_day_costs dc
                JOIN cost_items c ON c.id = dc.cost_item_id
                JOIN itinerary_days d ON d.id = dc.itinerary_day_id
                LEFT JOIN LATERAL (
                    SELECT price FROM cost_item_rates
                    WHERE cost_item_id = dc.cost_item_id
                    ORDER BY effective_from DESC NULLS LAST
                    LIMIT 1
                ) r ON true
                WHERE d.itinerary_id = @ItineraryId
                ORDER BY d.day_number, c.category, c.name;";

            return _dbConnection.Query<DayCostResponse>(sql, new { ItineraryId = itineraryId }).ToList();
        }

        public bool RemoveDayCost(long id)
        {
            return _dbConnection.Execute("DELETE FROM itinerary_day_costs WHERE id = @Id", new { Id = id }) > 0;
        }

        // ============================================================
        // BOOKING PRICING (Instance)
        // ============================================================
        public BookingPricingResponse GetBookingPricing(long instanceId)
        {
            // Get instance amounts
            string sqlInstance = @"
                SELECT total_amount, amount_paid, balance_amount
                FROM itinerary_instances WHERE id = @Id;";
            var instance = _dbConnection.QuerySingleOrDefault<dynamic>(sqlInstance, new { Id = instanceId });

            var response = new BookingPricingResponse
            {
                TotalAmount = instance?.total_amount ?? 0,
                AmountPaid = instance?.amount_paid ?? 0,
                BalanceAmount = instance?.balance_amount ?? 0
            };

            // Get day costs from instance_day_costs if they exist, otherwise calculate from template
            string sqlDayCosts = @"
                SELECT idc.id, id2.day_number, id2.location,
                       c.name, c.category,
                       idc.unit_price, idc.quantity, idc.total_price
                FROM itinerary_instance_day_costs idc
                JOIN itinerary_instance_days id2 ON id2.id = idc.instance_day_id
                JOIN cost_items c ON c.id = idc.cost_item_id
                WHERE id2.itinerary_instance_id = @InstanceId
                ORDER BY id2.day_number, c.category, c.name;";

            var costs = _dbConnection.Query<dynamic>(sqlDayCosts, new { InstanceId = instanceId }).ToList();

            var dayCostMap = new Dictionary<int, BookingDayCostResponse>();
            foreach (var cost in costs)
            {
                int dayNum = (int)cost.day_number;
                if (!dayCostMap.ContainsKey(dayNum))
                {
                    dayCostMap[dayNum] = new BookingDayCostResponse
                    {
                        DayNumber = dayNum,
                        Location = (string?)cost.location
                    };
                }
                dayCostMap[dayNum].CostItems.Add(new BookingCostItemResponse
                {
                    Name = (string)cost.name,
                    Category = (string)cost.category,
                    UnitPrice = (decimal)cost.unit_price,
                    Quantity = (int)cost.quantity,
                    TotalPrice = (decimal)cost.total_price
                });
            }

            response.DayCosts = dayCostMap.Values.OrderBy(d => d.DayNumber).ToList();
            return response;
        }

        // ============================================================
        // HELPER
        // ============================================================
        private DayCostResponse? GetDayCostById(long id)
        {
            string sql = @"
                SELECT dc.id, dc.itinerary_day_id, dc.cost_item_id, c.name AS cost_item_name,
                       c.category, c.unit_type, dc.quantity,
                       COALESCE(r.price, 0) AS unit_price,
                       dc.quantity * COALESCE(r.price, 0) AS total_price
                FROM itinerary_day_costs dc
                JOIN cost_items c ON c.id = dc.cost_item_id
                LEFT JOIN LATERAL (
                    SELECT price FROM cost_item_rates
                    WHERE cost_item_id = dc.cost_item_id
                    ORDER BY effective_from DESC NULLS LAST
                    LIMIT 1
                ) r ON true
                WHERE dc.id = @Id;";

            return _dbConnection.QuerySingleOrDefault<DayCostResponse>(sql, new { Id = id });
        }
    }
}
