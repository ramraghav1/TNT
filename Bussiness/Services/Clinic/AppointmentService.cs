using System;
using AutoMapper;
using Repository.Interfaces.Clinic;
using static Domain.Models.Clinic.Appointment;

namespace Bussiness.Services.Clinic
{
    public interface IAppointmentService
    {
        AppointmentResponse Create(CreateAppointmentRequest request);
        List<AppointmentResponse> GetByTenant(long tenantId);
        List<AppointmentResponse> GetByDateRange(long tenantId, DateOnly from, DateOnly to);
        List<AppointmentResponse> GetByPractitioner(long tenantId, long practitionerId, DateOnly? date = null);
        List<AppointmentResponse> GetByPatient(long tenantId, long patientId);
        AppointmentResponse? GetById(long tenantId, long id);
        AppointmentResponse? Update(long tenantId, long id, UpdateAppointmentRequest request);
        bool Delete(long tenantId, long id);
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;
        private readonly IMapper _mapper;

        public AppointmentService(IAppointmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public AppointmentResponse Create(CreateAppointmentRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Clinic.AppointmentDTO.CreateAppointmentRequest>(request);
            var result = _repository.Create(dto);
            return _mapper.Map<AppointmentResponse>(result);
        }

        public List<AppointmentResponse> GetByTenant(long tenantId)
        {
            var result = _repository.GetByTenant(tenantId);
            return _mapper.Map<List<AppointmentResponse>>(result);
        }

        public List<AppointmentResponse> GetByDateRange(long tenantId, DateOnly from, DateOnly to)
        {
            var result = _repository.GetByDateRange(tenantId, from, to);
            return _mapper.Map<List<AppointmentResponse>>(result);
        }

        public List<AppointmentResponse> GetByPractitioner(long tenantId, long practitionerId, DateOnly? date = null)
        {
            var result = _repository.GetByPractitioner(tenantId, practitionerId, date);
            return _mapper.Map<List<AppointmentResponse>>(result);
        }

        public List<AppointmentResponse> GetByPatient(long tenantId, long patientId)
        {
            var result = _repository.GetByPatient(tenantId, patientId);
            return _mapper.Map<List<AppointmentResponse>>(result);
        }

        public AppointmentResponse? GetById(long tenantId, long id)
        {
            var result = _repository.GetById(tenantId, id);
            return result == null ? null : _mapper.Map<AppointmentResponse>(result);
        }

        public AppointmentResponse? Update(long tenantId, long id, UpdateAppointmentRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Clinic.AppointmentDTO.UpdateAppointmentRequest>(request);
            var result = _repository.Update(tenantId, id, dto);
            return result == null ? null : _mapper.Map<AppointmentResponse>(result);
        }

        public bool Delete(long tenantId, long id)
        {
            return _repository.Delete(tenantId, id);
        }
    }
}
