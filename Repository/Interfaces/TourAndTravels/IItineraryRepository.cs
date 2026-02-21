using System;
using System.Collections.Generic;
using static Repository.DataModels.TourAndTravels.ItineraryDTO;

namespace Repository.Interfaces.TourAndTravels
{
    public interface IItineraryRepository
    {
        ItineraryResponse CreateItinerary(CreateItineraryRequest request);
        List<ItineraryResponse> GetAllItineraries();
        ItineraryDetailResponse? GetItineraryById(long id);
        ItineraryResponse? UpdateItinerary(long id, UpdateItineraryRequest request);
        bool DeleteItinerary(long id);
        ItineraryDayResponse? AddDayToItinerary(long itineraryId, CreateItineraryDayRequest request);
        ItineraryDayResponse? UpdateDay(long itineraryId, long dayId, UpdateItineraryDayRequest request);
    }
}