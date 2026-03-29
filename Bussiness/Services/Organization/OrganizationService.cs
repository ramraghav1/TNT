using System;
using AutoMapper;
using Domain.Models.Organization;
using Domain.Models.Transaction;
using Repository.DataModels.Organization;
using Repository.DataModels.Transaction;
using Repository.Interfaces;

namespace Bussiness.Services.Organization
{
	public interface IOrganizationService
	{
		Task<long> InsertOrganization(CreateOrganization objCreateOrganization);

		Task<List<OrganizationDetail>> GetDetails();

		Task<SetupOrganizationResponse> SetupOrganization(SetupOrganizationRequest request);
    }
	public class OrganizationService: IOrganizationService
    {
		private readonly IMapper _mapper;
		private readonly IOrganizationRepository _orgRepository;
		public OrganizationService(IMapper mapper, IOrganizationRepository organizationRepository)
		{
			_mapper = mapper;
			_orgRepository = organizationRepository;
		}
		public async Task<long> InsertOrganization(CreateOrganization objCreateOrganization)
		{
             var dto = _mapper.Map<CreateOrganizationDTO>(objCreateOrganization);
             long  result =  await _orgRepository.InsertAsync(dto);

			return result;
        }
        public async Task<List<OrganizationDetail>> GetDetails()
        {

            var itemList = await _orgRepository.ListAsync();
            var result = _mapper.Map<List<OrganizationDetail>>(itemList);
            return result;
        }

        public async Task<SetupOrganizationResponse> SetupOrganization(SetupOrganizationRequest request)
        {
            var dto = _mapper.Map<SetupOrganizationDTO>(request);
            var resultDto = await _orgRepository.SetupOrganizationWithManagerAsync(dto);
            return _mapper.Map<SetupOrganizationResponse>(resultDto);
        }
    }
}

