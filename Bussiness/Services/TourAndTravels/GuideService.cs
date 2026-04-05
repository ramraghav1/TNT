using System.Collections.Generic;
using Domain.Models.TourAndTravels;
using Repository.Interfaces.TourAndTravels;

namespace Bussiness.Services.TourAndTravels
{
    public interface IGuideService
    {
        Inventory.GuideResponse CreateGuide(Inventory.CreateGuideRequest request, long userId);
        List<Inventory.GuideResponse> GetAllGuides(bool includeInactive = false);
        Inventory.GuideResponse? GetGuideById(long id);
        Inventory.GuideResponse? UpdateGuide(long id, Inventory.UpdateGuideRequest request);
        bool DeleteGuide(long id);
        bool ActivateGuide(long id);
    }

    public class GuideService : IGuideService
    {
        private readonly IGuideRepository _repository;
        private readonly INotificationService _notificationService;

        public GuideService(IGuideRepository repository, INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        public Inventory.GuideResponse CreateGuide(Inventory.CreateGuideRequest request, long userId)
        {
            var result = _repository.CreateGuide(request, userId);

            // Push real-time notification
            _ = _notificationService.CreateAndBroadcastAsync(new Domain.Models.CreateNotification
            {
                Type = "inventory",
                Title = "New Guide Added",
                Message = $"Guide '{request.FullName}' has been added to inventory",
                Link = "/inventory/guides",
                Icon = "pi-user"
            });

            return result;
        }

        public List<Inventory.GuideResponse> GetAllGuides(bool includeInactive = false)
        {
            return _repository.GetAllGuides(includeInactive);
        }

        public Inventory.GuideResponse? GetGuideById(long id)
        {
            return _repository.GetGuideById(id);
        }

        public Inventory.GuideResponse? UpdateGuide(long id, Inventory.UpdateGuideRequest request)
        {
            return _repository.UpdateGuide(id, request);
        }

        public bool DeleteGuide(long id)
        {
            return _repository.DeleteGuide(id);
        }

        public bool ActivateGuide(long id)
        {
            return _repository.ActivateGuide(id);
        }
    }
}
