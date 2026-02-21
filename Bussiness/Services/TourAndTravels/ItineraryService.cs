using System;
using System.Collections.Generic;
using AutoMapper;
using Domain.Models.TourAndTravels; // Domain models
using Repository.Interfaces.TourAndTravels;
using static Domain.Models.TourAndTravels.Itinerary;

namespace Bussiness.Services.TourAndTravels
{
    public interface IItineraryService
    {
        // Template CRUD
        ItineraryResponse CreateItinerary(CreateItineraryRequest request);
        List<ItineraryResponse> GetAllItineraries();
        ItineraryDetailResponse? GetItineraryById(long id);
        ItineraryResponse? UpdateItinerary(long id, UpdateItineraryRequest request);
        bool DeleteItinerary(long id);

        // Days
        ItineraryDayResponse? AddDayToItinerary(long itineraryId, CreateItineraryDayRequest request);
        ItineraryDayResponse? UpdateDay(long itineraryId, long dayId, UpdateItineraryDayRequest request);
    }

    public class ItineraryService : IItineraryService
    {
        private readonly IItineraryRepository _repository;
        private readonly IMapper _mapper;

        public ItineraryService(IItineraryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // ----------------------------
        // Template CRUD
        // ----------------------------
        public ItineraryResponse CreateItinerary(CreateItineraryRequest request)
        {
            // Map domain model → repository DTO
            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.ItineraryDTO.CreateItineraryRequest>(request);
            var repoResponse = _repository.CreateItinerary(repoRequest);

            // Map repository response → domain model
            return _mapper.Map<ItineraryResponse>(repoResponse);
        }

        public List<ItineraryResponse> GetAllItineraries()
        {
            var repoResponses = _repository.GetAllItineraries();
            return _mapper.Map<List<ItineraryResponse>>(repoResponses);
        }

        public ItineraryDetailResponse? GetItineraryById(long id)
        {
            var repoResponse = _repository.GetItineraryById(id);
            return repoResponse == null ? null : _mapper.Map<ItineraryDetailResponse>(repoResponse);
        }

        public ItineraryResponse? UpdateItinerary(long id, UpdateItineraryRequest request)
        {
            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.ItineraryDTO.UpdateItineraryRequest>(request);
            var repoResponse = _repository.UpdateItinerary(id, repoRequest);
            return repoResponse == null ? null : _mapper.Map<ItineraryResponse>(repoResponse);
        }

        public bool DeleteItinerary(long id)
        {
            return _repository.DeleteItinerary(id);
        }

        // ----------------------------
        // Days
        // ----------------------------
        public ItineraryDayResponse? AddDayToItinerary(long itineraryId, CreateItineraryDayRequest request)
        {
            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.ItineraryDTO.CreateItineraryDayRequest>(request);
            var repoResponse = _repository.AddDayToItinerary(itineraryId, repoRequest);
            return repoResponse == null ? null : _mapper.Map<ItineraryDayResponse>(repoResponse);
        }

        public ItineraryDayResponse? UpdateDay(long itineraryId, long dayId, UpdateItineraryDayRequest request)
        {
            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.ItineraryDTO.UpdateItineraryDayRequest>(request);
            var repoResponse = _repository.UpdateDay(itineraryId, dayId, repoRequest);
            return repoResponse == null ? null : _mapper.Map<ItineraryDayResponse>(repoResponse);
        }
    }
}