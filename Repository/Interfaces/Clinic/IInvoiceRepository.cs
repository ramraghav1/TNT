using System.Collections.Generic;
using static Repository.DataModels.Clinic.InvoiceDTO;

namespace Repository.Interfaces.Clinic
{
    public interface IInvoiceRepository
    {
        InvoiceResponse Create(CreateInvoiceRequest request);
        List<InvoiceResponse> GetByTenant(long tenantId);
        List<InvoiceResponse> GetByPatient(long tenantId, long patientId);
        InvoiceResponse? GetById(long tenantId, long id);
        InvoiceResponse? Update(long tenantId, long id, UpdateInvoiceRequest request);
        bool Delete(long tenantId, long id);
    }
}
