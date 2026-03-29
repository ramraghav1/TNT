using AutoMapper;
using Domain.Models.Clinic;
using Repository.DataModels.Clinic;

namespace Bussiness.MappingProfiles.Clinic
{
    public class ClinicMappingProfile : Profile
    {
        public ClinicMappingProfile()
        {
            // Tenant
            CreateMap<Tenant.CreateTenantRequest, TenantDTO.CreateTenantRequest>().ReverseMap();
            CreateMap<Tenant.UpdateTenantRequest, TenantDTO.UpdateTenantRequest>().ReverseMap();
            CreateMap<TenantDTO.TenantResponse, Tenant.TenantResponse>().ReverseMap();

            // Practitioner
            CreateMap<Practitioner.CreatePractitionerRequest, PractitionerDTO.CreatePractitionerRequest>().ReverseMap();
            CreateMap<Practitioner.UpdatePractitionerRequest, PractitionerDTO.UpdatePractitionerRequest>().ReverseMap();
            CreateMap<PractitionerDTO.PractitionerResponse, Practitioner.PractitionerResponse>().ReverseMap();

            // Patient
            CreateMap<Patient.CreatePatientRequest, PatientDTO.CreatePatientRequest>().ReverseMap();
            CreateMap<Patient.UpdatePatientRequest, PatientDTO.UpdatePatientRequest>().ReverseMap();
            CreateMap<PatientDTO.PatientResponse, Patient.PatientResponse>().ReverseMap();

            // ClinicService
            CreateMap<ClinicService.CreateClinicServiceRequest, ClinicServiceDTO.CreateClinicServiceRequest>().ReverseMap();
            CreateMap<ClinicService.UpdateClinicServiceRequest, ClinicServiceDTO.UpdateClinicServiceRequest>().ReverseMap();
            CreateMap<ClinicServiceDTO.ClinicServiceResponse, ClinicService.ClinicServiceResponse>().ReverseMap();

            // Appointment
            CreateMap<Appointment.CreateAppointmentRequest, AppointmentDTO.CreateAppointmentRequest>().ReverseMap();
            CreateMap<Appointment.UpdateAppointmentRequest, AppointmentDTO.UpdateAppointmentRequest>().ReverseMap();
            CreateMap<AppointmentDTO.AppointmentResponse, Appointment.AppointmentResponse>().ReverseMap();

            // Invoice
            CreateMap<Invoice.CreateInvoiceRequest, InvoiceDTO.CreateInvoiceRequest>().ReverseMap();
            CreateMap<Invoice.UpdateInvoiceRequest, InvoiceDTO.UpdateInvoiceRequest>().ReverseMap();
            CreateMap<InvoiceDTO.InvoiceResponse, Invoice.InvoiceResponse>().ReverseMap();
        }
    }
}
