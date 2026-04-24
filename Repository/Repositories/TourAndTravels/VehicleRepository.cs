using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Domain.Models.TourAndTravels;
using Newtonsoft.Json;
using Repository.DataModels.TourAndTravels;
using Repository.Interfaces.TourAndTravels;

namespace Repository.Repositories.TourAndTravels
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly IDbConnection _dbConnection;

        public VehicleRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Inventory.VehicleResponse CreateVehicle(Inventory.CreateVehicleRequest request, long userId)
        {
            string insertQuery = @"
                INSERT INTO vehicles
                (vehicle_type, model, registration_number, capacity, features, price_per_day,
                 price_per_day_usd, price_per_day_inr, price_per_km, price_per_km_usd, price_per_km_inr,
                 driver_name, driver_contact, insurance_number, insurance_expiry, 
                 permit_number, permit_expiry, description, is_active, created_by)
                VALUES
                (@VehicleType, @Model, @RegistrationNumber, @Capacity, @Features::jsonb, @PricePerDay,
                 @PricePerDayUsd, @PricePerDayInr, @PricePerKm, @PricePerKmUsd, @PricePerKmInr,
                 @DriverName, @DriverContact, @InsuranceNumber, @InsuranceExpiry,
                 @PermitNumber, @PermitExpiry, @Description, true, @CreatedBy)
                RETURNING id;";

            long vehicleId = _dbConnection.QuerySingle<long>(
                insertQuery,
                new
                {
                    request.VehicleType,
                    request.Model,
                    request.RegistrationNumber,
                    request.Capacity,
                    Features = JsonConvert.SerializeObject(request.Features ?? new List<string>()),
                    request.PricePerDay,
                    request.PricePerDayUsd,
                    request.PricePerDayInr,
                    request.PricePerKm,
                    request.PricePerKmUsd,
                    request.PricePerKmInr,
                    request.DriverName,
                    request.DriverContact,
                    request.InsuranceNumber,
                    request.InsuranceExpiry,
                    request.PermitNumber,
                    request.PermitExpiry,
                    request.Description,
                    CreatedBy = userId
                }
            );

            var result = GetVehicleById(vehicleId);
            if (result == null)
                throw new Exception("Failed to retrieve created vehicle");

            return result;
        }

        public List<Inventory.VehicleResponse> GetAllVehicles(bool includeInactive = false)
        {
            string query = @"
                SELECT 
                    id, vehicle_type, model, registration_number, capacity, features, price_per_day,
                    price_per_day_usd, price_per_day_inr, price_per_km, price_per_km_usd, price_per_km_inr,
                    driver_name, driver_contact, insurance_number, insurance_expiry, 
                    permit_number, permit_expiry, description, is_active, created_by, created_at
                FROM vehicles
                WHERE (@IncludeInactive = true OR is_active = true)
                ORDER BY vehicle_type, model;";

            var vehicles = _dbConnection.Query<VehicleDTO>(query, new { IncludeInactive = includeInactive }).ToList();

            return vehicles.Select(v => new Inventory.VehicleResponse
            {
                Id = v.Id,
                VehicleType = v.VehicleType,
                Model = v.Model,
                RegistrationNumber = v.RegistrationNumber,
                Capacity = v.Capacity,
                Features = string.IsNullOrWhiteSpace(v.Features) 
                    ? new List<string>() 
                    : JsonConvert.DeserializeObject<List<string>>(v.Features) ?? new List<string>(),
                PricePerDay = v.PricePerDay,
                PricePerDayUsd = v.PricePerDayUsd,
                PricePerDayInr = v.PricePerDayInr,
                PricePerKm = v.PricePerKm,
                PricePerKmUsd = v.PricePerKmUsd,
                PricePerKmInr = v.PricePerKmInr,
                DriverName = v.DriverName,
                DriverContact = v.DriverContact,
                InsuranceNumber = v.InsuranceNumber,
                InsuranceExpiry = v.InsuranceExpiry,
                PermitNumber = v.PermitNumber,
                PermitExpiry = v.PermitExpiry,
                Description = v.Description,
                IsActive = v.IsActive,
                DaysBooked = 0, // TODO: Calculate from bookings
                CreatedAt = v.CreatedAt
            }).ToList();
        }

        public Inventory.VehicleResponse? GetVehicleById(long id)
        {
            string query = @"
                SELECT 
                    id, vehicle_type, model, registration_number, capacity, features, price_per_day,
                    price_per_day_usd, price_per_day_inr, price_per_km, price_per_km_usd, price_per_km_inr,
                    driver_name, driver_contact, insurance_number, insurance_expiry,
                    permit_number, permit_expiry, description, is_active, created_by, created_at
                FROM vehicles
                WHERE id = @Id;";

            var vehicle = _dbConnection.QuerySingleOrDefault<VehicleDTO>(query, new { Id = id });

            if (vehicle == null)
                return null;

            return new Inventory.VehicleResponse
            {
                Id = vehicle.Id,
                VehicleType = vehicle.VehicleType,
                Model = vehicle.Model,
                RegistrationNumber = vehicle.RegistrationNumber,
                Capacity = vehicle.Capacity,
                Features = string.IsNullOrWhiteSpace(vehicle.Features) 
                    ? new List<string>() 
                    : JsonConvert.DeserializeObject<List<string>>(vehicle.Features) ?? new List<string>(),
                PricePerDay = vehicle.PricePerDay,
                PricePerDayUsd = vehicle.PricePerDayUsd,
                PricePerDayInr = vehicle.PricePerDayInr,
                PricePerKm = vehicle.PricePerKm,
                PricePerKmUsd = vehicle.PricePerKmUsd,
                PricePerKmInr = vehicle.PricePerKmInr,
                DriverName = vehicle.DriverName,
                DriverContact = vehicle.DriverContact,
                InsuranceNumber = vehicle.InsuranceNumber,
                InsuranceExpiry = vehicle.InsuranceExpiry,
                PermitNumber = vehicle.PermitNumber,
                PermitExpiry = vehicle.PermitExpiry,
                Description = vehicle.Description,
                IsActive = vehicle.IsActive,
                DaysBooked = 0, // TODO: Calculate from bookings
                CreatedAt = vehicle.CreatedAt
            };
        }

        public Inventory.VehicleResponse? UpdateVehicle(long id, Inventory.UpdateVehicleRequest request)
        {
            var updateFields = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            if (request.VehicleType != null)
            {
                updateFields.Add("vehicle_type = @VehicleType");
                parameters.Add("VehicleType", request.VehicleType);
            }
            if (request.Model != null)
            {
                updateFields.Add("model = @Model");
                parameters.Add("Model", request.Model);
            }
            if (request.RegistrationNumber != null)
            {
                updateFields.Add("registration_number = @RegistrationNumber");
                parameters.Add("RegistrationNumber", request.RegistrationNumber);
            }
            if (request.Capacity.HasValue)
            {
                updateFields.Add("capacity = @Capacity");
                parameters.Add("Capacity", request.Capacity);
            }
            if (request.Features != null)
            {
                updateFields.Add("features = @Features::jsonb");
                parameters.Add("Features", JsonConvert.SerializeObject(request.Features));
            }
            if (request.PricePerDay.HasValue)
            {
                updateFields.Add("price_per_day = @PricePerDay");
                parameters.Add("PricePerDay", request.PricePerDay);
            }
            if (request.PricePerDayUsd.HasValue)
            {
                updateFields.Add("price_per_day_usd = @PricePerDayUsd");
                parameters.Add("PricePerDayUsd", request.PricePerDayUsd);
            }
            if (request.PricePerDayInr.HasValue)
            {
                updateFields.Add("price_per_day_inr = @PricePerDayInr");
                parameters.Add("PricePerDayInr", request.PricePerDayInr);
            }
            if (request.PricePerKm.HasValue)
            {
                updateFields.Add("price_per_km = @PricePerKm");
                parameters.Add("PricePerKm", request.PricePerKm);
            }
            if (request.PricePerKmUsd.HasValue)
            {
                updateFields.Add("price_per_km_usd = @PricePerKmUsd");
                parameters.Add("PricePerKmUsd", request.PricePerKmUsd);
            }
            if (request.PricePerKmInr.HasValue)
            {
                updateFields.Add("price_per_km_inr = @PricePerKmInr");
                parameters.Add("PricePerKmInr", request.PricePerKmInr);
            }
            if (request.DriverName != null)
            {
                updateFields.Add("driver_name = @DriverName");
                parameters.Add("DriverName", request.DriverName);
            }
            if (request.DriverContact != null)
            {
                updateFields.Add("driver_contact = @DriverContact");
                parameters.Add("DriverContact", request.DriverContact);
            }
            if (request.InsuranceNumber != null)
            {
                updateFields.Add("insurance_number = @InsuranceNumber");
                parameters.Add("InsuranceNumber", request.InsuranceNumber);
            }
            if (request.InsuranceExpiry.HasValue)
            {
                updateFields.Add("insurance_expiry = @InsuranceExpiry");
                parameters.Add("InsuranceExpiry", request.InsuranceExpiry);
            }
            if (request.PermitNumber != null)
            {
                updateFields.Add("permit_number = @PermitNumber");
                parameters.Add("PermitNumber", request.PermitNumber);
            }
            if (request.PermitExpiry.HasValue)
            {
                updateFields.Add("permit_expiry = @PermitExpiry");
                parameters.Add("PermitExpiry", request.PermitExpiry);
            }
            if (request.Description != null)
            {
                updateFields.Add("description = @Description");
                parameters.Add("Description", request.Description);
            }
            if (request.IsActive.HasValue)
            {
                updateFields.Add("is_active = @IsActive");
                parameters.Add("IsActive", request.IsActive);
            }

            if (!updateFields.Any())
                return GetVehicleById(id);

            string updateQuery = $@"
                UPDATE vehicles
                SET {string.Join(", ", updateFields)}
                WHERE id = @Id;";

            _dbConnection.Execute(updateQuery, parameters);

            return GetVehicleById(id);
        }

        public bool DeleteVehicle(long id)
        {
            string query = "UPDATE vehicles SET is_active = false WHERE id = @Id;";
            int rowsAffected = _dbConnection.Execute(query, new { Id = id });
            return rowsAffected > 0;
        }

        public bool ActivateVehicle(long id)
        {
            string query = "UPDATE vehicles SET is_active = true WHERE id = @Id;";
            int rowsAffected = _dbConnection.Execute(query, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
