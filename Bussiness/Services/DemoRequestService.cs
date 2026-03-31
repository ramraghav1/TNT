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
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;

        public DemoRequestService(IDemoRequestRepository demoRequestRepo, IMapper mapper, IEmailService emailService, INotificationService notificationService)
        {
            _demoRequestRepo = demoRequestRepo;
            _mapper = mapper;
            _emailService = emailService;
            _notificationService = notificationService;
        }

        public async Task<long> SubmitDemoRequest(DemoRequest request)
        {
            // Check for duplicate: same email + same product
            var exists = await _demoRequestRepo.ExistsByEmailAndProductAsync(request.Email, request.ProductInterest);
            if (exists)
            {
                throw new InvalidOperationException($"A demo request for '{request.ProductInterest}' has already been submitted with this email.");
            }

            var dto = _mapper.Map<DemoRequestDTO>(request);
            var id = await _demoRequestRepo.InsertAsync(dto);

            // Send email notification
            await _emailService.SendDemoRequestNotificationAsync(
                request.FullName,
                request.Email,
                request.Phone,
                request.CompanyName,
                request.ProductInterest,
                request.Message);

            // Push real-time notification
            await _notificationService.CreateAndBroadcastAsync(new Domain.Models.CreateNotification
            {
                Type = "demo_request",
                Title = "New Demo Request",
                Message = $"{request.FullName} requested a demo for {request.ProductInterest}",
                Link = "/demo-requests",
                Icon = "pi-envelope"
            });

            return id;
        }

        public async Task<IEnumerable<DemoRequestDTO>> GetAllDemoRequests()
        {
            return await _demoRequestRepo.ListAsync();
        }
    }
}
