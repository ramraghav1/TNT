using System.Collections.Generic;
using static Repository.DataModels.Remittance.ServiceChargeSetupDTO;

namespace Repository.Interfaces.Remittance
{
    public interface IServiceChargeSetupRepository
    {
        SetupDetailResponse Create(CreateSetupRequest request);
        List<SetupListItem> GetAll();
        SetupDetailResponse? GetById(long id);
        SetupDetailResponse? Update(long id, UpdateSetupRequest request);
        bool Delete(long id);
        CalculateChargeResponse? CalculateCharge(CalculateChargeRequest request);
    }
}
