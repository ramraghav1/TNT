# Tour & Travel System — Complete Technical Documentation

> **Last Updated:** April 18, 2026
> **Stack:** .NET 8 (C#) + PostgreSQL + Angular 18 (PrimeNG / Sakai)
> **Architecture:** Controller → Service → Repository (Dapper + raw SQL)

---

## Table of Contents

1. [System Overview & Menu Structure](#1-system-overview--menu-structure)
2. [Menu 1 — Itinerary Management](#2-menu-1--itinerary-management)
3. [Menu 2 — Inventory Management (Hotels, Vehicles, Guides, Activities)](#3-menu-2--inventory-management)
4. [Menu 3 — Availability & Calendar](#4-menu-3--availability--calendar)
5. [Menu 4 — Booking Management](#5-menu-4--booking-management)
6. [Menu 5 — Departure Management](#6-menu-5--departure-management)
7. [Menu 6 — Proposals (Itinerary Proposal to Traveler)](#7-menu-6--proposals)
8. [Menu 7 — Itinerary Enhancements (Media, Assignments, Booking Requests, Public API)](#8-menu-7--itinerary-enhancements)
9. [Menu 8 — Pricing & Cost Management](#9-menu-8--pricing--cost-management)
10. [Menu 9 — Finance & Accounting](#10-menu-9--finance--accounting)
11. [Menu 10 — Dashboard](#11-menu-10--dashboard)
12. [Complete Database Table Reference](#12-complete-database-table-reference)
13. [Full API Endpoint Reference](#13-full-api-endpoint-reference)

---

## 1. System Overview & Menu Structure

### Frontend Routes (Angular)

| # | Menu/Feature | Frontend Route | Component |
|---|---|---|---|
| 1 | TNT Dashboard | `/tnt-dashboard` | `TntDashboard` |
| 2 | Add Itinerary | `/add-itinerary` | `AddItinerary` |
| 3 | Itinerary List | `/itinerary-list` | `ItineraryList` |
| 4 | Itinerary Details | `/itinerary-details/:id` | `ItineraryDetailsComponent` |
| 5 | Edit Itinerary | `/edit-itinerary/:id` | `EditItinerary` |
| 6 | Customize Itinerary List | `/customize-itinerary-list` | `CustomizeItineraryList` |
| 7 | Customize Itinerary | `/customize-itinerary/:id` | `CustomizeItinerary` |
| 8 | Booking List | `/booking-list` | `BookingList` |
| 9 | Booking Detail | `/booking-detail/:id` | `BookingDetail` |
| 10 | My Bookings | `/my-bookings` | `MyBookings` |
| 11 | Booking View | `/booking-view/:ref` | `BookingView` |
| 12 | Manage Bookings | `/manage-bookings` | `ManageBookings` |
| 13 | Edit Booking | `/edit-booking/:id` | `EditBooking` |
| 14 | Departure List | `/departure-list` | `DepartureList` |
| 15 | Proposals | `/proposals` | `ProposalList` |
| 16 | Proposal Customize | `/proposal-customize/:itineraryId` | `ProposalCustomize` |
| 17 | Proposal Detail | `/proposal-detail/:id` | `ProposalDetail` |
| 18 | Hotels | `/inventory/hotels` | `HotelListComponent` |
| 19 | Hotel Form | `/inventory/hotels/new` or `/edit/:id` | `HotelFormComponent` |
| 20 | Vehicles | `/inventory/vehicles` | `VehicleListComponent` |
| 21 | Vehicle Form | `/inventory/vehicles/new` or `/edit/:id` | `VehicleFormComponent` |
| 22 | Guides | `/inventory/guides` | `GuideListComponent` |
| 23 | Guide Form | `/inventory/guides/new` or `/edit/:id` | `GuideFormComponent` |
| 24 | Activities | `/inventory/activities` | `ActivityListComponent` |
| 25 | Activity Form | `/inventory/activities/new` or `/edit/:id` | `ActivityFormComponent` |
| 26 | Availability Calendar | `/availability/calendar` | `AvailabilityCalendarComponent` |
| 27 | Finance Summary | `/finance/summary` | `FinanceSummary` |
| 28 | Finance Invoices | `/finance/invoices` | `FinanceInvoiceList` |
| 29 | Finance Expenses | `/finance/expenses` | `ExpenseList` |
| 30 | Finance Commissions | `/finance/commissions` | `CommissionList` |
| 31 | Finance Refunds | `/finance/refunds` | `RefundList` |
| 32 | Tour Website (Public) | `/tour` | Lazy-loaded `tourRoutes` |
| 33 | Proposal Public | `/proposal` | Lazy-loaded `proposalPublicRoutes` |

### Backend Layer Stack

```
Controller (API endpoint)
    ↓
Service (Business logic)  — in Bussiness/Services/TourAndTravels/
    ↓
Repository (Dapper SQL)   — in Repository/Repositories/TourAndTravels/
    ↓
PostgreSQL Database
```

---

## 2. Menu 1 — Itinerary Management

### What It Does
Create and manage tour itinerary **templates** — a multi-day travel plan with day-wise activities, locations, meals, transport, and accommodation details. These templates are reused across multiple bookings.

### Frontend Pages

| Page | Route | Description |
|---|---|---|
| Add Itinerary | `/add-itinerary` | Create a new itinerary template |
| Itinerary List | `/itinerary-list` | View all itineraries |
| Itinerary Details | `/itinerary-details/:id` | View details of a specific itinerary |
| Edit Itinerary | `/edit-itinerary/:id` | Edit an existing itinerary |

### Backend Flow

| Layer | File | Class |
|---|---|---|
| Controller | `server/Controller/TourAndTravels/ItinerariesController.cs` | `ItinerariesController` |
| Service | `Bussiness/Services/TourAndTravels/ItineraryService.cs` | `ItineraryService` |
| Repository | `Repository/Repositories/TourAndTravels/ItineraryRepository.cs` | `ItineraryRepository` |
| Domain Model | `Domain/Models/TourAndTravels/Itinerary.cs` | `Itinerary` |
| DTO | `Repository/DataModels/TourAndTravels/ItineraryDTO.cs` | `ItineraryDTO` |

### API Endpoints

| Method | Route | Controller Method | Description |
|---|---|---|---|
| `POST` | `/api/itineraries/create` | `CreateItinerary` | Create a new itinerary with days & activities |
| `GET` | `/api/itineraries/list` | `GetAllItineraries` | Get all itineraries |
| `GET` | `/api/itineraries/detail?id=` | `GetItineraryById` | Get itinerary by ID with all days/activities |
| `PUT` | `/api/itineraries/update/{id}` | `UpdateItinerary` | Update itinerary, days, and activities |
| `DELETE` | `/api/itineraries/delete/{id}` | `DeleteItinerary` | Delete itinerary and its days |

### Database Tables Affected

#### `POST /api/itineraries/create` — Create Itinerary

| Step | Operation | Table | Description |
|---|---|---|---|
| 1 | `INSERT` | **`itineraries`** | Insert the main itinerary record (title, description, duration, difficulty, group_size_max, etc.) |
| 2 | `INSERT` | **`itinerary_days`** | Insert each day of the itinerary (day_number, title, description, location, meals, transport, accommodation) |
| 3 | `INSERT` | **`itinerary_day_activities`** | Insert activities for each day (activity name, duration, description, sequence_order) |
| 4 | `INSERT` | **`cost_items`** | Upsert cost items referenced by the itinerary (name, category, unit, etc.) |
| 5 | `INSERT` | **`itinerary_day_costs`** | Link cost items to specific itinerary days with quantity and rates |

#### `GET /api/itineraries/list` — Get All Itineraries

| Operation | Table | Description |
|---|---|---|
| `SELECT` | **`itineraries`** | Fetch all itineraries |

#### `GET /api/itineraries/detail?id=` — Get Itinerary by ID

| Step | Operation | Table | Description |
|---|---|---|---|
| 1 | `SELECT` | **`itineraries`** | Fetch the itinerary header |
| 2 | `SELECT` | **`itinerary_days`** | Fetch all days for this itinerary |
| 3 | `SELECT` | **`itinerary_day_activities`** | Fetch activities for each day |
| 4 | `SELECT` | **`itinerary_day_costs`** + JOIN **`cost_items`** | Fetch costs for each day with cost item names |

#### `PUT /api/itineraries/update/{id}` — Update Itinerary

| Step | Operation | Table | Description |
|---|---|---|---|
| 1 | `UPDATE` | **`itineraries`** | Update itinerary header fields |
| 2 | `SELECT` | **`itinerary_days`** | Get existing days to diff |
| 3 | `DELETE` | **`itinerary_day_costs`** | Remove old day costs |
| 4 | `DELETE` | **`itinerary_day_activities`** | Remove old day activities |
| 5 | `DELETE` | **`itinerary_days`** | Remove old days |
| 6 | `INSERT` | **`itinerary_days`** | Re-insert updated days |
| 7 | `INSERT` | **`itinerary_day_activities`** | Re-insert updated activities |
| 8 | `INSERT/UPDATE` | **`cost_items`** | Upsert referenced cost items |
| 9 | `INSERT` | **`itinerary_day_costs`** | Re-insert day costs |

#### `DELETE /api/itineraries/delete/{id}` — Delete Itinerary

| Step | Operation | Table | Description |
|---|---|---|---|
| 1 | `DELETE` | **`itinerary_days`** | Cascade delete all days (triggers delete of activities/costs via FK) |
| 2 | `DELETE` | **`itineraries`** | Delete the itinerary record |

---

## 3. Menu 2 — Inventory Management

### What It Does
Manage the physical resources used in tours: **Hotels** (with rooms), **Vehicles**, **Guides**, and **Activities**. Each has full CRUD + activate/deactivate capabilities.

### Frontend Pages

| Page | Route | Description |
|---|---|---|
| Hotel List | `/inventory/hotels` | List all hotels |
| Hotel Form | `/inventory/hotels/new` or `/edit/:id` | Add/Edit a hotel |
| Vehicle List | `/inventory/vehicles` | List all vehicles |
| Vehicle Form | `/inventory/vehicles/new` or `/edit/:id` | Add/Edit a vehicle |
| Guide List | `/inventory/guides` | List all guides |
| Guide Form | `/inventory/guides/new` or `/edit/:id` | Add/Edit a guide |
| Activity List | `/inventory/activities` | List all activities |
| Activity Form | `/inventory/activities/new` or `/edit/:id` | Add/Edit an activity |

---

### 3A. Hotels

| Layer | File |
|---|---|
| Controller | `server/Controller/TourAndTravels/HotelsController.cs` |
| Service | `Bussiness/Services/TourAndTravels/HotelService.cs` |
| Repository | `Repository/Repositories/TourAndTravels/HotelRepository.cs` |

#### API Endpoints

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/inventory/hotels` | Create hotel with rooms |
| `GET` | `/api/inventory/hotels` | Get all hotels |
| `GET` | `/api/inventory/hotels/{id}` | Get hotel by ID (with rooms) |
| `PUT` | `/api/inventory/hotels/{id}` | Update hotel and rooms |
| `DELETE` | `/api/inventory/hotels/{id}` | Soft delete (deactivate) hotel |
| `POST` | `/api/inventory/hotels/{id}/activate` | Re-activate hotel |

#### Database Tables

| Endpoint | Operation | Table |
|---|---|---|
| `POST` (Create) | `INSERT` | **`hotels`** |
| `POST` (Create) | `INSERT` | **`hotel_rooms`** |
| `GET` (List) | `SELECT` | **`hotels`** |
| `GET` (Detail) | `SELECT` | **`hotels`**, **`hotel_rooms`** |
| `PUT` (Update) | `UPDATE` | **`hotels`** |
| `PUT` (Update) | `DELETE` + `INSERT` | **`hotel_rooms`** (replace all rooms) |
| `DELETE` | `UPDATE` | **`hotels`** (set `is_active = false`) |
| `POST` (Activate) | `UPDATE` | **`hotels`** (set `is_active = true`) |

---

### 3B. Vehicles

| Layer | File |
|---|---|
| Controller | `server/Controller/TourAndTravels/VehiclesController.cs` |
| Service | `Bussiness/Services/TourAndTravels/VehicleService.cs` |
| Repository | `Repository/Repositories/TourAndTravels/VehicleRepository.cs` |

#### API Endpoints

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/inventory/vehicles` | Create vehicle |
| `GET` | `/api/inventory/vehicles` | Get all vehicles |
| `GET` | `/api/inventory/vehicles/{id}` | Get vehicle by ID |
| `PUT` | `/api/inventory/vehicles/{id}` | Update vehicle |
| `DELETE` | `/api/inventory/vehicles/{id}` | Soft delete (deactivate) |
| `POST` | `/api/inventory/vehicles/{id}/activate` | Re-activate |

#### Database Tables

| Endpoint | Operation | Table |
|---|---|---|
| `POST` (Create) | `INSERT` | **`vehicles`** |
| `GET` | `SELECT` | **`vehicles`** |
| `PUT` (Update) | `UPDATE` | **`vehicles`** |
| `DELETE` | `UPDATE` | **`vehicles`** (set `is_active = false`) |
| Activate | `UPDATE` | **`vehicles`** (set `is_active = true`) |

---

### 3C. Guides

| Layer | File |
|---|---|
| Controller | `server/Controller/TourAndTravels/GuidesController.cs` |
| Service | `Bussiness/Services/TourAndTravels/GuideService.cs` |
| Repository | `Repository/Repositories/TourAndTravels/GuideRepository.cs` |

#### API Endpoints

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/inventory/guides` | Create guide |
| `GET` | `/api/inventory/guides` | Get all guides |
| `GET` | `/api/inventory/guides/{id}` | Get guide by ID |
| `PUT` | `/api/inventory/guides/{id}` | Update guide |
| `DELETE` | `/api/inventory/guides/{id}` | Soft delete (deactivate) |
| `POST` | `/api/inventory/guides/{id}/activate` | Re-activate |

#### Database Tables

| Endpoint | Operation | Table |
|---|---|---|
| `POST` (Create) | `INSERT` | **`guides`** |
| `GET` | `SELECT` | **`guides`** |
| `PUT` (Update) | `UPDATE` | **`guides`** |
| `DELETE` | `UPDATE` | **`guides`** (set `is_active = false`) |
| Activate | `UPDATE` | **`guides`** (set `is_active = true`) |

---

### 3D. Activities

| Layer | File |
|---|---|
| Controller | `server/Controller/TourAndTravels/ActivitiesController.cs` |
| Service | `Bussiness/Services/TourAndTravels/ActivityService.cs` |
| Repository | `Repository/Repositories/TourAndTravels/ActivityRepository.cs` |

#### API Endpoints

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/inventory/activities` | Create activity |
| `GET` | `/api/inventory/activities` | Get all activities |
| `GET` | `/api/inventory/activities/{id}` | Get activity by ID |
| `PUT` | `/api/inventory/activities/{id}` | Update activity |
| `DELETE` | `/api/inventory/activities/{id}` | Soft delete (deactivate) |
| `POST` | `/api/inventory/activities/{id}/activate` | Re-activate |

#### Database Tables

| Endpoint | Operation | Table |
|---|---|---|
| `POST` (Create) | `INSERT` | **`activities`** |
| `GET` | `SELECT` | **`activities`** |
| `PUT` (Update) | `UPDATE` | **`activities`** |
| `DELETE` | `UPDATE` | **`activities`** (set `is_active = false`) |
| Activate | `UPDATE` | **`activities`** (set `is_active = true`) |

---

## 4. Menu 3 — Availability & Calendar

### What It Does
Track date-based availability and capacity for inventory items (hotels, vehicles, guides, activities). Supports blocking dates, managing package departures, and linking inventory to bookings.

### Frontend Pages

| Page | Route | Description |
|---|---|---|
| Availability Calendar | `/availability/calendar` | Visual calendar view of availability |

### Backend Flow

| Layer | File |
|---|---|
| Controller | `server/Controller/TourAndTravels/AvailabilityController.cs` |
| Service | `Bussiness/Services/TourAndTravels/AvailabilityService.cs` |
| Repository | `Repository/Repositories/TourAndTravels/AvailabilityRepository.cs` |

### API Endpoints

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/availability/check` | Check availability for a resource on a date range |
| `POST` | `/api/availability/initialize` | Initialize daily availability records for an inventory item |
| `POST` | `/api/availability/blocks` | Block availability for a date range |
| `GET` | `/api/availability/blocks` | Get all blocks (optional date filter) |
| `DELETE` | `/api/availability/blocks/{id}` | Remove a block |
| `POST` | `/api/availability/calendar` | Get calendar view for resources |
| `POST` | `/api/availability/departures` | Create package departure |
| `GET` | `/api/availability/departures` | List departures (optional filter by itinerary) |
| `GET` | `/api/availability/departures/{id}` | Get departure by ID |
| `PUT` | `/api/availability/departures/{id}` | Update departure |
| `DELETE` | `/api/availability/departures/{id}` | Delete departure |
| `POST` | `/api/availability/booking-inventory` | Assign inventory item to booking |
| `GET` | `/api/availability/booking-inventory/{bookingInstanceId}` | Get inventory for a booking |
| `DELETE` | `/api/availability/booking-inventory/{id}` | Remove inventory from booking |

### Database Tables

| Endpoint | Operation | Table(s) |
|---|---|---|
| Check Availability | `SELECT` | **`availability`** |
| Initialize | `INSERT` | **`availability`** (one row per day in range) |
| Create Block | `INSERT` | **`availability_blocks`** |
| Create Block | `UPDATE` | **`availability`** (decrement available capacity) |
| Delete Block | `DELETE` | **`availability_blocks`** |
| Delete Block | `UPDATE` | **`availability`** (restore capacity) |
| Calendar View | `SELECT` | **`itinerary_instances`**, **`itineraries`**, **`itinerary_instance_days`**, **`itinerary_instance_day_assignments`**, **`guides`**, **`vehicles`**, **`hotels`** |
| Create Departure | `INSERT` | **`package_departures`** |
| Get Departures | `SELECT` | **`package_departures`** JOIN **`itineraries`** |
| Update Departure | `UPDATE` | **`package_departures`** |
| Delete Departure | `DELETE` | **`package_departures`** |
| Assign Inventory | `INSERT` | **`booking_inventory`** |
| Assign Inventory | `UPDATE` | **`availability`** (decrement) |
| Get Booking Inventory | `SELECT` | **`booking_inventory`** |
| Remove Inventory | `DELETE` | **`booking_inventory`** |

---

## 5. Menu 4 — Booking Management

### What It Does
Create and manage individual bookings (instances) of itinerary templates. Each booking is a concrete trip for a specific traveler/group with specific dates, customized days, travelers, payments, approvals, and inventory.

### Frontend Pages

| Page | Route | Description |
|---|---|---|
| Booking List | `/booking-list` | All bookings |
| Booking Detail | `/booking-detail/:id` | Full detail of a booking instance |
| My Bookings | `/my-bookings` | Bookings for current user |
| Booking View | `/booking-view/:ref` | View booking by reference |
| Manage Bookings | `/manage-bookings` | Admin booking management |
| Edit Booking | `/edit-booking/:id` | Edit a booking |

### Backend Flow

| Layer | File |
|---|---|
| Controller | `server/Controller/TourAndTravels/BookingsController.cs` |
| Service | `Bussiness/Services/TourAndTravels/BookingService.cs` |
| Repository | `Repository/Repositories/TourAndTravels/BookingRepository.cs` |

### API Endpoints

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/bookings/create` | Create a booking from template or reuse existing instance |
| `GET` | `/api/bookings` | Get all bookings (lightweight list) |
| `GET` | `/api/bookings/dashboard-stats` | Dashboard statistics |
| `GET` | `/api/bookings/{id}` | Get full booking detail |
| `PUT` | `/api/bookings/{id}/customize-day` | Customize a single day |
| `POST` | `/api/bookings/{id}/approve` | Approve booking (traveler or admin) |
| `POST` | `/api/bookings/{id}/payment` | Add payment |
| `GET` | `/api/bookings/{id}/payments` | Get all payments for booking |
| `PUT` | `/api/bookings/{id}/status` | Update booking status |
| `PUT` | `/api/bookings/{id}` | Full booking update (header + travelers + days) |
| `POST` | `/api/bookings/{id}/inventory` | Assign inventory to booking |
| `GET` | `/api/bookings/{id}/inventory` | Get inventory assigned to booking |
| `DELETE` | `/api/bookings/inventory/{itemId}` | Remove inventory from booking |

### Database Tables — Step by Step

#### `POST /api/bookings/create` — Create Booking

**Option A: From Template (itineraryId only)**

| Step | Operation | Table | Description |
|---|---|---|---|
| 1 | `INSERT` | **`itinerary_instances`** | Create instance record (booking_reference, start_date, end_date, status, total_amount, etc.) |
| 2 | `INSERT` | **`travelers`** | Insert each traveler (name, email, phone, passport, nationality, etc.) |
| 3 | `SELECT` | **`itinerary_days`** | Load template days |
| 4 | `INSERT` | **`itinerary_instance_days`** | Copy each template day into instance days (with actual dates) |
| 5 | `SELECT` | **`itinerary_day_activities`** | Load template day activities |
| 6 | `INSERT` | **`itinerary_instance_day_activities`** | Copy activities into instance day activities |

**Option B: From Existing Instance (sourceInstanceId)**

| Step | Operation | Table | Description |
|---|---|---|---|
| 1 | `INSERT` | **`itinerary_instances`** | Create new instance based on source |
| 2 | `INSERT` | **`travelers`** | Insert travelers |
| 3 | `SELECT` | **`itinerary_instance_days`** | Load source instance days |
| 4 | `INSERT` | **`itinerary_instance_days`** | Copy days to new instance |
| 5 | `SELECT` | **`itinerary_instance_day_activities`** | Load source day activities |
| 6 | `INSERT` | **`itinerary_instance_day_activities`** | Copy activities |

#### `GET /api/bookings` — List All Bookings

| Operation | Table(s) |
|---|---|
| `SELECT` | **`itinerary_instances`** JOIN **`itineraries`** + lateral join on **`travelers`** |

#### `GET /api/bookings/{id}` — Get Full Booking Detail

| Step | Operation | Table |
|---|---|---|
| 1 | `SELECT` | **`itinerary_instances`** JOIN **`itineraries`** |
| 2 | `SELECT` | **`itinerary_instance_days`** |
| 3 | `SELECT` | **`itinerary_instance_day_activities`** |
| 4 | `SELECT` | **`travelers`** |
| 5 | `SELECT` | **`payments`** |

#### `PUT /api/bookings/{id}/customize-day` — Customize Day

| Step | Operation | Table |
|---|---|---|
| 1 | `SELECT` | **`itinerary_instance_days`** (verify day exists) |
| 2 | `UPDATE` | **`itinerary_instance_days`** (update title, location, accommodation, transport, meals) |
| 3 | `DELETE` | **`itinerary_instance_day_activities`** (remove old activities) |
| 4 | `INSERT` | **`itinerary_instance_day_activities`** (insert new activities) |

#### `POST /api/bookings/{id}/approve` — Approve Booking

| Step | Operation | Table |
|---|---|---|
| 1 | `INSERT` | **`itinerary_approvals`** (record approval with approvedBy, approved, remarks) |
| 2 | `UPDATE` | **`itinerary_instances`** (set traveler_approved / admin_approved flags) |
| 3 | `SELECT` | **`itinerary_instances`** (check if both approved) |
| 4 | `UPDATE` | **`itinerary_instances`** (set status = 'Confirmed' if both approved) |

#### `POST /api/bookings/{id}/payment` — Add Payment

| Step | Operation | Table |
|---|---|---|
| 1 | `INSERT` | **`payments`** (amount, currency, payment_method, transaction_reference, etc.) |
| 2 | `SELECT` | **`payments`** (sum all payments for booking) |
| 3 | `SELECT` | **`itinerary_instances`** (get total_amount) |
| 4 | `UPDATE` | **`itinerary_instances`** (update payment_status: Unpaid/Partial/Paid) |

#### `PUT /api/bookings/{id}/status` — Update Status

| Operation | Table |
|---|---|
| `UPDATE` | **`itinerary_instances`** (set status = Draft/Pending/Confirmed/Cancelled) |

#### `PUT /api/bookings/{id}` — Full Update Booking

| Step | Operation | Table |
|---|---|---|
| 1 | `UPDATE` | **`itinerary_instances`** (header: start_date, end_date, special_requests, total_amount) |
| 2 | `DELETE` | **`travelers`** (remove existing travelers for this instance) |
| 3 | `INSERT` | **`travelers`** (re-insert updated travelers) |
| 4 | `UPDATE` | **`itinerary_instance_days`** (update each day) |
| 5 | `DELETE` | **`itinerary_instance_day_activities`** (remove old activities per day) |
| 6 | `INSERT` | **`itinerary_instance_day_activities`** (re-insert activities per day) |

#### `POST /api/bookings/{id}/inventory` — Assign Inventory

| Operation | Table |
|---|---|
| `INSERT` | **`booking_inventory`** |

#### `DELETE /api/bookings/inventory/{itemId}` — Remove Inventory

| Step | Operation | Table |
|---|---|---|
| 1 | `SELECT` | **`booking_inventory`** (get item details) |
| 2 | `DELETE` | **`booking_inventory`** |
| 3 | `UPDATE` | **`itinerary_instances`** (recalculate inventory_cost) |

---

## 6. Menu 5 — Departure Management

### What It Does
Group multiple bookings into a single **departure** — a physical group trip with a shared guide, vehicle, and date. Allows creating new departures or assigning bookings to existing ones.

### Frontend Pages

| Page | Route | Description |
|---|---|---|
| Departure List | `/departure-list` | View all departures with travelers/guide/vehicle info |

### Backend Flow

| Layer | File |
|---|---|
| Controller | `server/Controller/TourAndTravels/DepartureManagementController.cs` |
| Service | `Bussiness/Services/TourAndTravels/DepartureManagementService.cs` |
| Repository | `Repository/Repositories/TourAndTravels/DepartureManagementRepository.cs` |

### API Endpoints

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/departure-management/unassigned-bookings` | Get bookings not yet assigned to a departure |
| `GET` | `/api/departure-management/suggest` | Suggest existing departures for a given itinerary + seat count |
| `POST` | `/api/departure-management/create` | Create new departure and assign bookings |
| `POST` | `/api/departure-management/assign` | Assign bookings to existing departure |
| `GET` | `/api/departure-management/{id}` | Get departure detail |
| `GET` | `/api/departure-management/all` | Get all departures with guide/vehicle/traveler info |

### Database Tables

#### `GET .../unassigned-bookings`
| Operation | Table(s) |
|---|---|
| `SELECT` | **`itinerary_instances`** JOIN **`itineraries`** + lateral **`travelers`** (WHERE `departure_id IS NULL`) |

#### `GET .../suggest`
| Operation | Table(s) |
|---|---|
| `SELECT` | **`package_departures`** JOIN **`itineraries`**, **`guides`**, **`vehicles`** (WHERE available capacity >= required seats) |

#### `POST .../create` — Create Departure
| Step | Operation | Table |
|---|---|---|
| 1 | `SELECT` | **`itinerary_instances`** (validate selected bookings) |
| 2 | `INSERT` | **`package_departures`** (departure_date, guide_id, vehicle_id, max_travelers, notes) |
| 3 | `UPDATE` | **`itinerary_instances`** (set `departure_id` on each selected booking) |

#### `POST .../assign` — Assign to Existing Departure
| Step | Operation | Table |
|---|---|---|
| 1 | `SELECT` | **`package_departures`** (verify departure exists) |
| 2 | `SELECT` | **`itinerary_instances`** (count already-assigned travelers) |
| 3 | `UPDATE` | **`itinerary_instances`** (set `departure_id` on selected bookings) |
| 4 | `UPDATE` | **`package_departures`** (recalculate totals) |

#### `GET .../all` — All Departures
| Operation | Table(s) |
|---|---|
| `SELECT` | **`package_departures`** JOIN **`itineraries`**, **`itinerary_instances`**, **`travelers`**, **`guides`**, **`vehicles`** |

---

## 7. Menu 6 — Proposals

### What It Does
Send a customized itinerary proposal to a traveler via email. The traveler can view the proposal via a public link (token-based), accept/reject it, submit payments, provide feedback. Admins can verify payments and resend proposals.

### Frontend Pages

| Page | Route | Description |
|---|---|---|
| Proposal List | `/proposals` | Admin: view all proposals sent |
| Proposal Customize | `/proposal-customize/:itineraryId` | Admin: customize itinerary and compose proposal |
| Proposal Detail | `/proposal-detail/:id` | Admin: view proposal details, payments, feedback |
| Public Proposal | `/proposal/:token` | Traveler: view/accept/reject/pay (public, no auth) |

### Backend Flow

| Layer | File |
|---|---|
| Controller | `server/Controller/TourAndTravels/ProposalsController.cs` |
| Service | `Bussiness/Services/TourAndTravels/ItineraryProposalService.cs` |
| Repository | `Repository/Repositories/TourAndTravels/ItineraryProposalRepository.cs` |

### API Endpoints

| Method | Route | Auth | Description |
|---|---|---|---|
| `POST` | `/api/proposals/create` | Yes | Create proposal + send email |
| `GET` | `/api/proposals` | Yes | List all proposals |
| `GET` | `/api/proposals/{id}` | Yes | Get proposal by ID (admin) |
| `GET` | `/api/proposals/public/{token}` | No | Get proposal by token (public) |
| `POST` | `/api/proposals/public/{token}/accept` | No | Traveler accepts proposal |
| `POST` | `/api/proposals/public/{token}/reject` | No | Traveler rejects proposal |
| `POST` | `/api/proposals/public/{token}/payment` | No | Traveler submits payment |
| `POST` | `/api/proposals/public/payment/{paymentId}/screenshot` | No | Upload payment screenshot |
| `GET` | `/api/proposals/{id}/payments` | Yes | Get payments for proposal (admin) |
| `GET` | `/api/proposals/public/{token}/payments` | No | Get payments by token (public) |
| `POST` | `/api/proposals/payment/{paymentId}/verify` | Yes | Admin verifies payment |
| `POST` | `/api/proposals/public/{token}/feedback` | No | Traveler submits feedback |
| `GET` | `/api/proposals/{id}/feedback` | Yes | Get feedback (admin) |
| `PUT` | `/api/proposals/{id}/resend` | Yes | Re-customize and resend proposal |
| `GET` | `/api/proposals/public/{token}/invoice` | No | Get invoice for confirmed proposal |
| `GET` | `/api/proposals/test-email` | No | Test email sending |

### Database Tables

#### `POST /api/proposals/create`
| Step | Operation | Table |
|---|---|---|
| 1 | `INSERT` | **`itinerary_proposals`** (itinerary_id, traveler_name, traveler_email, traveler_phone, message, token, status, total_price, currency, valid_until, etc.) |
| 2 | `INSERT` | **`itinerary_proposal_days`** (day_number, title, description, meals, transport, accommodation, activities) |

#### `GET /api/proposals` — List
| Operation | Table(s) |
|---|---|
| `SELECT` | **`itinerary_proposals`** JOIN **`itineraries`** |

#### `GET /api/proposals/{id}` or `GET .../public/{token}` — Detail
| Step | Operation | Table(s) |
|---|---|---|
| 1 | `SELECT` | **`itinerary_proposals`** JOIN **`itineraries`** |
| 2 | `SELECT` | **`itinerary_proposal_days`** |

#### `POST .../public/{token}/accept` — Accept
| Step | Operation | Table |
|---|---|---|
| 1 | `SELECT` | **`itinerary_proposals`** (find by token) |
| 2 | `UPDATE` | **`itinerary_proposals`** (set status = 'Accepted') |

#### `POST .../public/{token}/reject` — Reject
| Operation | Table |
|---|---|
| `UPDATE` | **`itinerary_proposals`** (set status = 'Rejected') |

#### `POST .../public/{token}/payment` — Submit Payment
| Step | Operation | Table |
|---|---|---|
| 1 | `INSERT` | **`itinerary_proposal_payments`** (proposal_id, amount, currency, payment_method, transaction_reference, status) |
| 2 | `UPDATE` | **`itinerary_proposals`** (update payment_status based on total paid vs total_price) |

#### `POST .../payment/{paymentId}/verify` — Verify Payment
| Step | Operation | Table |
|---|---|---|
| 1 | `UPDATE` | **`itinerary_proposal_payments`** (set verified_by, verified_at, status) |
| 2 | `UPDATE` | **`itinerary_proposals`** (recalculate payment_status) |
| 3 | `UPDATE` | **`itinerary_proposals`** (set status = 'Confirmed' if fully paid) |

#### `POST .../public/{token}/feedback` — Submit Feedback
| Step | Operation | Table |
|---|---|---|
| 1 | `INSERT` | **`itinerary_proposal_feedback`** (proposal_id, rating, comment, feedback_type) |
| 2 | `UPDATE` | **`itinerary_proposals`** (update has_feedback, last_feedback_at) |

#### `PUT /api/proposals/{id}/resend` — Resend
| Step | Operation | Table |
|---|---|---|
| 1 | `UPDATE` | **`itinerary_proposals`** (update traveler info, message, total_price, etc.) |
| 2 | `DELETE` | **`itinerary_proposal_days`** (remove old days) |
| 3 | `INSERT` | **`itinerary_proposal_days`** (insert updated days) |

---

## 8. Menu 7 — Itinerary Enhancements

### What It Does
Adds media management (photos, videos), resource assignments (hotel/guide/vehicle per day), availability checking, booking requests from the public website, and a public API for the tour website.

### Frontend Pages

| Page | Component/Feature |
|---|---|
| Media Gallery | `media-gallery/` inside itinerary pages |
| Customize Itinerary List | `/customize-itinerary-list` |
| Customize Itinerary | `/customize-itinerary/:id` |
| Public Tour Website | `/tour` (lazy-loaded) |

### Backend Flow

| Layer | File |
|---|---|
| Controller | `server/Controller/TourAndTravels/ItineraryEnhancementsController.cs` |
| Service | `Bussiness/Services/TourAndTravels/ItineraryEnhancementsService.cs` |
| Repository | `Repository/Repositories/TourAndTravels/ItineraryEnhancementsRepository.cs` |

### API Endpoints

| Method | Route | Auth | Description |
|---|---|---|---|
| **Media** | | | |
| `POST` | `/api/inventory/itinerary-enhancements/itineraries/{itineraryId}/media` | Yes | Add media |
| `GET` | `/api/inventory/itinerary-enhancements/itineraries/{itineraryId}/media` | No | Get media |
| `DELETE` | `/api/inventory/itinerary-enhancements/media/{mediaId}` | Yes | Delete media |
| `PUT` | `/api/inventory/itinerary-enhancements/itineraries/{itineraryId}/media/reorder` | Yes | Reorder media |
| **Assignments** | | | |
| `POST` | `/api/inventory/itinerary-enhancements/bookings/create-with-assignments` | Yes | Create booking with hotel/guide/vehicle assignments |
| `POST` | `/api/inventory/itinerary-enhancements/assignments` | Yes | Add assignment to existing booking day |
| `GET` | `/api/inventory/itinerary-enhancements/instances/{instanceId}/assignments` | Yes | Get assignments for instance |
| `PUT` | `/api/inventory/itinerary-enhancements/assignments/{assignmentId}` | Yes | Update assignment |
| `DELETE` | `/api/inventory/itinerary-enhancements/assignments/{assignmentId}` | Yes | Delete assignment |
| **Availability** | | | |
| `POST` | `/api/inventory/itinerary-enhancements/check-availability` | Yes | Check hotel/guide/vehicle availability |
| `POST` | `/api/inventory/itinerary-enhancements/instances/{instanceId}/validate-assignments` | Yes | Validate assignments for conflicts |
| **Booking Requests** | | | |
| `POST` | `/api/inventory/itinerary-enhancements/booking-requests` | No | Public: submit booking request |
| `GET` | `/api/inventory/itinerary-enhancements/booking-requests` | Yes | Admin: list requests |
| `GET` | `/api/inventory/itinerary-enhancements/booking-requests/{id}` | Yes | Admin: get request |
| `PUT` | `/api/inventory/itinerary-enhancements/booking-requests/{id}/status` | Yes | Admin: update status |
| `POST` | `/api/inventory/itinerary-enhancements/booking-requests/{requestId}/convert` | Yes | Convert request to booking |
| **Public Website API** | | | |
| `GET` | `/api/inventory/itinerary-enhancements/public/itineraries` | No | Public: published itineraries |
| `GET` | `/api/inventory/itinerary-enhancements/public/itineraries/{itineraryId}` | No | Public: itinerary detail |

### Database Tables

| Feature | Operation | Table(s) |
|---|---|---|
| Add Media | `INSERT` | **`itinerary_media`** |
| Get Media | `SELECT` | **`itinerary_media`** |
| Delete Media | `DELETE` | **`itinerary_media`** |
| Reorder Media | `UPDATE` | **`itinerary_media`** |
| Create Assignment | `INSERT` | **`itinerary_instance_day_assignments`** |
| Get Assignments | `SELECT` | **`itinerary_instance_day_assignments`** JOIN **`itinerary_instance_days`**, **`hotels`**, **`guides`**, **`vehicles`** |
| Update Assignment | `UPDATE` | **`itinerary_instance_day_assignments`** |
| Delete Assignment | `DELETE` | **`itinerary_instance_day_assignments`** |
| Check Availability | `SELECT` | **`itinerary_instance_day_assignments`** JOIN **`itinerary_instance_days`**, **`itinerary_instances`** + **`hotels`**, **`guides`**, **`vehicles`** |
| Create Booking Request | `INSERT` | **`booking_requests`** |
| Get Requests | `SELECT` | **`booking_requests`** JOIN **`itineraries`** |
| Update Request Status | `UPDATE` | **`booking_requests`** |
| Convert to Booking | `UPDATE` | **`booking_requests`** + creates booking via booking service |
| Public Itineraries | `SELECT` | **`itineraries`** (WHERE is_published = true) |
| Public Itinerary Detail | `SELECT` | **`itineraries`**, **`itinerary_days`**, **`itinerary_day_activities`**, **`itinerary_media`** |

---

## 9. Menu 8 — Pricing & Cost Management

### What It Does
Manage cost items (transport, accommodation, food, etc.), set rates per cost item, assign costs to itinerary days, and calculate booking-level pricing.

### Frontend Page
Integrated into itinerary and booking detail pages.

### Backend Flow

| Layer | File |
|---|---|
| Controller | `server/Controller/TourAndTravels/PricingController.cs` |
| Service | `Bussiness/Services/TourAndTravels/PricingService.cs` |
| Repository | `Repository/Repositories/TourAndTravels/PricingRepository.cs` |

### API Endpoints

| Method | Route | Description |
|---|---|---|
| **Cost Items** | | |
| `POST` | `/api/pricing/cost-items` | Create cost item |
| `GET` | `/api/pricing/cost-items` | List all cost items |
| `PUT` | `/api/pricing/cost-items/{id}` | Update cost item |
| `DELETE` | `/api/pricing/cost-items/{id}` | Delete cost item |
| **Cost Item Rates** | | |
| `POST` | `/api/pricing/rates` | Create rate for a cost item |
| `GET` | `/api/pricing/rates/cost-item/{costItemId}` | Get rates by cost item |
| `GET` | `/api/pricing/rates/itinerary/{itineraryId}` | Get rates by itinerary |
| **Day Costs (Template)** | | |
| `POST` | `/api/pricing/day-costs` | Assign cost to a template day |
| `GET` | `/api/pricing/day-costs/day/{dayId}` | Get costs for a day |
| `GET` | `/api/pricing/day-costs/itinerary/{itineraryId}` | Get all costs for an itinerary |
| `DELETE` | `/api/pricing/day-costs/{id}` | Remove day cost |
| **Booking Pricing** | | |
| `GET` | `/api/pricing/booking/{instanceId}` | Get full pricing for a booking instance |

### Database Tables

| Endpoint | Operation | Table(s) |
|---|---|---|
| Create Cost Item | `INSERT` | **`cost_items`** |
| List Cost Items | `SELECT` | **`cost_items`** |
| Update Cost Item | `UPDATE` | **`cost_items`** |
| Delete Cost Item | `DELETE` | **`cost_items`** |
| Create Rate | `INSERT` | **`cost_item_rates`** |
| Get Rates | `SELECT` | **`cost_item_rates`** JOIN **`cost_items`** |
| Assign Day Cost | `INSERT` | **`itinerary_day_costs`** |
| Get Day Costs | `SELECT` | **`itinerary_day_costs`** JOIN **`cost_items`** + lateral **`cost_item_rates`** |
| Remove Day Cost | `DELETE` | **`itinerary_day_costs`** |
| Booking Pricing | `SELECT` | **`itinerary_instances`** → **`itinerary_instance_day_costs`** JOIN **`itinerary_instance_days`**, **`cost_items`** |

---

## 10. Menu 9 — Finance & Accounting

### What It Does
Complete financial management: tax configuration, invoices (auto-generated from bookings or manual), agent commissions, expense tracking, and refund processing.

### Frontend Pages

| Page | Route | Description |
|---|---|---|
| Finance Summary | `/finance/summary` | Dashboard: totals, charts |
| Invoice List | `/finance/invoices` | View/manage invoices |
| Expense List | `/finance/expenses` | Track expenses |
| Commission List | `/finance/commissions` | Agent commissions |
| Refund List | `/finance/refunds` | Process refunds |

### Backend Flow

| Layer | File |
|---|---|
| Controller | `server/Controller/TourAndTravels/FinanceController.cs` |
| Service | `Bussiness/Services/TourAndTravels/FinanceService.cs` |
| Repository | `Repository/Repositories/TourAndTravels/FinanceRepository.cs` |

### API Endpoints

| Method | Route | Description |
|---|---|---|
| **Summary** | | |
| `GET` | `/api/finance/summary` | Finance dashboard summary |
| **Tax Config** | | |
| `GET` | `/api/finance/tax-config` | All tax configs |
| `GET` | `/api/finance/tax-config/active` | Active tax config |
| `POST` | `/api/finance/tax-config` | Create tax config |
| `PUT` | `/api/finance/tax-config/{id}` | Update tax config |
| **Invoices** | | |
| `POST` | `/api/finance/invoices/generate/{bookingId}` | Auto-generate invoice from booking |
| `POST` | `/api/finance/invoices` | Create manual invoice |
| `GET` | `/api/finance/invoices` | List invoices (paginated, filterable) |
| `GET` | `/api/finance/invoices/{id}` | Get invoice detail |
| `PATCH` | `/api/finance/invoices/{id}/status` | Update invoice status |
| **Commissions** | | |
| `POST` | `/api/finance/commissions` | Create commission |
| `GET` | `/api/finance/commissions` | List commissions |
| `GET` | `/api/finance/commissions/{id}` | Get commission |
| `GET` | `/api/finance/commissions/booking/{bookingId}` | Commissions by booking |
| `PATCH` | `/api/finance/commissions/{id}/status` | Update commission status |
| **Expenses** | | |
| `POST` | `/api/finance/expenses` | Create expense |
| `GET` | `/api/finance/expenses` | List expenses |
| `GET` | `/api/finance/expenses/{id}` | Get expense |
| `PUT` | `/api/finance/expenses/{id}` | Update expense |
| `DELETE` | `/api/finance/expenses/{id}` | Delete expense |
| **Refunds** | | |
| `POST` | `/api/finance/refunds` | Create refund |
| `GET` | `/api/finance/refunds` | List refunds |
| `GET` | `/api/finance/refunds/{id}` | Get refund |
| `PATCH` | `/api/finance/refunds/{id}/process` | Process refund |

### Database Tables

| Feature | Operation | Table(s) |
|---|---|---|
| Tax Config CRUD | `INSERT/SELECT/UPDATE` | **`tnt_tax_configs`** |
| Create Invoice | `INSERT` | **`tnt_invoices`** |
| Create Invoice | `INSERT` | **`tnt_invoice_line_items`** |
| Generate from Booking | `SELECT` | **`itinerary_instances`** (to get booking details) → `INSERT` **`tnt_invoices`** + **`tnt_invoice_line_items`** |
| List/Get Invoices | `SELECT` | **`tnt_invoices`** JOIN **`itinerary_instances`** |
| Get Invoice Line Items | `SELECT` | **`tnt_invoice_line_items`** |
| Update Invoice Status | `UPDATE` | **`tnt_invoices`** |
| Create Commission | `INSERT` | **`tnt_agent_commissions`** |
| List/Get Commissions | `SELECT` | **`tnt_agent_commissions`** JOIN **`itinerary_instances`** |
| Update Commission | `UPDATE` | **`tnt_agent_commissions`** |
| Create Expense | `INSERT` | **`tnt_expenses`** |
| List/Get Expenses | `SELECT` | **`tnt_expenses`** JOIN **`itinerary_instances`** |
| Update Expense | `UPDATE` | **`tnt_expenses`** |
| Delete Expense | `DELETE` | **`tnt_expenses`** |
| Create Refund | `INSERT` | **`tnt_refunds`** |
| List/Get Refunds | `SELECT` | **`tnt_refunds`** JOIN **`itinerary_instances`** |
| Process Refund | `UPDATE` | **`tnt_refunds`** |
| Finance Summary | `SELECT` | **`tnt_invoices`**, **`tnt_expenses`**, **`tnt_agent_commissions`**, **`tnt_refunds`** (aggregates) |

---

## 11. Menu 10 — Dashboard

### What It Does
Show key statistics for the tour and travel business.

### Frontend
| Page | Route | Component |
|---|---|---|
| TNT Dashboard | `/tnt-dashboard` | `TntDashboard` |

### API Endpoint
| Method | Route | Description |
|---|---|---|
| `GET` | `/api/bookings/dashboard-stats` | Booking statistics |

### Database Tables
| Operation | Table(s) |
|---|---|
| `SELECT` (counts) | **`itinerary_instances`** |
| `SELECT` (counts) | **`itineraries`** |
| `SELECT` (counts) | **`travelers`** |
| `SELECT` (sums) | **`payments`** |
| `SELECT` (counts) | **`itinerary_instances`** JOIN **`itineraries`** (by status, by itinerary) |

---

## 12. Complete Database Table Reference

### Core Tables

| # | Table Name | Purpose | Main Operations |
|---|---|---|---|
| 1 | **`itineraries`** | Tour itinerary templates (master) | CRUD |
| 2 | **`itinerary_days`** | Days within an itinerary template | INSERT/DELETE with parent |
| 3 | **`itinerary_day_activities`** | Activities per template day | INSERT/DELETE with parent |
| 4 | **`itinerary_day_costs`** | Cost assignments per template day | INSERT/DELETE |

### Booking Tables

| # | Table Name | Purpose | Main Operations |
|---|---|---|---|
| 5 | **`itinerary_instances`** | Concrete booking instances | CRUD + status updates |
| 6 | **`itinerary_instance_days`** | Days within a booking (copy of template, editable) | INSERT/UPDATE |
| 7 | **`itinerary_instance_day_activities`** | Activities per booking day | INSERT/DELETE |
| 8 | **`itinerary_instance_day_assignments`** | Hotel/Guide/Vehicle assigned per booking day | CRUD |
| 9 | **`itinerary_instance_day_costs`** | Costs per booking instance day | SELECT |
| 10 | **`travelers`** | Traveler info linked to booking instances | INSERT/DELETE |
| 11 | **`payments`** | Payment records for bookings | INSERT/SELECT |
| 12 | **`itinerary_approvals`** | Approval records (traveler + admin) | INSERT |
| 13 | **`booking_inventory`** | Inventory items assigned to bookings | INSERT/DELETE |

### Inventory Tables

| # | Table Name | Purpose | Main Operations |
|---|---|---|---|
| 14 | **`hotels`** | Hotel inventory | CRUD + soft delete |
| 15 | **`hotel_rooms`** | Rooms per hotel | INSERT/DELETE (replaced on update) |
| 16 | **`vehicles`** | Vehicle inventory | CRUD + soft delete |
| 17 | **`guides`** | Guide inventory | CRUD + soft delete |
| 18 | **`activities`** | Activity inventory | CRUD + soft delete |

### Availability Tables

| # | Table Name | Purpose | Main Operations |
|---|---|---|---|
| 19 | **`availability`** | Daily capacity records per resource | INSERT/UPDATE |
| 20 | **`availability_blocks`** | Date-range blocks on resources | INSERT/DELETE |
| 21 | **`package_departures`** | Group departures linking bookings | CRUD |

### Pricing Tables

| # | Table Name | Purpose | Main Operations |
|---|---|---|---|
| 22 | **`cost_items`** | Cost item definitions (transport, food, etc.) | CRUD |
| 23 | **`cost_item_rates`** | Rates per cost item (date-dependent) | INSERT/SELECT |

### Proposal Tables

| # | Table Name | Purpose | Main Operations |
|---|---|---|---|
| 24 | **`itinerary_proposals`** | Proposals sent to travelers | CRUD + status updates |
| 25 | **`itinerary_proposal_days`** | Days within a proposal | INSERT/DELETE |
| 26 | **`itinerary_proposal_payments`** | Payments on proposals | INSERT/UPDATE |
| 27 | **`itinerary_proposal_feedback`** | Traveler feedback on proposals | INSERT |

### Finance Tables

| # | Table Name | Purpose | Main Operations |
|---|---|---|---|
| 28 | **`tnt_tax_configs`** | Tax configuration (VAT, service tax) | CRUD |
| 29 | **`tnt_invoices`** | Invoices generated for bookings | CRUD |
| 30 | **`tnt_invoice_line_items`** | Line items within invoices | INSERT/SELECT |
| 31 | **`tnt_agent_commissions`** | Agent commission records | CRUD |
| 32 | **`tnt_expenses`** | Expense tracking | CRUD |
| 33 | **`tnt_refunds`** | Refund records | CRUD |

### Enhancement Tables

| # | Table Name | Purpose | Main Operations |
|---|---|---|---|
| 34 | **`itinerary_media`** | Photos/videos for itineraries | CRUD |
| 35 | **`booking_requests`** | Booking requests from public website | CRUD |

---

## 13. Full API Endpoint Reference

### Itineraries (`/api/itineraries`)
```
POST   /api/itineraries/create          → CreateItinerary
GET    /api/itineraries/list            → GetAllItineraries
GET    /api/itineraries/detail?id=      → GetItineraryById
PUT    /api/itineraries/update/{id}     → UpdateItinerary
DELETE /api/itineraries/delete/{id}     → DeleteItinerary
```

### Inventory — Hotels (`/api/inventory/hotels`)
```
POST   /api/inventory/hotels            → CreateHotel
GET    /api/inventory/hotels            → GetAllHotels
GET    /api/inventory/hotels/{id}       → GetHotelById
PUT    /api/inventory/hotels/{id}       → UpdateHotel
DELETE /api/inventory/hotels/{id}       → DeleteHotel (soft)
POST   /api/inventory/hotels/{id}/activate → ActivateHotel
```

### Inventory — Vehicles (`/api/inventory/vehicles`)
```
POST   /api/inventory/vehicles          → CreateVehicle
GET    /api/inventory/vehicles          → GetAllVehicles
GET    /api/inventory/vehicles/{id}     → GetVehicleById
PUT    /api/inventory/vehicles/{id}     → UpdateVehicle
DELETE /api/inventory/vehicles/{id}     → DeleteVehicle (soft)
POST   /api/inventory/vehicles/{id}/activate → ActivateVehicle
```

### Inventory — Guides (`/api/inventory/guides`)
```
POST   /api/inventory/guides            → CreateGuide
GET    /api/inventory/guides            → GetAllGuides
GET    /api/inventory/guides/{id}       → GetGuideById
PUT    /api/inventory/guides/{id}       → UpdateGuide
DELETE /api/inventory/guides/{id}       → DeleteGuide (soft)
POST   /api/inventory/guides/{id}/activate → ActivateGuide
```

### Inventory — Activities (`/api/inventory/activities`)
```
POST   /api/inventory/activities        → CreateActivity
GET    /api/inventory/activities        → GetAllActivities
GET    /api/inventory/activities/{id}   → GetActivityById
PUT    /api/inventory/activities/{id}   → UpdateActivity
DELETE /api/inventory/activities/{id}   → DeleteActivity (soft)
POST   /api/inventory/activities/{id}/activate → ActivateActivity
```

### Availability (`/api/availability`)
```
POST   /api/availability/check                          → CheckAvailability
POST   /api/availability/initialize                     → InitializeAvailability
POST   /api/availability/blocks                         → CreateBlock
GET    /api/availability/blocks                         → GetBlocks
DELETE /api/availability/blocks/{id}                    → DeleteBlock
POST   /api/availability/calendar                       → GetCalendarView
POST   /api/availability/departures                     → CreatePackageDeparture
GET    /api/availability/departures                     → GetPackageDepartures
GET    /api/availability/departures/{id}                → GetPackageDepartureById
PUT    /api/availability/departures/{id}                → UpdatePackageDeparture
DELETE /api/availability/departures/{id}                → DeletePackageDeparture
POST   /api/availability/booking-inventory              → AssignInventoryToBooking
GET    /api/availability/booking-inventory/{instanceId} → GetBookingInventory
DELETE /api/availability/booking-inventory/{id}         → RemoveInventoryFromBooking
```

### Bookings (`/api/bookings`)
```
POST   /api/bookings/create                → CreateBooking
GET    /api/bookings                       → GetAllBookings
GET    /api/bookings/dashboard-stats       → GetDashboardStats
GET    /api/bookings/{id}                  → GetBookingById
PUT    /api/bookings/{id}/customize-day    → CustomizeDay
POST   /api/bookings/{id}/approve          → ApproveBooking
POST   /api/bookings/{id}/payment          → AddPayment
GET    /api/bookings/{id}/payments         → GetPayments
PUT    /api/bookings/{id}/status           → UpdateStatus
PUT    /api/bookings/{id}                  → UpdateBooking (full)
POST   /api/bookings/{id}/inventory        → AssignInventory
GET    /api/bookings/{id}/inventory        → GetBookingInventory
DELETE /api/bookings/inventory/{itemId}    → RemoveInventoryItem
```

### Departure Management (`/api/departure-management`)
```
GET    /api/departure-management/unassigned-bookings → GetUnassignedBookings
GET    /api/departure-management/suggest             → SuggestDepartures
POST   /api/departure-management/create              → CreateDeparture
POST   /api/departure-management/assign              → AssignToDeparture
GET    /api/departure-management/{id}                → GetDepartureById
GET    /api/departure-management/all                 → GetAllDepartures
```

### Proposals (`/api/proposals`)
```
POST   /api/proposals/create                           → CreateProposal
GET    /api/proposals                                  → GetAll
GET    /api/proposals/{id}                             → GetById
GET    /api/proposals/public/{token}                   → GetByToken (public)
POST   /api/proposals/public/{token}/accept            → AcceptProposal (public)
POST   /api/proposals/public/{token}/reject            → RejectProposal (public)
POST   /api/proposals/public/{token}/payment           → SubmitPayment (public)
POST   /api/proposals/public/payment/{paymentId}/screenshot → UploadScreenshot (public)
GET    /api/proposals/{id}/payments                    → GetPayments
GET    /api/proposals/public/{token}/payments           → GetPaymentsByToken (public)
POST   /api/proposals/payment/{paymentId}/verify        → VerifyPayment
POST   /api/proposals/public/{token}/feedback           → SubmitFeedback (public)
GET    /api/proposals/{id}/feedback                     → GetFeedback
PUT    /api/proposals/{id}/resend                       → ResendProposal
GET    /api/proposals/public/{token}/invoice             → GetInvoice (public)
GET    /api/proposals/test-email                         → TestEmail
```

### Itinerary Enhancements (`/api/inventory/itinerary-enhancements`)
```
POST   .../itineraries/{id}/media                      → AddMedia
GET    .../itineraries/{id}/media                      → GetMedia
DELETE .../media/{mediaId}                             → DeleteMedia
PUT    .../itineraries/{id}/media/reorder              → ReorderMedia
POST   .../bookings/create-with-assignments             → CreateBookingWithAssignments
POST   .../assignments                                 → CreateAssignment
GET    .../instances/{instanceId}/assignments           → GetAssignments
PUT    .../assignments/{assignmentId}                   → UpdateAssignment
DELETE .../assignments/{assignmentId}                   → DeleteAssignment
POST   .../check-availability                           → CheckAvailability
POST   .../instances/{instanceId}/validate-assignments  → ValidateAssignments
POST   .../booking-requests                             → CreateBookingRequest (public)
GET    .../booking-requests                             → GetBookingRequests
GET    .../booking-requests/{id}                        → GetBookingRequest
PUT    .../booking-requests/{id}/status                 → UpdateBookingRequestStatus
POST   .../booking-requests/{requestId}/convert         → ConvertBookingRequest
GET    .../public/itineraries                           → GetPublishedItineraries (public)
GET    .../public/itineraries/{itineraryId}             → GetPublicItinerary (public)
```

### Pricing (`/api/pricing`)
```
POST   /api/pricing/cost-items                         → CreateCostItem
GET    /api/pricing/cost-items                         → GetAllCostItems
PUT    /api/pricing/cost-items/{id}                    → UpdateCostItem
DELETE /api/pricing/cost-items/{id}                    → DeleteCostItem
POST   /api/pricing/rates                              → CreateRate
GET    /api/pricing/rates/cost-item/{costItemId}       → GetRatesByCostItem
GET    /api/pricing/rates/itinerary/{itineraryId}      → GetRatesByItinerary
POST   /api/pricing/day-costs                          → AssignDayCost
GET    /api/pricing/day-costs/day/{dayId}              → GetDayCosts
GET    /api/pricing/day-costs/itinerary/{itineraryId}  → GetAllDayCostsByItinerary
DELETE /api/pricing/day-costs/{id}                     → RemoveDayCost
GET    /api/pricing/booking/{instanceId}               → GetBookingPricing
```

### Finance (`/api/finance`)
```
GET    /api/finance/summary                            → GetSummary
GET    /api/finance/tax-config                         → GetTaxConfigs
GET    /api/finance/tax-config/active                  → GetActiveTaxConfig
POST   /api/finance/tax-config                         → CreateTaxConfig
PUT    /api/finance/tax-config/{id}                    → UpdateTaxConfig
POST   /api/finance/invoices/generate/{bookingId}      → GenerateFromBooking
POST   /api/finance/invoices                           → CreateInvoice
GET    /api/finance/invoices                           → GetInvoices
GET    /api/finance/invoices/{id}                      → GetInvoice
PATCH  /api/finance/invoices/{id}/status               → UpdateInvoiceStatus
POST   /api/finance/commissions                        → CreateCommission
GET    /api/finance/commissions                        → GetCommissions
GET    /api/finance/commissions/{id}                   → GetCommission
GET    /api/finance/commissions/booking/{bookingId}    → GetCommissionsByBooking
PATCH  /api/finance/commissions/{id}/status            → UpdateCommissionStatus
POST   /api/finance/expenses                           → CreateExpense
GET    /api/finance/expenses                           → GetExpenses
GET    /api/finance/expenses/{id}                      → GetExpense
PUT    /api/finance/expenses/{id}                      → UpdateExpense
DELETE /api/finance/expenses/{id}                      → DeleteExpense
POST   /api/finance/refunds                            → CreateRefund
GET    /api/finance/refunds                            → GetRefunds
GET    /api/finance/refunds/{id}                       → GetRefund
PATCH  /api/finance/refunds/{id}/process               → ProcessRefund
```

---

## Data Flow Summary Diagram

```
┌─────────────┐     ┌──────────────┐     ┌──────────────────┐
│  ITINERARY  │────▶│   BOOKING    │────▶│    DEPARTURE     │
│  (Template) │     │  (Instance)  │     │ (Group Trip)     │
│             │     │              │     │                  │
│ itineraries │     │ itinerary_   │     │ package_         │
│ itinerary_  │     │   instances  │     │   departures     │
│   days      │     │ itinerary_   │     └──────────────────┘
│ itinerary_  │     │   instance_  │              │
│   day_      │     │   days       │              │ assigned
│   activities│     │ travelers    │              ▼
└─────────────┘     │ payments     │     ┌──────────────────┐
       │            │ itinerary_   │     │   INVENTORY      │
       │            │   approvals  │     │ hotels           │
       │            └──────────────┘     │ hotel_rooms      │
       │                   │             │ vehicles         │
       ▼                   │             │ guides           │
┌─────────────┐            │             │ activities       │
│  PROPOSAL   │            │             └──────────────────┘
│             │            │                      │
│ itinerary_  │            ▼                      ▼
│  proposals  │     ┌──────────────┐     ┌──────────────────┐
│ itinerary_  │     │   PRICING    │     │  AVAILABILITY    │
│  proposal_  │     │              │     │                  │
│  days       │     │ cost_items   │     │ availability     │
│ itinerary_  │     │ cost_item_   │     │ availability_    │
│  proposal_  │     │   rates      │     │   blocks         │
│  payments   │     │ itinerary_   │     │ booking_         │
│ itinerary_  │     │   day_costs  │     │   inventory      │
│  proposal_  │     └──────────────┘     └──────────────────┘
│  feedback   │
└─────────────┘            │
                           ▼
                  ┌──────────────────┐
                  │    FINANCE       │
                  │                  │
                  │ tnt_invoices     │
                  │ tnt_invoice_     │
                  │   line_items     │
                  │ tnt_agent_       │
                  │   commissions    │
                  │ tnt_expenses     │
                  │ tnt_refunds      │
                  │ tnt_tax_configs  │
                  └──────────────────┘
```

---

*End of Document*
