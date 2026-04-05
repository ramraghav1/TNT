using System.Collections.Generic;
using Domain.Models.TourAndTravels;
using Repository.Interfaces.TourAndTravels;

namespace Bussiness.Services.TourAndTravels
{
    public interface IVehicleService
    {
        Inventory.VehicleResponse CreateVehicle(Inventory.CreateVehicleRequest request, long userId);
        List<Inventory.VehicleResponse> GetAllVehicles(bool includeInactive = false);
        Inventory.VehicleResponse? GetVehicleById(long id);
        Inventory.VehicleResponse? UpdateVehicle(long id, Inventory.UpdateVehicleRequest request);
        bool DeleteVehicle(long id);
        bool ActivateVehicle(long id);
    }

    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _repository;
        private readonly INotificationService _notificationService;

        public VehicleService(IVehicleRepository repository, INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        public Inventory.VehicleResponse CreateVehicle(Inventory.CreateVehicleRequest request, long userId)
        {
            var result = _repository.CreateVehicle(request, userId);

            // Push real-time notification
            _ = _notificationService.CreateAndBroadcastAsync(new Domain.Models.CreateNotification
            {
                Type = "inventory",
                Title = "New Vehicle Added",
                Message = $"Vehicle '{request.Model}' ({request.RegistrationNumber}) has been added to inventory",
                Link = "/inventory/vehicles",
                Icon = "pi-car"
            });

            return result;
        }

        public List<Inventory.VehicleResponse> GetAllVehicles(bool includeInactive = false)
        {
            return _repository.GetAllVehicles(includeInactive);
        }

        public Inventory.VehicleResponse? GetVehicleById(long id)
        {
            return _repository.GetVehicleById(id);
        }

        public Inventory.VehicleResponse? UpdateVehicle(long id, Inventory.UpdateVehicleRequest request)
        {
            return _repository.UpdateVehicle(id, request);
        }

        public bool DeleteVehicle(long id)
        {
            return _repository.DeleteVehicle(id);
        }

        public bool ActivateVehicle(long id)
        {
            return _repository.ActivateVehicle(id);
        }
    }
}
