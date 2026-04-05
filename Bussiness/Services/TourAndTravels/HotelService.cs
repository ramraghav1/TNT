using System.Collections.Generic;
using Domain.Models.TourAndTravels;
using Repository.Interfaces.TourAndTravels;

namespace Bussiness.Services.TourAndTravels
{
    public interface IHotelService
    {
        Inventory.HotelResponse CreateHotel(Inventory.CreateHotelRequest request, long userId);
        List<Inventory.HotelResponse> GetAllHotels(bool includeInactive = false);
        Inventory.HotelResponse? GetHotelById(long id);
        Inventory.HotelResponse? UpdateHotel(long id, Inventory.UpdateHotelRequest request);
        bool DeleteHotel(long id);
        bool ActivateHotel(long id);
    }

    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _repository;
        private readonly INotificationService _notificationService;

        public HotelService(IHotelRepository repository, INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        public Inventory.HotelResponse CreateHotel(Inventory.CreateHotelRequest request, long userId)
        {
            var result = _repository.CreateHotel(request, userId);

            // Push real-time notification
            _ = _notificationService.CreateAndBroadcastAsync(new Domain.Models.CreateNotification
            {
                Type = "inventory",
                Title = "New Hotel Added",
                Message = $"Hotel '{request.Name}' has been added to inventory",
                Link = "/inventory/hotels",
                Icon = "pi-building"
            });

            return result;
        }

        public List<Inventory.HotelResponse> GetAllHotels(bool includeInactive = false)
        {
            return _repository.GetAllHotels(includeInactive);
        }

        public Inventory.HotelResponse? GetHotelById(long id)
        {
            return _repository.GetHotelById(id);
        }

        public Inventory.HotelResponse? UpdateHotel(long id, Inventory.UpdateHotelRequest request)
        {
            return _repository.UpdateHotel(id, request);
        }

        public bool DeleteHotel(long id)
        {
            return _repository.DeleteHotel(id);
        }

        public bool ActivateHotel(long id)
        {
            return _repository.ActivateHotel(id);
        }
    }
}
