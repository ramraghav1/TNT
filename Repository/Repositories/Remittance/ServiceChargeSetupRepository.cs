using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Remittance;
using static Repository.DataModels.Remittance.ServiceChargeSetupDTO;

namespace Repository.Repositories.Remittance
{
    public class ServiceChargeSetupRepository : IServiceChargeSetupRepository
    {
        private readonly IDbConnection _dbConnection;

        public ServiceChargeSetupRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // ============================================================
        // CREATE SETUP + SLABS
        // ============================================================
        public SetupDetailResponse Create(CreateSetupRequest request)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                string sqlSetup = @"
                    INSERT INTO service_charge_setups
                    (sending_country_id, receiving_country_id, payment_type_id, agent_id,
                     charge_mode, is_active, created_at)
                    VALUES
                    (@SendingCountryId, @ReceivingCountryId, @PaymentTypeId, @AgentId,
                     @ChargeMode, true, NOW())
                    RETURNING id;";

                long setupId = _dbConnection.QuerySingle<long>(sqlSetup, new
                {
                    request.SendingCountryId,
                    request.ReceivingCountryId,
                    request.PaymentTypeId,
                    request.AgentId,
                    request.ChargeMode
                }, transaction);

                InsertSlabs(setupId, request.Slabs, transaction);

                transaction.Commit();
                return GetById(setupId)!;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // ============================================================
        // GET ALL (list view)
        // ============================================================
        public List<SetupListItem> GetAll()
        {
            string sql = @"
                SELECT
                    s.id,
                    s.sending_country_id,
                    sc.name AS sending_country_name,
                    s.receiving_country_id,
                    rc.name AS receiving_country_name,
                    s.payment_type_id,
                    pt.name AS payment_type_name,
                    s.agent_id,
                    a.name  AS agent_name,
                    s.charge_mode,
                    s.is_active,
                    s.created_at,
                    s.updated_at
                FROM service_charge_setups s
                JOIN countries sc ON sc.id = s.sending_country_id
                JOIN countries rc ON rc.id = s.receiving_country_id
                JOIN payment_types pt ON pt.id = s.payment_type_id
                LEFT JOIN agents a ON a.id = s.agent_id
                ORDER BY s.created_at DESC;";

            return _dbConnection.Query<SetupListItem>(sql).ToList();
        }

        // ============================================================
        // GET BY ID (detail with slabs)
        // ============================================================
        public SetupDetailResponse? GetById(long id)
        {
            string sqlSetup = @"
                SELECT
                    s.id,
                    s.sending_country_id,
                    sc.name AS sending_country_name,
                    s.receiving_country_id,
                    rc.name AS receiving_country_name,
                    s.payment_type_id,
                    pt.name AS payment_type_name,
                    s.agent_id,
                    a.name  AS agent_name,
                    s.charge_mode,
                    s.is_active,
                    s.created_at,
                    s.updated_at
                FROM service_charge_setups s
                JOIN countries sc ON sc.id = s.sending_country_id
                JOIN countries rc ON rc.id = s.receiving_country_id
                JOIN payment_types pt ON pt.id = s.payment_type_id
                LEFT JOIN agents a ON a.id = s.agent_id
                WHERE s.id = @Id;";

            var setup = _dbConnection.QuerySingleOrDefault<SetupDetailResponse>(sqlSetup, new { Id = id });
            if (setup == null) return null;

            string sqlSlabs = @"
                SELECT id, min_amount, max_amount, charge_type, charge_value, currency
                FROM service_charge_slabs
                WHERE setup_id = @SetupId
                ORDER BY min_amount;";

            setup.Slabs = _dbConnection.Query<SlabResponse>(sqlSlabs, new { SetupId = id }).ToList();
            return setup;
        }

        // ============================================================
        // UPDATE (header + replace slabs)
        // ============================================================
        public SetupDetailResponse? Update(long id, UpdateSetupRequest request)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                string sql = @"
                    UPDATE service_charge_setups
                    SET charge_mode = COALESCE(@ChargeMode, charge_mode),
                        is_active   = COALESCE(@IsActive, is_active),
                        updated_at  = NOW()
                    WHERE id = @Id;";

                int affected = _dbConnection.Execute(sql, new
                {
                    request.ChargeMode,
                    request.IsActive,
                    Id = id
                }, transaction);

                if (affected == 0)
                {
                    transaction.Rollback();
                    return null;
                }

                // Replace slabs if provided
                if (request.Slabs != null)
                {
                    _dbConnection.Execute("DELETE FROM service_charge_slabs WHERE setup_id = @Id;", new { Id = id }, transaction);
                    InsertSlabs(id, request.Slabs, transaction);
                }

                transaction.Commit();
                return GetById(id);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // ============================================================
        // DELETE
        // ============================================================
        public bool Delete(long id)
        {
            // Cascade deletes slabs via FK
            string sql = "DELETE FROM service_charge_setups WHERE id = @Id;";
            return _dbConnection.Execute(sql, new { Id = id }) > 0;
        }

        // ============================================================
        // CALCULATE CHARGE for a given amount
        // ============================================================
        public CalculateChargeResponse? CalculateCharge(CalculateChargeRequest request)
        {
            // Find matching setup (agent-specific first, then default/null agent)
            string sqlSetup = @"
                SELECT id, charge_mode
                FROM service_charge_setups
                WHERE sending_country_id = @SendingCountryId
                  AND receiving_country_id = @ReceivingCountryId
                  AND payment_type_id = @PaymentTypeId
                  AND is_active = true
                  AND (agent_id = @AgentId OR (agent_id IS NULL AND @AgentId IS NULL))
                ORDER BY agent_id DESC NULLS LAST
                LIMIT 1;";

            var setup = _dbConnection.QuerySingleOrDefault<dynamic>(sqlSetup, new
            {
                request.SendingCountryId,
                request.ReceivingCountryId,
                request.PaymentTypeId,
                request.AgentId
            });

            // If no agent-specific setup found, try default (null agent)
            if (setup == null && request.AgentId.HasValue)
            {
                setup = _dbConnection.QuerySingleOrDefault<dynamic>(sqlSetup, new
                {
                    request.SendingCountryId,
                    request.ReceivingCountryId,
                    request.PaymentTypeId,
                    AgentId = (long?)null
                });
            }

            if (setup == null) return null;

            long setupId = (long)setup.id;

            // Find matching slab for the amount
            string sqlSlab = @"
                SELECT charge_type, charge_value, currency
                FROM service_charge_slabs
                WHERE setup_id = @SetupId
                  AND min_amount <= @Amount
                  AND max_amount >= @Amount
                LIMIT 1;";

            var slab = _dbConnection.QuerySingleOrDefault<dynamic>(sqlSlab, new
            {
                SetupId = setupId,
                Amount = request.Amount
            });

            if (slab == null) return null;

            string chargeType = (string)slab.charge_type;
            decimal chargeValue = (decimal)slab.charge_value;
            string currency = (string)slab.currency;

            decimal serviceCharge = chargeType == "Percentage"
                ? Math.Round(request.Amount * chargeValue / 100m, 2)
                : chargeValue;

            return new CalculateChargeResponse
            {
                SendAmount = request.Amount,
                ServiceCharge = serviceCharge,
                ChargeType = chargeType,
                ChargeValue = chargeValue,
                TotalDeducted = request.Amount + serviceCharge,
                Currency = currency
            };
        }

        // ============================================================
        // HELPER: Insert slabs
        // ============================================================
        private void InsertSlabs(long setupId, List<SlabRequest> slabs, IDbTransaction transaction)
        {
            foreach (var slab in slabs)
            {
                string sqlSlab = @"
                    INSERT INTO service_charge_slabs
                    (setup_id, min_amount, max_amount, charge_type, charge_value, currency, created_at)
                    VALUES
                    (@SetupId, @MinAmount, @MaxAmount, @ChargeType, @ChargeValue, @Currency, NOW());";

                _dbConnection.Execute(sqlSlab, new
                {
                    SetupId = setupId,
                    slab.MinAmount,
                    slab.MaxAmount,
                    slab.ChargeType,
                    slab.ChargeValue,
                    slab.Currency
                }, transaction);
            }
        }
    }
}
