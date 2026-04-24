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
    public class HotelRepository : IHotelRepository
    {
        private readonly IDbConnection _dbConnection;

        public HotelRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Inventory.HotelResponse CreateHotel(Inventory.CreateHotelRequest request, long userId)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            long hotelId;

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    string insertHotelQuery = @"
                        INSERT INTO hotels
                        (name, location, address, contact_person, phone, email, 
                         star_rating, category, amenities, is_active, created_by)
                        VALUES
                        (@Name, @Location, @Address, @ContactPerson, @Phone, @Email,
                         @StarRating, @Category, @Amenities::jsonb, true, @CreatedBy)
                        RETURNING id;";

                    hotelId = _dbConnection.QuerySingle<long>(
                        insertHotelQuery,
                        new
                        {
                            request.Name,
                            request.Location,
                            request.Address,
                            request.ContactPerson,
                            request.Phone,
                            request.Email,
                            request.StarRating,
                            request.Category,
                            Amenities = JsonConvert.SerializeObject(request.Amenities ?? new List<string>()),
                            CreatedBy = userId
                        },
                        transaction
                    );

                    // Insert rooms
                    if (request.Rooms != null && request.Rooms.Any())
                    {
                        foreach (var room in request.Rooms)
                        {
                            string insertRoomQuery = @"
                                INSERT INTO hotel_rooms
                                (hotel_id, room_type, capacity, total_rooms, price_per_night, price_per_night_usd, price_per_night_inr, features)
                                VALUES
                                (@HotelId, @RoomType, @Capacity, @TotalRooms, @PricePerNight, @PricePerNightUsd, @PricePerNightInr, @Features::jsonb);";

                            _dbConnection.Execute(
                                insertRoomQuery,
                                new
                                {
                                    HotelId = hotelId,
                                    room.RoomType,
                                    room.Capacity,
                                    room.TotalRooms,
                                    room.PricePerNight,
                                    room.PricePerNightUsd,
                                    room.PricePerNightInr,
                                    Features = JsonConvert.SerializeObject(room.Features ?? new List<string>())
                                },
                                transaction
                            );
                        }
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            // Retrieve created hotel outside transaction scope
            var result = GetHotelById(hotelId);
            if (result == null)
                throw new Exception("Failed to retrieve created hotel");

            return result;
        }

        public List<Inventory.HotelResponse> GetAllHotels(bool includeInactive = false)
        {
            string query = @"
                SELECT 
                    h.id, h.name, h.location, h.address, h.contact_person, h.phone, 
                    h.email, h.star_rating, h.category, h.amenities, h.is_active,
                    h.created_by, h.created_at
                FROM hotels h
                WHERE (@IncludeInactive = true OR h.is_active = true)
                ORDER BY h.name;";

            var hotels = _dbConnection.Query<HotelDTO>(query, new { IncludeInactive = includeInactive }).ToList();

            return hotels.Select(h => new Inventory.HotelResponse
            {
                Id = h.Id,
                Name = h.Name,
                Location = h.Location,
                Address = h.Address,
                ContactPerson = h.ContactPerson,
                Phone = h.Phone,
                Email = h.Email,
                StarRating = h.StarRating,
                Category = h.Category,
                Amenities = string.IsNullOrWhiteSpace(h.Amenities) 
                    ? new List<string>() 
                    : JsonConvert.DeserializeObject<List<string>>(h.Amenities) ?? new List<string>(),
                IsActive = h.IsActive,
                CreatedAt = h.CreatedAt,
                Rooms = new List<Inventory.HotelRoomResponse>() // Rooms loaded separately if needed
            }).ToList();
        }

        public Inventory.HotelResponse? GetHotelById(long id)
        {
            string hotelQuery = @"
                SELECT 
                    h.id, h.name, h.location, h.address, h.contact_person, h.phone,
                    h.email, h.star_rating, h.category, h.amenities, h.is_active,
                    h.created_by, h.created_at
                FROM hotels h
                WHERE h.id = @Id;";

            var hotel = _dbConnection.QuerySingleOrDefault<HotelDTO>(hotelQuery, new { Id = id });

            if (hotel == null)
                return null;

            // Load rooms
            string roomsQuery = @"
                SELECT 
                    id, hotel_id, room_type, capacity, total_rooms, price_per_night, price_per_night_usd, price_per_night_inr, features
                FROM hotel_rooms
                WHERE hotel_id = @HotelId
                ORDER BY room_type;";

            var rooms = _dbConnection.Query<HotelRoomDTO>(roomsQuery, new { HotelId = id }).ToList();

            return new Inventory.HotelResponse
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Location = hotel.Location,
                Address = hotel.Address,
                ContactPerson = hotel.ContactPerson,
                Phone = hotel.Phone,
                Email = hotel.Email,
                StarRating = hotel.StarRating,
                Category = hotel.Category,
                Amenities = string.IsNullOrWhiteSpace(hotel.Amenities) 
                    ? new List<string>() 
                    : JsonConvert.DeserializeObject<List<string>>(hotel.Amenities) ?? new List<string>(),
                IsActive = hotel.IsActive,
                CreatedAt = hotel.CreatedAt,
                Rooms = rooms.Select(r => new Inventory.HotelRoomResponse
                {
                    Id = r.Id,
                    RoomType = r.RoomType,
                    Capacity = r.Capacity,
                    TotalRooms = r.TotalRooms,
                    PricePerNight = r.PricePerNight,
                    PricePerNightUsd = r.PricePerNightUsd,
                    PricePerNightInr = r.PricePerNightInr,
                    Features = string.IsNullOrWhiteSpace(r.Features) 
                        ? new List<string>() 
                        : JsonConvert.DeserializeObject<List<string>>(r.Features) ?? new List<string>()
                }).ToList()
            };
        }

        public Inventory.HotelResponse? UpdateHotel(long id, Inventory.UpdateHotelRequest request)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Build dynamic update query
                    var updateFields = new List<string>();
                    var parameters = new DynamicParameters();
                    parameters.Add("Id", id);

                    if (request.Name != null)
                    {
                        updateFields.Add("name = @Name");
                        parameters.Add("Name", request.Name);
                    }
                    if (request.Location != null)
                    {
                        updateFields.Add("location = @Location");
                        parameters.Add("Location", request.Location);
                    }
                    if (request.Address != null)
                    {
                        updateFields.Add("address = @Address");
                        parameters.Add("Address", request.Address);
                    }
                    if (request.ContactPerson != null)
                    {
                        updateFields.Add("contact_person = @ContactPerson");
                        parameters.Add("ContactPerson", request.ContactPerson);
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
                    if (request.StarRating.HasValue)
                    {
                        updateFields.Add("star_rating = @StarRating");
                        parameters.Add("StarRating", request.StarRating);
                    }
                    if (request.Category != null)
                    {
                        updateFields.Add("category = @Category");
                        parameters.Add("Category", request.Category);
                    }
                    if (request.Amenities != null)
                    {
                        updateFields.Add("amenities = @Amenities::jsonb");
                        parameters.Add("Amenities", JsonConvert.SerializeObject(request.Amenities));
                    }

                    if (updateFields.Any())
                    {
                        string updateQuery = $@"
                            UPDATE hotels
                            SET {string.Join(", ", updateFields)}
                            WHERE id = @Id;";

                        _dbConnection.Execute(updateQuery, parameters, transaction);
                    }

                    // Update rooms if provided
                    if (request.Rooms != null && request.Rooms.Any())
                    {
                        // Delete existing rooms
                        _dbConnection.Execute("DELETE FROM hotel_rooms WHERE hotel_id = @Id", new { Id = id }, transaction);

                        // Insert new rooms
                        foreach (var room in request.Rooms)
                        {
                            string insertRoomQuery = @"
                                INSERT INTO hotel_rooms
                                (hotel_id, room_type, capacity, total_rooms, price_per_night, price_per_night_usd, price_per_night_inr, features)
                                VALUES
                                (@HotelId, @RoomType, @Capacity, @TotalRooms, @PricePerNight, @PricePerNightUsd, @PricePerNightInr, @Features::jsonb);";

                            _dbConnection.Execute(
                                insertRoomQuery,
                                new
                                {
                                    HotelId = id,
                                    room.RoomType,
                                    room.Capacity,
                                    room.TotalRooms,
                                    room.PricePerNight,
                                    room.PricePerNightUsd,
                                    room.PricePerNightInr,
                                    Features = JsonConvert.SerializeObject(room.Features ?? new List<string>())
                                },
                                transaction
                            );
                        }
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            // Retrieve updated hotel outside transaction scope
            return GetHotelById(id);
        }

        public bool DeleteHotel(long id)
        {
            string query = "UPDATE hotels SET is_active = false WHERE id = @Id;";
            int rowsAffected = _dbConnection.Execute(query, new { Id = id });
            return rowsAffected > 0;
        }

        public bool ActivateHotel(long id)
        {
            string query = "UPDATE hotels SET is_active = true WHERE id = @Id;";
            int rowsAffected = _dbConnection.Execute(query, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
