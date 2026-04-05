using System.Collections.Generic;
using Domain.Models.TourAndTravels;
using Repository.Interfaces.TourAndTravels;

namespace Bussiness.Services.TourAndTravels
{
    public interface IActivityService
    {
        Inventory.ActivityResponse CreateActivity(Inventory.CreateActivityRequest request, long userId);
        List<Inventory.ActivityResponse> GetAllActivities(bool includeInactive = false);
        Inventory.ActivityResponse? GetActivityById(long id);
        Inventory.ActivityResponse? UpdateActivity(long id, Inventory.UpdateActivityRequest request);
        bool DeleteActivity(long id);
        bool ActivateActivity(long id);
    }

    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _repository;
        private readonly INotificationService _notificationService;

        public ActivityService(IActivityRepository repository, INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        public Inventory.ActivityResponse CreateActivity(Inventory.CreateActivityRequest request, long userId)
        {
            var result = _repository.CreateActivity(request, userId);

            // Push real-time notification
            _ = _notificationService.CreateAndBroadcastAsync(new Domain.Models.CreateNotification
            {
                Type = "inventory",
                Title = "New Activity Added",
                Message = $"Activity '{request.Name}' has been added to inventory",
                Link = "/inventory/activities",
                Icon = "pi-map-marker"
            });

            return result;
        }

        public List<Inventory.ActivityResponse> GetAllActivities(bool includeInactive = false)
        {
            return _repository.GetAllActivities(includeInactive);
        }

        public Inventory.ActivityResponse? GetActivityById(long id)
        {
            return _repository.GetActivityById(id);
        }

        public Inventory.ActivityResponse? UpdateActivity(long id, Inventory.UpdateActivityRequest request)
        {
            return _repository.UpdateActivity(id, request);
        }

        public bool DeleteActivity(long id)
        {
            return _repository.DeleteActivity(id);
        }

        public bool ActivateActivity(long id)
        {
            return _repository.ActivateActivity(id);
        }
    }
}
