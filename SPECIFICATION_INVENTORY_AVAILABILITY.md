# Tour & Travel Management System
## Inventory & Availability Management Specification

**Version:** 1.0  
**Date:** April 2, 2026  
**Status:** Draft

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [System Architecture](#system-architecture)
3. [Module 1: Inventory Management](#module-1-inventory-management)
4. [Module 2: Availability & Scheduling](#module-2-availability--scheduling)
5. [Database Schema](#database-schema)
6. [API Endpoints](#api-endpoints)
7. [Frontend Components](#frontend-components)
8. [Business Logic & Rules](#business-logic--rules)
9. [User Workflows](#user-workflows)
10. [Implementation Phases](#implementation-phases)

---

## Executive Summary

This specification defines the **Inventory Management** and **Availability & Scheduling** modules for the TNT Tour & Travel Management System. These modules will manage hotels, vehicles, guides, and activities, along with their date-wise availability, capacity constraints, and booking integration.

### Key Features
- **Inventory Management**: Hotels, Vehicles, Guides, Activities
- **Availability Tracking**: Date-wise availability with capacity limits
- **Calendar View**: Visual scheduling interface
- **Booking Integration**: Connect inventory to existing booking/itinerary system
- **Multi-language Support**: English & Nepali (following existing pattern)

### Technology Stack
- **Backend**: .NET 8.0, C#, Dapper, PostgreSQL
- **Frontend**: Angular 21, PrimeNG, TypeScript
- **State Management**: Angular Signals
- **Authentication**: JWT (existing system)

---

## System Architecture

### High-Level Architecture
```
┌─────────────────────────────────────────────────────────────┐
│                    Frontend (Angular)                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Inventory  │  │ Availability  │  │   Calendar   │      │
│  │   Manager    │  │   Manager     │  │     View     │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                Backend API (.NET 8.0)                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Inventory  │  │ Availability  │  │   Booking    │      │
│  │   Service    │  │   Service     │  │ Integration  │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│               Data Layer (PostgreSQL)                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Hotels     │  │   Vehicles   │  │    Guides    │      │
│  │  Activities  │  │ Availability │  │  Bookings    │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
```

### Module Dependencies
- **Inventory** → Base module (independent)
- **Availability** → Depends on Inventory
- **Booking Integration** → Depends on Inventory + Availability + existing Booking module

---

## Module 1: Inventory Management

### 1.1 Hotels Module

#### Features
- Add/Edit/Delete hotels
- Hotel details (name, location, address, contact)
- Room types and capacity
- Amenities and facilities
- Pricing per room type
- Star rating and category
- Images and gallery
- Active/Inactive status

#### Data Model

**Hotel Entity**
```csharp
public class Hotel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }  // City/Area
    public string Address { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int StarRating { get; set; }  // 1-5
    public string Category { get; set; }  // Budget, Standard, Deluxe, Luxury
    public string? Description { get; set; }
    public string? Amenities { get; set; }  // JSON array
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CreatedBy { get; set; }
}

public class HotelRoom
{
    public long Id { get; set; }
    public long HotelId { get; set; }
    public string RoomType { get; set; }  // Single, Double, Suite, Deluxe
    public int Capacity { get; set; }  // Max persons per room
    public int TotalRooms { get; set; }  // Total inventory
    public decimal PricePerNight { get; set; }
    public string? Features { get; set; }  // JSON array
    public bool IsActive { get; set; }
}
```

**API Requests/Responses**
```csharp
public class CreateHotelRequest
{
    public string Name { get; set; }
    public string Location { get; set; }
    public string Address { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int StarRating { get; set; }
    public string Category { get; set; }
    public string? Description { get; set; }
    public List<string>? Amenities { get; set; }
    public List<HotelRoomRequest> Rooms { get; set; }
}

public class HotelRoomRequest
{
    public string RoomType { get; set; }
    public int Capacity { get; set; }
    public int TotalRooms { get; set; }
    public decimal PricePerNight { get; set; }
    public List<string>? Features { get; set; }
}

public class HotelResponse
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string Address { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int StarRating { get; set; }
    public string Category { get; set; }
    public string? Description { get; set; }
    public List<string> Amenities { get; set; }
    public bool IsActive { get; set; }
    public List<HotelRoomResponse> Rooms { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class HotelRoomResponse
{
    public long Id { get; set; }
    public string RoomType { get; set; }
    public int Capacity { get; set; }
    public int TotalRooms { get; set; }
    public decimal PricePerNight { get; set; }
    public List<string> Features { get; set; }
    public bool IsActive { get; set; }
}
```

#### UI Components
- **Hotel List** - Table with search, filter, pagination
- **Hotel Form** - Create/Edit modal with tabs (Basic Info, Rooms, Amenities)
- **Hotel Card** - Visual card display with rating, location, price range
- **Room Configuration** - Nested form for room types

---

### 1.2 Vehicles Module

#### Features
- Add/Edit/Delete vehicles
- Vehicle details (type, model, capacity, registration)
- Driver assignment
- Insurance and permit details
- Pricing per day/km
- Maintenance scheduling
- Active/Inactive status

#### Data Model

**Vehicle Entity**
```csharp
public class Vehicle
{
    public long Id { get; set; }
    public string VehicleType { get; set; }  // Bus, Van, Car, Jeep, Hiace
    public string Model { get; set; }
    public string RegistrationNumber { get; set; }
    public int Capacity { get; set; }  // Number of seats
    public string? Features { get; set; }  // AC, Non-AC, WiFi, etc. (JSON)
    public decimal PricePerDay { get; set; }
    public decimal? PricePerKm { get; set; }
    public string? DriverName { get; set; }
    public string? DriverContact { get; set; }
    public string? InsuranceNumber { get; set; }
    public DateTime? InsuranceExpiry { get; set; }
    public string? PermitNumber { get; set; }
    public DateTime? PermitExpiry { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CreatedBy { get; set; }
}
```

**API Requests/Responses**
```csharp
public class CreateVehicleRequest
{
    public string VehicleType { get; set; }
    public string Model { get; set; }
    public string RegistrationNumber { get; set; }
    public int Capacity { get; set; }
    public List<string>? Features { get; set; }
    public decimal PricePerDay { get; set; }
    public decimal? PricePerKm { get; set; }
    public string? DriverName { get; set; }
    public string? DriverContact { get; set; }
    public string? InsuranceNumber { get; set; }
    public DateTime? InsuranceExpiry { get; set; }
    public string? PermitNumber { get; set; }
    public DateTime? PermitExpiry { get; set; }
    public string? Description { get; set; }
}

public class VehicleResponse
{
    public long Id { get; set; }
    public string VehicleType { get; set; }
    public string Model { get; set; }
    public string RegistrationNumber { get; set; }
    public int Capacity { get; set; }
    public List<string> Features { get; set; }
    public decimal PricePerDay { get; set; }
    public decimal? PricePerKm { get; set; }
    public string? DriverName { get; set; }
    public string? DriverContact { get; set; }
    public string? InsuranceNumber { get; set; }
    public DateTime? InsuranceExpiry { get; set; }
    public string? PermitNumber { get; set; }
    public DateTime? PermitExpiry { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int DaysBooked { get; set; }  // Statistics
    public DateTime CreatedAt { get; set; }
}
```

#### UI Components
- **Vehicle List** - Table with type filter, capacity sort
- **Vehicle Form** - Create/Edit form with document upload
- **Vehicle Card** - Card with capacity, pricing, availability indicator
- **Maintenance Tracker** - Calendar for maintenance schedules

---

### 1.3 Guides Module

#### Features
- Add/Edit/Delete guides
- Guide profile (name, experience, languages, specialization)
- Certifications and licenses
- Availability calendar
- Pricing per day
- Rating and reviews
- Active/Inactive status

#### Data Model

**Guide Entity**
```csharp
public class Guide
{
    public long Id { get; set; }
    public string FullName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int ExperienceYears { get; set; }
    public string? Languages { get; set; }  // JSON array
    public string? Specialization { get; set; }  // Trekking, Cultural, Adventure, etc.
    public string? CertificationNumber { get; set; }
    public DateTime? CertificationExpiry { get; set; }
    public string? LicenseNumber { get; set; }
    public decimal PricePerDay { get; set; }
    public decimal? Rating { get; set; }  // Average rating
    public string? Bio { get; set; }
    public string? Photo { get; set; }  // URL
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CreatedBy { get; set; }
}
```

**API Requests/Responses**
```csharp
public class CreateGuideRequest
{
    public string FullName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int ExperienceYears { get; set; }
    public List<string>? Languages { get; set; }
    public string? Specialization { get; set; }
    public string? CertificationNumber { get; set; }
    public DateTime? CertificationExpiry { get; set; }
    public string? LicenseNumber { get; set; }
    public decimal PricePerDay { get; set; }
    public string? Bio { get; set; }
    public string? Photo { get; set; }
}

public class GuideResponse
{
    public long Id { get; set; }
    public string FullName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int ExperienceYears { get; set; }
    public List<string> Languages { get; set; }
    public string? Specialization { get; set; }
    public string? CertificationNumber { get; set; }
    public DateTime? CertificationExpiry { get; set; }
    public string? LicenseNumber { get; set; }
    public decimal PricePerDay { get; set; }
    public decimal? Rating { get; set; }
    public string? Bio { get; set; }
    public string? Photo { get; set; }
    public bool IsActive { get; set; }
    public int TripsCompleted { get; set; }  // Statistics
    public DateTime CreatedAt { get; set; }
}
```

#### UI Components
- **Guide List** - Table with language, specialization filters
- **Guide Profile** - Detailed view with photo, ratings, reviews
- **Guide Form** - Create/Edit form with photo upload
- **Guide Scheduler** - Calendar showing availability

---

### 1.4 Activities Module

#### Features
- Add/Edit/Delete activities
- Activity details (name, type, duration, difficulty)
- Capacity limits
- Equipment requirements
- Pricing options
- Location and availability
- Active/Inactive status

#### Data Model

**Activity Entity**
```csharp
public class Activity
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string ActivityType { get; set; }  // Rafting, Paragliding, Hiking, etc.
    public string? Location { get; set; }
    public int DurationHours { get; set; }
    public string DifficultyLevel { get; set; }  // Easy, Moderate, Hard, Extreme
    public int MaxParticipants { get; set; }
    public int MinParticipants { get; set; }
    public string? Equipment { get; set; }  // JSON array of required equipment
    public decimal PricePerPerson { get; set; }
    public string? Description { get; set; }
    public string? SafetyInstructions { get; set; }
    public string? Images { get; set; }  // JSON array of URLs
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CreatedBy { get; set; }
}
```

**API Requests/Responses**
```csharp
public class CreateActivityRequest
{
    public string Name { get; set; }
    public string ActivityType { get; set; }
    public string? Location { get; set; }
    public int DurationHours { get; set; }
    public string DifficultyLevel { get; set; }
    public int MaxParticipants { get; set; }
    public int MinParticipants { get; set; }
    public List<string>? Equipment { get; set; }
    public decimal PricePerPerson { get; set; }
    public string? Description { get; set; }
    public string? SafetyInstructions { get; set; }
    public List<string>? Images { get; set; }
}

public class ActivityResponse
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string ActivityType { get; set; }
    public string? Location { get; set; }
    public int DurationHours { get; set; }
    public string DifficultyLevel { get; set; }
    public int MaxParticipants { get; set; }
    public int MinParticipants { get; set; }
    public List<string> Equipment { get; set; }
    public decimal PricePerPerson { get; set; }
    public string? Description { get; set; }
    public string? SafetyInstructions { get; set; }
    public List<string> Images { get; set; }
    public bool IsActive { get; set; }
    public int TotalBookings { get; set; }  // Statistics
    public DateTime CreatedAt { get; set; }
}
```

#### UI Components
- **Activity List** - Card grid with type filters
- **Activity Detail** - Full page with images, description, pricing
- **Activity Form** - Create/Edit form with image upload
- **Activity Booking** - Integration with booking system

---

## Module 2: Availability & Scheduling

### 2.1 Core Availability System

#### Features
- Date-wise availability tracking for all inventory types
- Real-time capacity management
- Booking reservation integration
- Conflict detection and prevention
- Automated availability updates

#### Data Model

**Availability Entity**
```csharp
public class Availability
{
    public long Id { get; set; }
    public string InventoryType { get; set; }  // Hotel, Vehicle, Guide, Activity
    public long InventoryId { get; set; }
    public DateTime Date { get; set; }
    public int TotalCapacity { get; set; }
    public int BookedCapacity { get; set; }
    public int AvailableCapacity { get; set; }
    public string Status { get; set; }  // Available, Limited, Full, Blocked
    public decimal? SpecialPrice { get; set; }  // Peak season pricing override
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AvailabilityBlock
{
    public long Id { get; set; }
    public string InventoryType { get; set; }
    public long InventoryId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; }  // Maintenance, Unavailable, etc.
    public string? Notes { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

**API Requests/Responses**
```csharp
public class CheckAvailabilityRequest
{
    public string InventoryType { get; set; }
    public long? InventoryId { get; set; }  // Null = check all
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int RequiredCapacity { get; set; }
}

public class AvailabilityResponse
{
    public DateTime Date { get; set; }
    public string InventoryType { get; set; }
    public long InventoryId { get; set; }
    public string InventoryName { get; set; }
    public int TotalCapacity { get; set; }
    public int AvailableCapacity { get; set; }
    public string Status { get; set; }
    public decimal? Price { get; set; }
    public bool IsAvailable { get; set; }
}

public class BlockAvailabilityRequest
{
    public string InventoryType { get; set; }
    public long InventoryId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; }
    public string? Notes { get; set; }
}
```

---

### 2.2 Calendar View System

#### Features
- Monthly/Weekly/Daily calendar views
- Color-coded availability status
- Drag-and-drop booking assignment
- Multi-resource timeline view
- Capacity indicators

#### Data Model

**Calendar Event**
```csharp
public class CalendarEvent
{
    public long Id { get; set; }
    public string EventType { get; set; }  // Booking, Block, Maintenance
    public string InventoryType { get; set; }
    public long InventoryId { get; set; }
    public string Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
    public string? Color { get; set; }  // Visual indicator
    public string? Description { get; set; }
    public long? LinkedBookingId { get; set; }
}

public class CalendarViewRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? InventoryType { get; set; }  // Filter by type
    public long? InventoryId { get; set; }  // Filter by specific item
}

public class CalendarViewResponse
{
    public List<CalendarEvent> Events { get; set; }
    public Dictionary<DateTime, DailyCapacitySummary> DailySummaries { get; set; }
}

public class DailyCapacitySummary
{
    public DateTime Date { get; set; }
    public int TotalHotels { get; set; }
    public int AvailableHotels { get; set; }
    public int TotalVehicles { get; set; }
    public int AvailableVehicles { get; set; }
    public int TotalGuides { get; set; }
    public int AvailableGuides { get; set; }
    public int TotalActivities { get; set; }
    public int AvailableActivities { get; set; }
}
```

#### UI Components
- **Calendar Dashboard** - Main calendar view with filters
- **Resource Timeline** - Gantt-style view for multiple resources
- **Capacity Heatmap** - Visual representation of busy/available dates
- **Quick Booking Panel** - Sidebar for rapid booking creation

---

### 2.3 Date-wise Package Availability

#### Features
- Package-level availability management
- Automatic capacity calculation from components
- Departure date management
- Group size limits
- Guaranteed departure tracking

#### Data Model

**Package Availability**
```csharp
public class PackageAvailability
{
    public long Id { get; set; }
    public long ItineraryId { get; set; }
    public DateTime DepartureDate { get; set; }
    public int MinGroupSize { get; set; }
    public int MaxGroupSize { get; set; }
    public int CurrentBookings { get; set; }
    public int AvailableSeats { get; set; }
    public string Status { get; set; }  // OpenForBooking, GuaranteedDeparture, Full, Cancelled
    public decimal PackagePrice { get; set; }
    public bool IsGuaranteedDeparture { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreatePackageDepartureRequest
{
    public long ItineraryId { get; set; }
    public DateTime DepartureDate { get; set; }
    public int MinGroupSize { get; set; }
    public int MaxGroupSize { get; set; }
    public decimal PackagePrice { get; set; }
    public bool IsGuaranteedDeparture { get; set; }
}

public class PackageDepartureResponse
{
    public long Id { get; set; }
    public long ItineraryId { get; set; }
    public string ItineraryTitle { get; set; }
    public DateTime DepartureDate { get; set; }
    public int MinGroupSize { get; set; }
    public int MaxGroupSize { get; set; }
    public int CurrentBookings { get; set; }
    public int AvailableSeats { get; set; }
    public string Status { get; set; }
    public decimal PackagePrice { get; set; }
    public bool IsGuaranteedDeparture { get; set; }
    public bool CanStillBook { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

## Database Schema

### Table Structure

```sql
-- ===========================
-- HOTELS
-- ===========================
CREATE TABLE hotels (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    location VARCHAR(100) NOT NULL,
    address TEXT NOT NULL,
    contact_person VARCHAR(100),
    phone VARCHAR(20),
    email VARCHAR(100),
    star_rating INT CHECK (star_rating BETWEEN 1 AND 5),
    category VARCHAR(50) NOT NULL,  -- Budget, Standard, Deluxe, Luxury
    description TEXT,
    amenities JSONB,  -- ["WiFi", "Pool", "Restaurant"]
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by INT NOT NULL,
    CONSTRAINT fk_hotels_created_by FOREIGN KEY (created_by) REFERENCES users(id)
);

CREATE TABLE hotel_rooms (
    id BIGSERIAL PRIMARY KEY,
    hotel_id BIGINT NOT NULL,
    room_type VARCHAR(50) NOT NULL,  -- Single, Double, Suite, Deluxe
    capacity INT NOT NULL,  -- Max persons
    total_rooms INT NOT NULL,
    price_per_night DECIMAL(10,2) NOT NULL,
    features JSONB,  -- ["AC", "TV", "Balcony"]
    is_active BOOLEAN DEFAULT TRUE,
    CONSTRAINT fk_hotel_rooms_hotel FOREIGN KEY (hotel_id) REFERENCES hotels(id) ON DELETE CASCADE
);

CREATE INDEX idx_hotels_location ON hotels(location);
CREATE INDEX idx_hotels_category ON hotels(category);
CREATE INDEX idx_hotels_active ON hotels(is_active);

-- ===========================
-- VEHICLES
-- ===========================
CREATE TABLE vehicles (
    id BIGSERIAL PRIMARY KEY,
    vehicle_type VARCHAR(50) NOT NULL,  -- Bus, Van, Car, Jeep, Hiace
    model VARCHAR(100) NOT NULL,
    registration_number VARCHAR(50) UNIQUE NOT NULL,
    capacity INT NOT NULL,
    features JSONB,  -- ["AC", "WiFi", "Reclining Seats"]
    price_per_day DECIMAL(10,2) NOT NULL,
    price_per_km DECIMAL(10,2),
    driver_name VARCHAR(100),
    driver_contact VARCHAR(20),
    insurance_number VARCHAR(100),
    insurance_expiry DATE,
    permit_number VARCHAR(100),
    permit_expiry DATE,
    description TEXT,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by INT NOT NULL,
    CONSTRAINT fk_vehicles_created_by FOREIGN KEY (created_by) REFERENCES users(id)
);

CREATE INDEX idx_vehicles_type ON vehicles(vehicle_type);
CREATE INDEX idx_vehicles_capacity ON vehicles(capacity);
CREATE INDEX idx_vehicles_active ON vehicles(is_active);

-- ===========================
-- GUIDES
-- ===========================
CREATE TABLE guides (
    id BIGSERIAL PRIMARY KEY,
    full_name VARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    email VARCHAR(100),
    address TEXT,
    experience_years INT NOT NULL,
    languages JSONB,  -- ["English", "Nepali", "Hindi"]
    specialization VARCHAR(100),  -- Trekking, Cultural, Adventure
    certification_number VARCHAR(100),
    certification_expiry DATE,
    license_number VARCHAR(100),
    price_per_day DECIMAL(10,2) NOT NULL,
    rating DECIMAL(3,2),  -- Average rating
    bio TEXT,
    photo VARCHAR(500),  -- URL
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by INT NOT NULL,
    CONSTRAINT fk_guides_created_by FOREIGN KEY (created_by) REFERENCES users(id)
);

CREATE INDEX idx_guides_specialization ON guides(specialization);
CREATE INDEX idx_guides_active ON guides(is_active);

-- ===========================
-- ACTIVITIES
-- ===========================
CREATE TABLE activities (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    activity_type VARCHAR(50) NOT NULL,  -- Rafting, Paragliding, Hiking
    location VARCHAR(100),
    duration_hours INT NOT NULL,
    difficulty_level VARCHAR(20) NOT NULL,  -- Easy, Moderate, Hard, Extreme
    max_participants INT NOT NULL,
    min_participants INT NOT NULL DEFAULT 1,
    equipment JSONB,  -- ["Helmet", "Life Jacket"]
    price_per_person DECIMAL(10,2) NOT NULL,
    description TEXT,
    safety_instructions TEXT,
    images JSONB,  -- ["url1", "url2"]
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by INT NOT NULL,
    CONSTRAINT fk_activities_created_by FOREIGN KEY (created_by) REFERENCES users(id)
);

CREATE INDEX idx_activities_type ON activities(activity_type);
CREATE INDEX idx_activities_location ON activities(location);
CREATE INDEX idx_activities_active ON activities(is_active);

-- ===========================
-- AVAILABILITY
-- ===========================
CREATE TABLE availability (
    id BIGSERIAL PRIMARY KEY,
    inventory_type VARCHAR(20) NOT NULL,  -- Hotel, Vehicle, Guide, Activity
    inventory_id BIGINT NOT NULL,
    date DATE NOT NULL,
    total_capacity INT NOT NULL,
    booked_capacity INT NOT NULL DEFAULT 0,
    available_capacity INT NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'Available',  -- Available, Limited, Full, Blocked
    special_price DECIMAL(10,2),  -- Peak season override
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    CONSTRAINT uq_availability_inventory_date UNIQUE (inventory_type, inventory_id, date)
);

CREATE INDEX idx_availability_date ON availability(date);
CREATE INDEX idx_availability_type ON availability(inventory_type);
CREATE INDEX idx_availability_inventory ON availability(inventory_id);
CREATE INDEX idx_availability_status ON availability(status);

-- ===========================
-- AVAILABILITY BLOCKS
-- ===========================
CREATE TABLE availability_blocks (
    id BIGSERIAL PRIMARY KEY,
    inventory_type VARCHAR(20) NOT NULL,
    inventory_id BIGINT NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    reason VARCHAR(100) NOT NULL,  -- Maintenance, Personal Leave, etc.
    notes TEXT,
    created_by INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_blocks_created_by FOREIGN KEY (created_by) REFERENCES users(id),
    CONSTRAINT chk_block_dates CHECK (end_date >= start_date)
);

CREATE INDEX idx_blocks_dates ON availability_blocks(start_date, end_date);
CREATE INDEX idx_blocks_inventory ON availability_blocks(inventory_type, inventory_id);

-- ===========================
-- PACKAGE AVAILABILITY
-- ===========================
CREATE TABLE package_departures (
    id BIGSERIAL PRIMARY KEY,
    itinerary_id BIGINT NOT NULL,
    departure_date DATE NOT NULL,
    min_group_size INT NOT NULL,
    max_group_size INT NOT NULL,
    current_bookings INT NOT NULL DEFAULT 0,
    available_seats INT NOT NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'OpenForBooking',  -- OpenForBooking, GuaranteedDeparture, Full, Cancelled
    package_price DECIMAL(10,2) NOT NULL,
    is_guaranteed_departure BOOLEAN DEFAULT FALSE,
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    CONSTRAINT fk_package_itinerary FOREIGN KEY (itinerary_id) REFERENCES itinerary_template(id) ON DELETE CASCADE,
    CONSTRAINT uq_package_departure UNIQUE (itinerary_id, departure_date)
);

CREATE INDEX idx_package_departure_date ON package_departures(departure_date);
CREATE INDEX idx_package_itinerary ON package_departures(itinerary_id);
CREATE INDEX idx_package_status ON package_departures(status);

-- ===========================
-- BOOKING INVENTORY LINKS
-- ===========================
CREATE TABLE booking_inventory (
    id BIGSERIAL PRIMARY KEY,
    booking_instance_id BIGINT NOT NULL,
    inventory_type VARCHAR(20) NOT NULL,
    inventory_id BIGINT NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    quantity INT NOT NULL DEFAULT 1,
    price DECIMAL(10,2) NOT NULL,
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_booking_inventory_instance FOREIGN KEY (booking_instance_id) 
        REFERENCES booking_instance(id) ON DELETE CASCADE
);

CREATE INDEX idx_booking_inventory_instance ON booking_inventory(booking_instance_id);
CREATE INDEX idx_booking_inventory_type ON booking_inventory(inventory_type, inventory_id);
CREATE INDEX idx_booking_inventory_dates ON booking_inventory(start_date, end_date);
```

---

## API Endpoints

### Hotels API

```
GET    /api/hotels                      - List all hotels
GET    /api/hotels/{id}                 - Get hotel details
POST   /api/hotels                      - Create new hotel
PUT    /api/hotels/{id}                 - Update hotel
DELETE /api/hotels/{id}                 - Delete hotel
GET    /api/hotels/{id}/rooms           - Get hotel rooms
POST   /api/hotels/{id}/rooms           - Add room type
PUT    /api/hotels/rooms/{roomId}       - Update room
DELETE /api/hotels/rooms/{roomId}       - Delete room
GET    /api/hotels/search               - Search hotels by criteria
```

### Vehicles API

```
GET    /api/vehicles                    - List all vehicles
GET    /api/vehicles/{id}               - Get vehicle details
POST   /api/vehicles                    - Create new vehicle
PUT    /api/vehicles/{id}               - Update vehicle
DELETE /api/vehicles/{id}               - Delete vehicle
GET    /api/vehicles/by-type/{type}     - Filter by vehicle type
```

### Guides API

```
GET    /api/guides                      - List all guides
GET    /api/guides/{id}                 - Get guide profile
POST   /api/guides                      - Create new guide
PUT    /api/guides/{id}                 - Update guide
DELETE /api/guides/{id}                 - Delete guide
GET    /api/guides/by-specialization    - Filter by specialization
```

### Activities API

```
GET    /api/activities                  - List all activities
GET    /api/activities/{id}             - Get activity details
POST   /api/activities                  - Create new activity
PUT    /api/activities/{id}             - Update activity
DELETE /api/activities/{id}             - Delete activity
GET    /api/activities/by-type/{type}   - Filter by activity type
```

### Availability API

```
POST   /api/availability/check          - Check availability for date range
GET    /api/availability/calendar       - Get calendar view data
POST   /api/availability/block          - Block availability
DELETE /api/availability/block/{id}     - Unblock availability
PUT    /api/availability/update         - Update capacity/pricing
GET    /api/availability/summary        - Get daily summary for date range
POST   /api/availability/bulk-update    - Bulk update multiple dates
```

### Package Departures API

```
GET    /api/packages/departures         - List all departures
GET    /api/packages/{itineraryId}/departures - Get departures for itinerary
POST   /api/packages/departures         - Create new departure
PUT    /api/packages/departures/{id}    - Update departure
DELETE /api/packages/departures/{id}    - Cancel departure
GET    /api/packages/departures/available - Search available departures
```

---

## Frontend Components

### Angular Module Structure

```
src/app/pages/inventory/
├── hotels/
│   ├── hotel-list/
│   │   ├── hotel-list.ts
│   │   ├── hotel-list.html
│   │   └── hotel-list.scss
│   ├── hotel-form/
│   │   ├── hotel-form.ts
│   │   ├── hotel-form.html
│   │   └── hotel-form.scss
│   ├── hotel-card/
│   │   ├── hotel-card.ts
│   │   ├── hotel-card.html
│   │   └── hotel-card.scss
│   └── hotel.service.ts
├── vehicles/
│   ├── vehicle-list/
│   ├── vehicle-form/
│   └── vehicle.service.ts
├── guides/
│   ├── guide-list/
│   ├── guide-profile/
│   ├── guide-form/
│   └── guide.service.ts
├── activities/
│   ├── activity-list/
│   ├── activity-detail/
│   ├── activity-form/
│   └── activity.service.ts
└── inventory.routes.ts

src/app/pages/availability/
├── calendar-view/
│   ├── calendar-view.ts
│   ├── calendar-view.html
│   └── calendar-view.scss
├── availability-manager/
│   ├── availability-manager.ts
│   ├── availability-manager.html
│   └── availability-manager.scss
├── resource-timeline/
│   ├── resource-timeline.ts
│   ├── resource-timeline.html
│   └── resource-timeline.scss
├── package-departures/
│   ├── departure-list/
│   ├── departure-form/
│   └── departure-calendar/
└── availability.service.ts
```

### Key Component Features

#### Hotel List Component
- PrimeNG Table with sorting, filtering, pagination
- Card/Table toggle view
- Location, category, star rating filters
- Room availability indicator
- Quick actions (Edit, View, Deactivate)

#### Calendar View Component
- PrimeNG FullCalendar integration
- Month/Week/Day view switching
- Color-coded events (Available=Green, Limited=Yellow, Full=Red, Blocked=Gray)
- Click to view details or create booking
- Drag capability for admin users
- Export to PDF/Excel

#### Resource Timeline Component
- Gantt-style multi-resource view
- Shows all inventory items in parallel rows
- Date range selector
- Conflict highlighting
- Quick assignment of bookings to resources

---

## Business Logic & Rules

### Availability Calculation Rules

1. **Automatic Capacity Updates**
   - When booking is created → Reduce available capacity
   - When booking is cancelled → Restore available capacity
   - When booking is modified → Recalculate capacity

2. **Status Determination**
   - `Available`: availableCapacity > 50% of totalCapacity
   - `Limited`: availableCapacity between 1 and 50%
   - `Full`: availableCapacity = 0
   - `Blocked`: Manual block or maintenance

3. **Conflict Prevention**
   - Cannot double-book single resources (1 guide, 1 vehicle)
   - Can partial-book multi-unit resources (hotels with multiple rooms)
   - Warn if capacity < required amount
   - Block creation if already full

4. **Price Management**
   - Base price from inventory master
   - Override with seasonal/special pricing in availability table
   - Package pricing affects all component prices

### Package Departure Rules

1. **Booking Threshold**
   - Automatically mark "Guaranteed Departure" if bookings ≥ minGroupSize
   - Block new bookings if bookings ≥ maxGroupSize
   - Send notifications when thresholds reached

2. **Cancellation Policy**
   - Cannot cancel if status = "GuaranteedDeparture" without admin override
   - Refund all bookings if departure cancelled
   - Update package status to "Cancelled"

3. **Availability Sync**
   - Creating package departure reserves all required inventory
   - Check hotel, vehicle, guide availability for entire duration
   - Display availability conflicts before confirming

---

## User Workflows

### Workflow 1: Create Hotel and Add Rooms

1. Admin navigates to Inventory → Hotels
2. Clicks "Add Hotel" button
3. Fills basic information form (name, location, rating, etc.)
4. Adds multiple room types with capacity and pricing
5. Uploads hotel images
6. Saves hotel → System creates hotel and room records
7. System initializes availability for next 365 days

### Workflow 2: Check Vehicle Availability

1. Admin creating booking selects date range
2. System queries vehicle availability
3. Shows list of available vehicles with capacity, features, price
4. Admin selects vehicle → System reserves vehicle for booking
5. System updates availability table (reduces capacity)
6. Confirmation displayed

### Workflow 3: Block Guide Availability

1. Admin navigates to Guides → Select guide
2. Clicks "Block Availability" button
3. Selects date range and reason (e.g., "Personal Leave")
4. System creates availability_block record
5. System updates availability status to "Blocked" for date range
6. Guide appears unavailable in booking searches

### Workflow 4: Create Package Departure

1. Admin navigates to Packages → Select itinerary
2. Clicks "Add Departure Date"
3. Selects departure date
4. Sets min/max group size and package price
5. System checks inventory availability for entire itinerary duration
6. If available → Creates package_departure record
7. If conflicts → Displays list of unavailable resources
8. Admin resolves conflicts or chooses different date

### Workflow 5: Customer Books Available Package

1. Customer browses available packages on frontend
2. Filters by date, destination, duration
3. Sees list of departures with available seats
4. Selects departure date
5. Fills traveler information
6. System checks final availability (race condition check)
7. If still available → Creates booking and reduces available_seats
8. If just filled → Displays "Sorry, just booked out" message

---

## Implementation Phases

### Phase 1: Database & Backend Core (Week 1-2)
**Deliverables:**
- Database schema created and migrated
- DTOs and domain models
- Repository layer (Dapper queries)
- Basic CRUD services for all inventory types
- Unit tests for services

**Tasks:**
1. Create database migration files
2. Implement Hotel, Vehicle, Guide, Activity DTOs
3. Implement repositories with CRUD operations
4. Create domain models and mapping profiles
5. Implement services with business logic
6. Write unit tests

### Phase 2: API Controllers (Week 2-3)
**Deliverables:**
- REST API endpoints for all inventory types
- API documentation (Swagger)
- Request validation
- Error handling
- Authorization middleware

**Tasks:**
1. Create controllers for Hotels, Vehicles, Guides, Activities
2. Implement search and filter endpoints
3. Add validation attributes to request models
4. Configure Swagger documentation
5. Add role-based authorization
6. Integration testing

### Phase 3: Availability System Backend (Week 3-4)
**Deliverables:**
- Availability calculation logic
- Calendar API endpoints
- Blocking/unblocking functionality
- Package departure management
- Automated capacity updates

**Tasks:**
1. Implement availability service
2. Create availability check API
3. Implement blocking functionality
4. Create package departure service
5. Integrate with existing booking system
6. Write integration tests

### Phase 4: Frontend - Inventory Management (Week 4-6)
**Deliverables:**
- Hotel management UI (list, form, details)
- Vehicle management UI
- Guide management UI
- Activity management UI
- Translation support (EN/NP)

**Tasks:**
1. Create Angular services for each inventory type
2. Implement list components with PrimeNG tables
3. Create form components with validation
4. Add image upload functionality
5. Implement search and filter UI
6. Add translations to en.json and np.json
7. Component testing

### Phase 5: Frontend - Calendar & Availability (Week 6-8)
**Deliverables:**
- Calendar view component
- Resource timeline view
- Availability manager
- Package departure UI
- Booking integration

**Tasks:**
1. Integrate PrimeNG FullCalendar
2. Create calendar view with month/week/day switching
3. Implement resource timeline component
4. Create availability manager interface
5. Build package departure management UI
6. Connect to booking flow
7. Add real-time updates
8. End-to-end testing

### Phase 6: Integration & Testing (Week 8-9)
**Deliverables:**
- Full system integration
- Automated tests
- Performance optimization
- Bug fixes
- Documentation

**Tasks:**
1. Integration testing across all modules
2. Load testing for availability checks
3. Performance optimization (caching, indexing)
4. Fix identified bugs
5. User acceptance testing
6. Update documentation

### Phase 7: Deployment & Training (Week 9-10)
**Deliverables:**
- Production deployment
- User training materials
- Admin documentation
- Monitoring setup

**Tasks:**
1. Deploy to production environment
2. Configure production database
3. Set up monitoring and logging
4. Create user training videos/docs
5. Conduct admin training sessions
6. Post-deployment support

---

## Translation Keys (English & Nepali)

### Inventory Module Translations

```json
// en.json additions
"inventory": {
  "title": "Inventory",
  "hotels": {
    "title": "Hotels",
    "addNew": "Add Hotel",
    "name": "Hotel Name",
    "location": "Location",
    "address": "Address",
    "contactPerson": "Contact Person",
    "phone": "Phone",
    "email": "Email",
    "starRating": "Star Rating",
    "category": "Category",
    "description": "Description",
    "amenities": "Amenities",
    "rooms": "Rooms",
    "roomType": "Room Type",
    "capacity": "Capacity",
    "totalRooms": "Total Rooms",
    "pricePerNight": "Price Per Night",
    "features": "Features",
    "categories": {
      "budget": "Budget",
      "standard": "Standard",
      "deluxe": "Deluxe",
      "luxury": "Luxury"
    }
  },
  "vehicles": {
    "title": "Vehicles",
    "addNew": "Add Vehicle",
    "vehicleType": "Vehicle Type",
    "model": "Model",
    "registrationNumber": "Registration Number",
    "capacity": "Capacity",
    "features": "Features",
    "pricePerDay": "Price Per Day",
    "pricePerKm": "Price Per Km",
    "driverName": "Driver Name",
    "driverContact": "Driver Contact",
    "insurance": "Insurance",
    "insuranceNumber": "Insurance Number",
    "insuranceExpiry": "Insurance Expiry",
    "permit": "Permit",
    "permitNumber": "Permit Number",
    "permitExpiry": "Permit Expiry"
  },
  "guides": {
    "title": "Guides",
    "addNew": "Add Guide",
    "fullName": "Full Name",
    "phone": "Phone",
    "email": "Email",
    "address": "Address",
    "experienceYears": "Years of Experience",
    "languages": "Languages",
    "specialization": "Specialization",
    "certification": "Certification",
    "certificationNumber": "Certification Number",
    "certificationExpiry": "Certification Expiry",
    "license": "License",
    "licenseNumber": "License Number",
    "pricePerDay": "Price Per Day",
    "rating": "Rating",
    "bio": "Biography",
    "photo": "Photo"
  },
  "activities": {
    "title": "Activities",
    "addNew": "Add Activity",
    "name": "Activity Name",
    "activityType": "Activity Type",
    "location": "Location",
    "durationHours": "Duration (Hours)",
    "difficultyLevel": "Difficulty Level",
    "maxParticipants": "Max Participants",
    "minParticipants": "Min Participants",
    "equipment": "Equipment Required",
    "pricePerPerson": "Price Per Person",
    "description": "Description",
    "safetyInstructions": "Safety Instructions",
    "images": "Images"
  }
},
"availability": {
  "title": "Availability",
  "calendar": "Calendar",
  "checkAvailability": "Check Availability",
  "blockAvailability": "Block Availability",
  "available": "Available",
  "limited": "Limited",
  "full": "Full",
  "blocked": "Blocked",
  "capacity": "Capacity",
  "totalCapacity": "Total Capacity",
  "availableCapacity": "Available Capacity",
  "bookedCapacity": "Booked Capacity",
  "startDate": "Start Date",
  "endDate": "End Date",
  "reason": "Reason",
  "notes": "Notes",
  "packageDepartures": {
    "title": "Package Departures",
    "addDeparture": "Add Departure",
    "departureDate": "Departure Date",
    "minGroupSize": "Min Group Size",
    "maxGroupSize": "Max Group Size",
    "currentBookings": "Current Bookings",
    "availableSeats": "Available Seats",
    "packagePrice": "Package Price",
    "guaranteedDeparture": "Guaranteed Departure",
    "status": "Status",
    "openForBooking": "Open for Booking",
    "full": "Full",
    "cancelled": "Cancelled"
  }
}

// np.json (Nepali translations will be added similarly)
```

---

## Success Metrics

### KPIs to Track

1. **Operational Efficiency**
   - Time to create booking: < 5 minutes
   - Availability check response time: < 2 seconds
   - Calendar load time: < 3 seconds
   - Booking confirmation rate: > 90%

2. **Resource Utilization**
   - Hotel occupancy rate: Target 70%+
   - Vehicle utilization rate: Target 65%+
   - Guide booking rate: Target 60%+
   - Activity participation rate: Target 55%+

3. **User Satisfaction**
   - Admin task completion rate: > 95%
   - Customer booking success rate: > 85%
   - System uptime: 99.5%+
   - Error rate: < 1%

4. **Business Impact**
   - Revenue from inventory management
   - Reduction in overbooking incidents: 100%
   - Increase in package departure confirmations: 30%+
   - Customer retention rate improvement: 20%+

---

## Risk Assessment & Mitigation

### Technical Risks

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Race conditions in availability | High | Medium | Implement database locks, transaction isolation |
| Calendar performance with large data | Medium | High | Implement pagination, caching, lazy loading |
| Double booking | High | Low | Add unique constraints, implement optimistic locking |
| Data migration issues | Medium | Medium | Thorough testing, rollback plan, staged deployment |

### Business Risks

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| User resistance to new system | Medium | Medium | Training, gradual rollout, feedback loop |
| Incomplete inventory data | Medium | High | Import from existing sources, validation rules |
| Seasonal demand fluctuations | High | High | Dynamic pricing, waitlist feature, demand forecasting |
| Integration with external systems | Low | Low | API documentation, webhook support |

---

## Appendix

### A. Sample API Requests

**Create Hotel Example:**
```json
POST /api/hotels
{
  "name": "Hotel Everest View",
  "location": "Pokhara",
  "address": "Lakeside, Pokhara-6, Kaski",
  "contactPerson": "Ram Sharma",
  "phone": "+977-61-123456",
  "email": "info@everestview.com",
  "starRating": 4,
  "category": "Deluxe",
  "description": "Luxury hotel with mountain views",
  "amenities": ["WiFi", "Pool", "Restaurant", "Spa"],
  "rooms": [
    {
      "roomType": "Deluxe Double",
      "capacity": 2,
      "totalRooms": 10,
      "pricePerNight": 8000,
      "features": ["AC", "TV", "Balcony"]
    },
    {
      "roomType": "Suite",
      "capacity": 4,
      "totalRooms": 5,
      "pricePerNight": 15000,
      "features": ["AC", "TV", "Balcony", "Mini Bar"]
    }
  ]
}
```

**Check Availability Example:**
```json
POST /api/availability/check
{
  "inventoryType": "Hotel",
  "inventoryId": 123,
  "startDate": "2026-06-01",
  "endDate": "2026-06-05",
  "requiredCapacity": 3
}
```

### B. Database Queries Examples

**Get Available Vehicles for Date Range:**
```sql
SELECT v.*, 
       COALESCE(SUM(CASE WHEN a.status != 'Blocked' 
                         THEN a.available_capacity 
                         ELSE 0 END), 0) as available_days
FROM vehicles v
LEFT JOIN availability a ON a.inventory_type = 'Vehicle' 
                        AND a.inventory_id = v.id
                        AND a.date BETWEEN '2026-06-01' AND '2026-06-05'
WHERE v.is_active = true
GROUP BY v.id
HAVING COUNT(CASE WHEN a.status = 'Blocked' THEN 1 END) = 0
ORDER BY v.capacity DESC;
```

**Update Availability After Booking:**
```sql
UPDATE availability
SET booked_capacity = booked_capacity + :quantity,
    available_capacity = total_capacity - (booked_capacity + :quantity),
    status = CASE 
        WHEN available_capacity <= 0 THEN 'Full'
        WHEN available_capacity < (total_capacity * 0.5) THEN 'Limited'
        ELSE 'Available'
    END,
    updated_at = CURRENT_TIMESTAMP
WHERE inventory_type = :type
  AND inventory_id = :id
  AND date BETWEEN :start_date AND :end_date;
```

### C. Component Hierarchy

```
AppComponent
└── LayoutComponent
    └── RouterOutlet
        ├── InventoryDashboard
        │   ├── HotelList
        │   │   ├── HotelCard
        │   │   └── HotelForm (Modal)
        │   ├── VehicleList
        │   ├── GuideList
        │   └── ActivityList
        └── AvailabilityDashboard
            ├── CalendarView
            │   ├── CalendarToolbar
            │   ├── CalendarGrid
            │   └── EventDetails (Sidebar)
            ├── ResourceTimeline
            └── PackageDepartures
                ├── DepartureList
                └── DepartureForm
```

---

## Conclusion

This specification provides a comprehensive blueprint for implementing the Inventory Management and Availability & Scheduling modules for the TNT Tour & Travel Management System. The modular design ensures scalability, maintainability, and seamless integration with the existing Booking and Itinerary systems.

### Next Steps

1. **Review & Approval**: Stakeholder review of specification
2. **Sprint Planning**: Break down into 2-week sprints
3. **Development**: Follow implementation phases
4. **Testing**: Unit, integration, and UAT
5. **Deployment**: Staged rollout with monitoring
6. **Training**: Admin and staff training
7. **Go Live**: Production deployment with support

---

**Document Control:**
- **Version:** 1.0
- **Last Updated:** April 2, 2026
- **Next Review:** After Phase 3 completion
- **Approval Required From:** Product Owner, Tech Lead, Stakeholders

---

*End of Specification Document*
