using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Clinic;
using static Repository.DataModels.Clinic.InvoiceDTO;

namespace Repository.Repositories.Clinic
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IDbConnection _dbConnection;

        public InvoiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public InvoiceResponse Create(CreateInvoiceRequest request)
        {
            string sql = @"
                INSERT INTO invoices (tenant_id, invoice_number, patient_id, appointment_id, amount, tax, discount, total, currency, status, payment_method, notes, created_at)
                VALUES (@TenantId, @InvoiceNumber, @PatientId, @AppointmentId, @Amount, @Tax, @Discount, @Total, @Currency, @Status, @PaymentMethod, @Notes, NOW())
                RETURNING id;";

            long id = _dbConnection.QuerySingle<long>(sql, request);
            return GetById(request.TenantId, id)!;
        }

        public List<InvoiceResponse> GetByTenant(long tenantId)
        {
            string sql = BaseSelectSql() + @"
                WHERE i.tenant_id = @TenantId
                ORDER BY i.created_at DESC;";
            return _dbConnection.Query<InvoiceResponse>(sql, new { TenantId = tenantId }).ToList();
        }

        public List<InvoiceResponse> GetByPatient(long tenantId, long patientId)
        {
            string sql = BaseSelectSql() + @"
                WHERE i.tenant_id = @TenantId AND i.patient_id = @PatientId
                ORDER BY i.created_at DESC;";
            return _dbConnection.Query<InvoiceResponse>(sql, new { TenantId = tenantId, PatientId = patientId }).ToList();
        }

        public InvoiceResponse? GetById(long tenantId, long id)
        {
            string sql = BaseSelectSql() + @"
                WHERE i.id = @Id AND i.tenant_id = @TenantId;";
            return _dbConnection.QuerySingleOrDefault<InvoiceResponse>(sql, new { Id = id, TenantId = tenantId });
        }

        public InvoiceResponse? Update(long tenantId, long id, UpdateInvoiceRequest request)
        {
            string sql = @"
                UPDATE invoices
                SET amount = COALESCE(@Amount, amount),
                    tax = COALESCE(@Tax, tax),
                    discount = COALESCE(@Discount, discount),
                    total = COALESCE(@Total, total),
                    status = COALESCE(@Status, status),
                    payment_method = COALESCE(@PaymentMethod, payment_method),
                    paid_at = COALESCE(@PaidAt, paid_at),
                    notes = COALESCE(@Notes, notes),
                    updated_at = NOW()
                WHERE id = @Id AND tenant_id = @TenantId;";

            int affected = _dbConnection.Execute(sql, new
            {
                request.Amount,
                request.Tax,
                request.Discount,
                request.Total,
                request.Status,
                request.PaymentMethod,
                request.PaidAt,
                request.Notes,
                Id = id,
                TenantId = tenantId
            });

            return affected > 0 ? GetById(tenantId, id) : null;
        }

        public bool Delete(long tenantId, long id)
        {
            string sql = "DELETE FROM invoices WHERE id = @Id AND tenant_id = @TenantId;";
            return _dbConnection.Execute(sql, new { Id = id, TenantId = tenantId }) > 0;
        }

        private static string BaseSelectSql()
        {
            return @"
                SELECT i.id, i.tenant_id, i.invoice_number, i.patient_id,
                       CONCAT(p.first_name, ' ', p.last_name) AS patient_name,
                       i.appointment_id, i.amount, i.tax, i.discount, i.total, i.currency,
                       i.status, i.payment_method, i.paid_at, i.notes, i.created_at, i.updated_at
                FROM invoices i
                JOIN patients p ON p.id = i.patient_id";
        }
    }
}
