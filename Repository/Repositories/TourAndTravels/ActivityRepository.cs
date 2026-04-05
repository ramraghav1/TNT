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
    public class ActivityRepository : IActivityRepository
    {
        private readonly IDbConnection _dbConnection;

        public ActivityRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Inventory.ActivityResponse CreateActivity(Inventory.CreateActivityRequest request, long userId)
        {
            string insertQuery = @"
                INSERT INTO activities
                (name, activity_type, location, duration_hours, difficulty_level, max_participants,
                 min_participants, equipment, price_per_person, description, safety_instructions, images,
                 is_active, created_by)
                VALUES
                (@Name, @ActivityType, @Location, @DurationHours, @DifficultyLevel, @MaxParticipants,
                 @MinParticipants, @Equipment::jsonb, @PricePerPerson, @Description, @SafetyInstructions, @Images::jsonb,
                 true, @CreatedBy)
                RETURNING id;";

            long activityId = _dbConnection.QuerySingle<long>(
                insertQuery,
                new
                {
                    request.Name,
                    request.ActivityType,
                    request.Location,
                    request.DurationHours,
                    request.DifficultyLevel,
                    request.MaxParticipants,
                    request.MinParticipants,
                    Equipment = JsonConvert.SerializeObject(request.Equipment ?? new List<string>()),
                    request.PricePerPerson,
                    request.Description,
                    request.SafetyInstructions,
                    Images = JsonConvert.SerializeObject(request.Images ?? new List<string>()),
                    CreatedBy = userId
                }
            );

            var result = GetActivityById(activityId);
            if (result == null)
                throw new Exception("Failed to retrieve created activity");

            return result;
        }

        public List<Inventory.ActivityResponse> GetAllActivities(bool includeInactive = false)
        {
            string query = @"
                SELECT 
                    id, name, activity_type, location, duration_hours, difficulty_level, max_participants,
                    min_participants, equipment, price_per_person, description, safety_instructions, images,
                    is_active, created_by, created_at
                FROM activities
                WHERE (@IncludeInactive = true OR is_active = true)
                ORDER BY name;";

            var activities = _dbConnection.Query<ActivityDTO>(query, new { IncludeInactive = includeInactive }).ToList();

            return activities.Select(a => new Inventory.ActivityResponse
            {
                Id = a.Id,
                Name = a.Name,
                ActivityType = a.ActivityType,
                Location = a.Location,
                DurationHours = a.DurationHours,
                DifficultyLevel = a.DifficultyLevel,
                MaxParticipants = a.MaxParticipants ?? 10,
                MinParticipants = a.MinParticipants ?? 1,
                Equipment = string.IsNullOrWhiteSpace(a.Equipment) 
                    ? new List<string>() 
                    : JsonConvert.DeserializeObject<List<string>>(a.Equipment) ?? new List<string>(),
                PricePerPerson = a.PricePerPerson,
                Description = a.Description,
                SafetyInstructions = a.SafetyInstructions,
                Images = string.IsNullOrWhiteSpace(a.Images) 
                    ? new List<string>() 
                    : JsonConvert.DeserializeObject<List<string>>(a.Images) ?? new List<string>(),
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt
            }).ToList();
        }

        public Inventory.ActivityResponse? GetActivityById(long id)
        {
            string query = @"
                SELECT 
                    id, name, activity_type, location, duration_hours, difficulty_level, max_participants,
                    min_participants, equipment, price_per_person, description, safety_instructions, images,
                    is_active, created_by, created_at
                FROM activities
                WHERE id = @Id;";

            var activity = _dbConnection.QuerySingleOrDefault<ActivityDTO>(query, new { Id = id });

            if (activity == null)
                return null;

            return new Inventory.ActivityResponse
            {
                Id = activity.Id,
                Name = activity.Name,
                ActivityType = activity.ActivityType,
                Location = activity.Location,
                DurationHours = activity.DurationHours,
                DifficultyLevel = activity.DifficultyLevel,
                MaxParticipants = activity.MaxParticipants ?? 10,
                MinParticipants = activity.MinParticipants ?? 1,
                Equipment = string.IsNullOrWhiteSpace(activity.Equipment) 
                    ? new List<string>() 
                    : JsonConvert.DeserializeObject<List<string>>(activity.Equipment) ?? new List<string>(),
                PricePerPerson = activity.PricePerPerson,
                Description = activity.Description,
                SafetyInstructions = activity.SafetyInstructions,
                Images = string.IsNullOrWhiteSpace(activity.Images) 
                    ? new List<string>() 
                    : JsonConvert.DeserializeObject<List<string>>(activity.Images) ?? new List<string>(),
                IsActive = activity.IsActive,
                CreatedAt = activity.CreatedAt
            };
        }

        public Inventory.ActivityResponse? UpdateActivity(long id, Inventory.UpdateActivityRequest request)
        {
            var updateFields = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            if (request.Name != null)
            {
                updateFields.Add("name = @Name");
                parameters.Add("Name", request.Name);
            }
            if (request.ActivityType != null)
            {
                updateFields.Add("activity_type = @ActivityType");
                parameters.Add("ActivityType", request.ActivityType);
            }
            if (request.Location != null)
            {
                updateFields.Add("location = @Location");
                parameters.Add("Location", request.Location);
            }
            if (request.DurationHours.HasValue)
            {
                updateFields.Add("duration_hours = @DurationHours");
                parameters.Add("DurationHours", request.DurationHours);
            }
            if (request.DifficultyLevel != null)
            {
                updateFields.Add("difficulty_level = @DifficultyLevel");
                parameters.Add("DifficultyLevel", request.DifficultyLevel);
            }
            if (request.MaxParticipants.HasValue)
            {
                updateFields.Add("max_participants = @MaxParticipants");
                parameters.Add("MaxParticipants", request.MaxParticipants);
            }
            if (request.MinParticipants.HasValue)
            {
                updateFields.Add("min_participants = @MinParticipants");
                parameters.Add("MinParticipants", request.MinParticipants);
            }
            if (request.Equipment != null)
            {
                updateFields.Add("equipment = @Equipment::jsonb");
                parameters.Add("Equipment", JsonConvert.SerializeObject(request.Equipment));
            }
            if (request.PricePerPerson.HasValue)
            {
                updateFields.Add("price_per_person = @PricePerPerson");
                parameters.Add("PricePerPerson", request.PricePerPerson);
            }
            if (request.Description != null)
            {
                updateFields.Add("description = @Description");
                parameters.Add("Description", request.Description);
            }
            if (request.SafetyInstructions != null)
            {
                updateFields.Add("safety_instructions = @SafetyInstructions");
                parameters.Add("SafetyInstructions", request.SafetyInstructions);
            }
            if (request.Images != null)
            {
                updateFields.Add("images = @Images::jsonb");
                parameters.Add("Images", JsonConvert.SerializeObject(request.Images));
            }

            if (!updateFields.Any())
                return GetActivityById(id);

            string updateQuery = $@"
                UPDATE activities
                SET {string.Join(", ", updateFields)}
                WHERE id = @Id;";

            _dbConnection.Execute(updateQuery, parameters);

            return GetActivityById(id);
        }

        public bool DeleteActivity(long id)
        {
            string query = "UPDATE activities SET is_active = false WHERE id = @Id;";
            int rowsAffected = _dbConnection.Execute(query, new { Id = id });
            return rowsAffected > 0;
        }

        public bool ActivateActivity(long id)
        {
            string query = "UPDATE activities SET is_active = true WHERE id = @Id;";
            int rowsAffected = _dbConnection.Execute(query, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
