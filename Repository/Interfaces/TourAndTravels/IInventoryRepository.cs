using System;
using System.Collections.Generic;
using Domain.Models.TourAndTravels;

namespace Repository.Interfaces.TourAndTravels
{
    public interface IHotelRepository
    {
        Inventory.HotelResponse CreateHotel(Inventory.CreateHotelRequest request, long userId);
        List<Inventory.HotelResponse> GetAllHotels(bool includeInactive = false);
        Inventory.HotelResponse? GetHotelById(long id);
        Inventory.HotelResponse? UpdateHotel(long id, Inventory.UpdateHotelRequest request);
        bool DeleteHotel(long id);
        bool ActivateHotel(long id);
    }

    public interface IVehicleRepository
    {
        Inventory.VehicleResponse CreateVehicle(Inventory.CreateVehicleRequest request, long userId);
        List<Inventory.VehicleResponse> GetAllVehicles(bool includeInactive = false);
        Inventory.VehicleResponse? GetVehicleById(long id);
        Inventory.VehicleResponse? UpdateVehicle(long id, Inventory.UpdateVehicleRequest request);
        bool DeleteVehicle(long id);
        bool ActivateVehicle(long id);
    }

    public interface IGuideRepository
    {
        Inventory.GuideResponse CreateGuide(Inventory.CreateGuideRequest request, long userId);
        List<Inventory.GuideResponse> GetAllGuides(bool includeInactive = false);
        Inventory.GuideResponse? GetGuideById(long id);
        Inventory.GuideResponse? UpdateGuide(long id, Inventory.UpdateGuideRequest request);
        bool DeleteGuide(long id);
        bool ActivateGuide(long id);
    }

    public interface IActivityRepository
    {
        Inventory.ActivityResponse CreateActivity(Inventory.CreateActivityRequest request, long userId);
        List<Inventory.ActivityResponse> GetAllActivities(bool includeInactive = false);
        Inventory.ActivityResponse? GetActivityById(long id);
        Inventory.ActivityResponse? UpdateActivity(long id, Inventory.UpdateActivityRequest request);
        bool DeleteActivity(long id);
        bool ActivateActivity(long id);
    }
}
