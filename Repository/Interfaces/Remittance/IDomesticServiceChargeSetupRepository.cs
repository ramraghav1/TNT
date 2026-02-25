using System.Collections.Generic;
using static Repository.DataModels.Remittance.DomesticServiceChargeSetupDTO;

namespace Repository.Interfaces.Remittance
{
    public interface IDomesticServiceChargeSetupRepository
    {
        SetupDetailResponse Create(CreateSetupRequest request);
        List<SetupListItem> GetAll();
        SetupDetailResponse? GetById(long id);
        SetupDetailResponse? Update(long id, UpdateSetupRequest request);
        bool Delete(long id);
        CalculateChargeResponse? CalculateCharge(CalculateChargeRequest request);
    }
}
