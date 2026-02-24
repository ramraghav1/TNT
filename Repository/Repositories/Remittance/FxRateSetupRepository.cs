using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Remittance;
using static Repository.DataModels.Remittance.FxRateSetupDTO;

namespace Repository.Repositories.Remittance
{
    public class FxRateSetupRepository : IFxRateSetupRepository
    {
        private readonly IDbConnection _dbConnection;

        public FxRateSetupRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // ============================================================
        // CREATE
        // ============================================================
        public FxRateDetailResponse Create(CreateFxRateRequest request)
        {
            string sql = @"
                INSERT INTO fx_rate_setups
                    (sending_country_id, receiving_country_id, payment_type_id, agent_id,
                     sending_currency, receiving_currency, settlement_currency,
                     customer_rate, settlement_rate, cross_rate,
                     margin_type, margin_value,
                     valid_from, valid_to,
                     is_active, created_by, created_at)
                VALUES
                    (@SendingCountryId, @ReceivingCountryId, @PaymentTypeId, @AgentId,
                     @SendingCurrency, @ReceivingCurrency, @SettlementCurrency,
                     @CustomerRate, @SettlementRate, @CrossRate,
                     @MarginType, @MarginValue,
                     @ValidFrom, @ValidTo,
                     true, @CreatedBy, NOW())
                RETURNING id;";

            long id = _dbConnection.QuerySingle<long>(sql, request);

            // Insert initial history record
            InsertHistory(id, null, request.CustomerRate, null, request.SettlementRate,
                          null, request.CrossRate, request.CreatedBy, "Initial rate setup");

            return GetById(id)!;
        }

        // ============================================================
        // GET ALL
        // ============================================================
        public List<FxRateListItem> GetAll()
        {
            string sql = @"
                SELECT
                    f.id,
                    f.sending_country_id,  sc.name AS sending_country_name,
                    f.receiving_country_id, rc.name AS receiving_country_name,
                    f.payment_type_id,     pt.name AS payment_type_name,
                    f.agent_id,            a.name  AS agent_name,
                    f.sending_currency,    f.receiving_currency,   f.settlement_currency,
                    f.customer_rate,       f.settlement_rate,      f.cross_rate,
                    f.margin_type,         f.margin_value,
                    f.valid_from,          f.valid_to,
                    f.is_active,           f.created_at,           f.updated_at
                FROM fx_rate_setups f
                JOIN countries sc  ON sc.id = f.sending_country_id
                JOIN countries rc  ON rc.id = f.receiving_country_id
                JOIN payment_types pt ON pt.id = f.payment_type_id
                LEFT JOIN agents a ON a.id = f.agent_id
                ORDER BY f.created_at DESC;";

            return _dbConnection.Query<FxRateListItem>(sql).ToList();
        }

        // ============================================================
        // GET BY ID (with rate change history)
        // ============================================================
        public FxRateDetailResponse? GetById(long id)
        {
            string sqlSetup = @"
                SELECT
                    f.id,
                    f.sending_country_id,  sc.name AS sending_country_name,
                    f.receiving_country_id, rc.name AS receiving_country_name,
                    f.payment_type_id,     pt.name AS payment_type_name,
                    f.agent_id,            a.name  AS agent_name,
                    f.sending_currency,    f.receiving_currency,   f.settlement_currency,
                    f.customer_rate,       f.settlement_rate,      f.cross_rate,
                    f.margin_type,         f.margin_value,
                    f.valid_from,          f.valid_to,
                    f.is_active,           f.created_at,           f.updated_at
                FROM fx_rate_setups f
                JOIN countries sc  ON sc.id = f.sending_country_id
                JOIN countries rc  ON rc.id = f.receiving_country_id
                JOIN payment_types pt ON pt.id = f.payment_type_id
                LEFT JOIN agents a ON a.id = f.agent_id
                WHERE f.id = @Id;";

            var setup = _dbConnection.QuerySingleOrDefault<FxRateDetailResponse>(sqlSetup, new { Id = id });
            if (setup == null) return null;

            string sqlHistory = @"
                SELECT id, previous_customer_rate, new_customer_rate,
                       previous_settlement_rate, new_settlement_rate,
                       previous_cross_rate, new_cross_rate,
                       changed_by, changed_at, reason
                FROM fx_rate_history
                WHERE fx_rate_setup_id = @SetupId
                ORDER BY changed_at DESC;";

            setup.History = _dbConnection.Query<FxRateHistoryItem>(sqlHistory, new { SetupId = id }).ToList();
            return setup;
        }

        // ============================================================
        // UPDATE (with rate history tracking)
        // ============================================================
        public FxRateDetailResponse? Update(long id, UpdateFxRateRequest request)
        {
            // Fetch current record for history comparison
            var current = GetById(id);
            if (current == null) return null;

            string sql = @"
                UPDATE fx_rate_setups
                SET sending_currency    = COALESCE(@SendingCurrency, sending_currency),
                    receiving_currency  = COALESCE(@ReceivingCurrency, receiving_currency),
                    settlement_currency = COALESCE(@SettlementCurrency, settlement_currency),
                    customer_rate       = COALESCE(@CustomerRate, customer_rate),
                    settlement_rate     = COALESCE(@SettlementRate, settlement_rate),
                    cross_rate          = COALESCE(@CrossRate, cross_rate),
                    margin_type         = COALESCE(@MarginType, margin_type),
                    margin_value        = COALESCE(@MarginValue, margin_value),
                    valid_from          = COALESCE(@ValidFrom, valid_from),
                    valid_to            = COALESCE(@ValidTo, valid_to),
                    is_active           = COALESCE(@IsActive, is_active),
                    updated_at          = NOW()
                WHERE id = @Id;";

            _dbConnection.Execute(sql, new
            {
                request.SendingCurrency,
                request.ReceivingCurrency,
                request.SettlementCurrency,
                request.CustomerRate,
                request.SettlementRate,
                request.CrossRate,
                request.MarginType,
                request.MarginValue,
                request.ValidFrom,
                request.ValidTo,
                request.IsActive,
                Id = id
            });

            // Track rate changes in history if any rate actually changed
            bool rateChanged = (request.CustomerRate.HasValue && request.CustomerRate != current.CustomerRate)
                            || (request.SettlementRate.HasValue && request.SettlementRate != current.SettlementRate)
                            || (request.CrossRate.HasValue && request.CrossRate != current.CrossRate);

            if (rateChanged)
            {
                InsertHistory(id,
                    current.CustomerRate,
                    request.CustomerRate ?? current.CustomerRate,
                    current.SettlementRate,
                    request.SettlementRate ?? current.SettlementRate,
                    current.CrossRate,
                    request.CrossRate ?? current.CrossRate,
                    request.UpdatedBy,
                    "Rate updated");
            }

            return GetById(id);
        }

        // ============================================================
        // DELETE
        // ============================================================
        public bool Delete(long id)
        {
            // Delete history first, then setup
            _dbConnection.Execute("DELETE FROM fx_rate_history WHERE fx_rate_setup_id = @Id;", new { Id = id });
            return _dbConnection.Execute("DELETE FROM fx_rate_setups WHERE id = @Id;", new { Id = id }) > 0;
        }

        // ============================================================
        // CONVERT — Calculate converted amounts using active rate
        // ============================================================
        public ConvertResponse? Convert(ConvertRequest request)
        {
            // Find matching rate (agent-specific first, then corridor default)
            string sql = @"
                SELECT id, sending_currency, receiving_currency, settlement_currency,
                       customer_rate, settlement_rate, cross_rate
                FROM fx_rate_setups
                WHERE sending_country_id = @SendingCountryId
                  AND receiving_country_id = @ReceivingCountryId
                  AND payment_type_id = @PaymentTypeId
                  AND is_active = true
                  AND (valid_from IS NULL OR valid_from <= NOW())
                  AND (valid_to IS NULL OR valid_to >= NOW())
                  AND (agent_id = @AgentId OR (agent_id IS NULL AND @AgentId IS NULL))
                ORDER BY agent_id DESC NULLS LAST
                LIMIT 1;";

            var rate = _dbConnection.QuerySingleOrDefault<dynamic>(sql, new
            {
                request.SendingCountryId,
                request.ReceivingCountryId,
                request.PaymentTypeId,
                request.AgentId
            });

            // Fallback: try default (null agent) if agent-specific not found
            if (rate == null && request.AgentId.HasValue)
            {
                rate = _dbConnection.QuerySingleOrDefault<dynamic>(sql, new
                {
                    request.SendingCountryId,
                    request.ReceivingCountryId,
                    request.PaymentTypeId,
                    AgentId = (long?)null
                });
            }

            if (rate == null) return null;

            decimal customerRate = (decimal)rate.customer_rate;
            decimal receiveAmount = Math.Round(request.Amount * customerRate, 2);

            var response = new ConvertResponse
            {
                SendAmount = request.Amount,
                SendingCurrency = (string)rate.sending_currency,
                CustomerRate = customerRate,
                ReceiveAmount = receiveAmount,
                ReceivingCurrency = (string)rate.receiving_currency,
                SettlementCurrency = rate.settlement_currency as string,
                SettlementRate = rate.settlement_rate as decimal?,
                CrossRate = rate.cross_rate as decimal?
            };

            // Calculate settlement amount if settlement currency differs
            if (response.SettlementRate.HasValue && !string.IsNullOrEmpty(response.SettlementCurrency))
            {
                response.SettlementAmount = Math.Round(request.Amount * response.SettlementRate.Value, 2);
            }

            return response;
        }

        // ============================================================
        // HELPER: Insert rate history
        // ============================================================
        private void InsertHistory(long setupId,
            decimal? prevCustomerRate, decimal newCustomerRate,
            decimal? prevSettlementRate, decimal? newSettlementRate,
            decimal? prevCrossRate, decimal? newCrossRate,
            string? changedBy, string? reason)
        {
            string sql = @"
                INSERT INTO fx_rate_history
                    (fx_rate_setup_id, previous_customer_rate, new_customer_rate,
                     previous_settlement_rate, new_settlement_rate,
                     previous_cross_rate, new_cross_rate,
                     changed_by, changed_at, reason)
                VALUES
                    (@SetupId, @PrevCustomerRate, @NewCustomerRate,
                     @PrevSettlementRate, @NewSettlementRate,
                     @PrevCrossRate, @NewCrossRate,
                     @ChangedBy, NOW(), @Reason);";

            _dbConnection.Execute(sql, new
            {
                SetupId = setupId,
                PrevCustomerRate = prevCustomerRate,
                NewCustomerRate = newCustomerRate,
                PrevSettlementRate = prevSettlementRate,
                NewSettlementRate = newSettlementRate,
                PrevCrossRate = prevCrossRate,
                NewCrossRate = newCrossRate,
                ChangedBy = changedBy,
                Reason = reason
            });
        }
    }
}
