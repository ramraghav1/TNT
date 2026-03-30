using AutoMapper;
using Domain.Models;
using Repository.DataModels;
using Repository.Interfaces;

namespace Bussiness.Services
{
    public interface IDemoRequestService
    {
        Task<long> SubmitDemoRequest(DemoRequest request);
        Task<IEnumerable<DemoRequestDTO>> GetAllDemoRequests();
    }

    public class DemoRequestService : IDemoRequestService
    {
        private readonly IDemoRequestRepository _demoRequestRepo;
        private readonly IMapper _mapper;

        public DemoRequestService(IDemoRequestRepository demoRequestRepo, IMapper mapper)
        {
            _demoRequestRepo = demoRequestRepo;
            _mapper = mapper;
        }

        public async Task<long> SubmitDemoRequest(DemoRequest request)
        {
            var dto = _mapper.Map<DemoRequestDTO>(request);
            return await _demoRequestRepo.InsertAsync(dto);
        }

        public async Task<IEnumerable<DemoRequestDTO>> GetAllDemoRequests()
        {
            return await _demoRequestRepo.ListAsync();
        }
    }
}
