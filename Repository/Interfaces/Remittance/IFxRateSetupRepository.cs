using System.Collections.Generic;
using static Repository.DataModels.Remittance.FxRateSetupDTO;

namespace Repository.Interfaces.Remittance
{
    public interface IFxRateSetupRepository
    {
        FxRateDetailResponse Create(CreateFxRateRequest request);
        List<FxRateListItem> GetAll();
        FxRateDetailResponse? GetById(long id);
        FxRateDetailResponse? Update(long id, UpdateFxRateRequest request);
        bool Delete(long id);
        ConvertResponse? Convert(ConvertRequest request);
    }
}
