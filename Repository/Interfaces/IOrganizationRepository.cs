using System;
using Repository.DataModels.Organization;

namespace Repository.Interfaces
{
	public interface IOrganizationRepository
	{
         Task<long> InsertAsync(CreateOrganizationDTO entity);
         Task<int> UpdateAsync(CreateOrganizationDTO entity);
         Task<int> DeleteAsync(int organizationId);
         Task<IEnumerable<OrganizationDetailDTO>> ListAsync();
    }
}

