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
    public class GuideRepository : IGuideRepository
    {
        private readonly IDbConnection _dbConnection;

        public GuideRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Inventory.GuideResponse CreateGuide(Inventory.CreateGuideRequest request, long userId)
        {
            string insertQuery = @"
                INSERT INTO guides
                (full_name, phone, email, experience_years, languages, specialization,
                 certification_number, price_per_day, rating, bio, photo, is_active, created_by)
                VALUES
                (@FullName, @Phone, @Email, @ExperienceYears, @Languages::jsonb, @Specialization,
                 @CertificationNumber, @PricePerDay, @Rating, @Bio, @Photo, true, @CreatedBy)
                RETURNING id;";

            long guideId = _dbConnection.QuerySingle<long>(
                insertQuery,
                new
                {
                    request.FullName,
                    request.Phone,
                    request.Email,
                    request.ExperienceYears,
                    Languages = JsonConvert.SerializeObject(request.Languages ?? new List<string>()),
                    request.Specialization,
                    request.CertificationNumber,
                    request.PricePerDay,
                    request.Bio,
                    request.Photo,
                    CreatedBy = userId
                }
            );

            var result = GetGuideById(guideId);
            if (result == null)
                throw new Exception("Failed to retrieve created guide");

            return result;
        }

        public List<Inventory.GuideResponse> GetAllGuides(bool includeInactive = false)
        {
            string query = @"
                SELECT 
                    id, full_name, phone, email, experience_years, languages, specialization,
                    certification_number, price_per_day, rating, bio, photo, is_active, created_by, created_at
                FROM guides
                WHERE (@IncludeInactive = true OR is_active = true)
                ORDER BY full_name;";

            var guides = _dbConnection.Query<GuideDTO>(query, new { IncludeInactive = includeInactive }).ToList();

            return guides.Select(g => new Inventory.GuideResponse
            {
                Id = g.Id,
                FullName = g.FullName,
                Phone = g.Phone,
                Email = g.Email,
                ExperienceYears = g.ExperienceYears ?? 0,
                Languages = string.IsNullOrWhiteSpace(g.Languages) 
                    ? new List<string>() 
                    : JsonConvert.DeserializeObject<List<string>>(g.Languages) ?? new List<string>(),
                Specialization = g.Specialization,
                CertificationNumber = g.CertificationNumber,
                PricePerDay = g.PricePerDay,
                Rating = g.Rating,
                Bio = g.Bio,
                Photo = g.Photo,
                IsActive = g.IsActive,
                CreatedAt = g.CreatedAt
            }).ToList();
        }

        public Inventory.GuideResponse? GetGuideById(long id)
        {
            string query = @"
                SELECT 
                    id, full_name, phone, email, experience_years, languages, specialization,
                    certification_number, price_per_day, rating, bio, photo, is_active, created_by, created_at
                FROM guides
                WHERE id = @Id;";

            var guide = _dbConnection.QuerySingleOrDefault<GuideDTO>(query, new { Id = id });

            if (guide == null)
                return null;

            return new Inventory.GuideResponse
            {
                Id = guide.Id,
                FullName = guide.FullName,
                Phone = guide.Phone,
                Email = guide.Email,
                ExperienceYears = guide.ExperienceYears ?? 0,
                Languages = string.IsNullOrWhiteSpace(guide.Languages) 
                    ? new List<string>() 
                    : JsonConvert.DeserializeObject<List<string>>(guide.Languages) ?? new List<string>(),
                Specialization = guide.Specialization,
                CertificationNumber = guide.CertificationNumber,
                PricePerDay = guide.PricePerDay,
                Rating = guide.Rating,
                Bio = guide.Bio,
                Photo = guide.Photo,
                IsActive = guide.IsActive,
                CreatedAt = guide.CreatedAt
            };
        }

        public Inventory.GuideResponse? UpdateGuide(long id, Inventory.UpdateGuideRequest request)
        {
            var updateFields = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            if (request.FullName != null)
            {
                updateFields.Add("full_name = @FullName");
                parameters.Add("FullName", request.FullName);
            }
            if (request.Phone != null)
            {
                updateFields.Add("phone = @Phone");
                parameters.Add("Phone", request.Phone);
            }
            if (request.Email != null)
            {
                updateFields.Add("email = @Email");
                parameters.Add("Email", request.Email);
            }
            if (request.ExperienceYears.HasValue)
            {
                updateFields.Add("experience_years = @ExperienceYears");
                parameters.Add("ExperienceYears", request.ExperienceYears);
            }
            if (request.Languages != null)
            {
                updateFields.Add("languages = @Languages::jsonb");
                parameters.Add("Languages", JsonConvert.SerializeObject(request.Languages));
            }
            if (request.Specialization != null)
            {
                updateFields.Add("specialization = @Specialization");
                parameters.Add("Specialization", request.Specialization);
            }
            if (request.CertificationNumber != null)
            {
                updateFields.Add("CertificationNumber = @CertificationNumber");
                parameters.Add("CertificationNumber", request.CertificationNumber);
            }
            if (request.PricePerDay.HasValue)
            {
                updateFields.Add("price_per_day = @PricePerDay");
                parameters.Add("PricePerDay", request.PricePerDay);
            }
            if (request.Bio != null)
            {
                updateFields.Add("bio = @Bio");
                parameters.Add("Bio", request.Bio);
            }
            if (request.Photo != null)
            {
                updateFields.Add("photo = @Photo");
                parameters.Add("Photo", request.Photo);
            }

            if (!updateFields.Any())
                return GetGuideById(id);

            string updateQuery = $@"
                UPDATE guides
                SET {string.Join(", ", updateFields)}
                WHERE id = @Id;";

            _dbConnection.Execute(updateQuery, parameters);

            return GetGuideById(id);
        }

        public bool DeleteGuide(long id)
        {
            string query = "UPDATE guides SET is_active = false WHERE id = @Id;";
            int rowsAffected = _dbConnection.Execute(query, new { Id = id });
            return rowsAffected > 0;
        }

        public bool ActivateGuide(long id)
        {
            string query = "UPDATE guides SET is_active = true WHERE id = @Id;";
            int rowsAffected = _dbConnection.Execute(query, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
