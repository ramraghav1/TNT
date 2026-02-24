using System.Collections.Generic;
using static Repository.DataModels.Remittance.PaymentTypeDTO;

namespace Repository.Interfaces.Remittance
{
    public interface IPaymentTypeRepository
    {
        PaymentTypeResponse Create(CreatePaymentTypeRequest request);
        List<PaymentTypeResponse> GetAll();
        PaymentTypeResponse? GetById(long id);
        PaymentTypeResponse? Update(long id, UpdatePaymentTypeRequest request);
        bool Delete(long id);
    }
}
