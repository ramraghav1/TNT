# Booking UI Workflow - Complete Guide

## Overview
This document explains how to create bookings from the Angular UI with hotel/guide/vehicle assignment and availability checking.

## API Endpoints Summary

### 1. Check Availability (Before Creating Booking)
**Endpoint:** `POST /api/inventory/itinerary-enhancements/check-availability`  
**Auth:** Required  
**Purpose:** Check if hotels, guides, and vehicles are available for selected dates

### 2. Create Booking with Assignments
**Endpoint:** `POST /api/inventory/itinerary-enhancements/bookings/create-with-assignments`  
**Auth:** Required  
**Purpose:** Create booking instance and assign resources in one call

### 3. Validate Assignments (Optional - Before Creating)
**Endpoint:** `POST /api/inventory/itinerary-enhancements/instances/{instanceId}/validate-assignments`  
**Auth:** Required  
**Purpose:** Validate assignments for conflicts before creating booking

---

## Complete UI Workflow

### Step 1: Select Itinerary Template
User selects an itinerary from the list (e.g., "Everest Base Camp Trek")

```typescript
selectedItinerary = {
  id: 123,
  title: "Everest Base Camp Trek",
  durationDays: 12,
  priceStartingFrom: 1200
};
```

### Step 2: Select Date Range
User picks start and end dates

```typescript
bookingForm = {
  startDate: new Date('2026-05-01'),
  endDate: new Date('2026-05-12')
};
```

### Step 3: Check Resource Availability
**IMPORTANT:** Always check availability BEFORE assigning resources

```typescript
// TypeScript/Angular Service
async checkAvailability(request: CheckAvailabilityRequest) {
  const url = `${this.apiUrl}/itinerary-enhancements/check-availability`;
  return this.http.post<AvailabilityResponse>(url, request).toPromise();
}

// Usage in Component
const availabilityCheck = await this.bookingService.checkAvailability({
  startDate: '2026-05-01',
  endDate: '2026-05-12',
  hotelIds: [1, 2, 3, 4],  // All hotels user might want to assign
  guideIds: [10, 11, 12],  // All guides available
  vehicleIds: [20, 21]     // All vehicles available
});
```

**Sample Response:**
```json
{
  "startDate": "2026-05-01",
  "endDate": "2026-05-12",
  "hotels": [
    {
      "id": 1,
      "name": "Mountain View Hotel",
      "isAvailable": true,
      "availableRooms": 15,
      "conflicts": []
    },
    {
      "id": 2,
      "name": "Summit Lodge",
      "isAvailable": false,
      "availableRooms": 0,
      "conflicts": [
        {
          "date": "2026-05-05",
          "bookingId": 456,
          "bookingReference": "BK-2026-05-001"
        }
      ]
    }
  ],
  "guides": [
    {
      "id": 10,
      "name": "Tenzin Sherpa",
      "isAvailable": true,
      "conflicts": []
    },
    {
      "id": 11,
      "name": "Pasang Lama",
      "isAvailable": false,
      "conflicts": [
        {
          "date": "2026-05-07",
          "bookingId": 457,
          "bookingReference": "BK-2026-05-002"
        }
      ]
    }
  ],
  "vehicles": [...]
}
```

### Step 4: Display Availability to User
Show visual indicators in the UI:
- ✅ **Green:** Resource available
- ⚠️ **Yellow/Orange:** Partially available (for hotels with limited rooms)
- ❌ **Red:** Not available (conflicts exist)

```html
<!-- Example UI Display -->
<div *ngFor="let hotel of availabilityResponse.hotels">
  <div class="resource-card" [ngClass]="{
    'available': hotel.isAvailable,
    'unavailable': !hotel.isAvailable
  }">
    <h4>{{ hotel.name }}</h4>
    <span *ngIf="hotel.isAvailable" class="badge-success">
      {{ hotel.availableRooms }} rooms available
    </span>
    <span *ngIf="!hotel.isAvailable" class="badge-danger">
      Not available
    </span>
    
    <!-- Show conflicts -->
    <div *ngIf="hotel.conflicts.length > 0">
      <p-message severity="warn">
        Conflicts on: {{ hotel.conflicts.map(c => c.date).join(', ') }}
      </p-message>
    </div>
  </div>
</div>
```

### Step 5: User Assigns Resources Per Day
User assigns hotel, guide, and vehicle for each day of the itinerary

```typescript
// Component Data Structure
dayAssignments: DayAssignmentInput[] = [
  {
    dayNumber: 1,
    date: new Date('2026-05-01'),
    hotelId: 1,
    roomType: 'Double',
    numberOfRooms: 3,
    guideId: 10,
    vehicleId: 20,
    notes: 'Pick up from airport'
  },
  {
    dayNumber: 2,
    date: new Date('2026-05-02'),
    hotelId: 1,
    roomType: 'Double',
    numberOfRooms: 3,
    guideId: 10,
    vehicleId: 20,
    notes: null
  },
  // ... more days
];
```

**UI Component Example:**
```html
<p-table [value]="itineraryDays">
  <ng-template pTemplate="header">
    <tr>
      <th>Day</th>
      <th>Date</th>
      <th>Hotel</th>
      <th>Rooms</th>
      <th>Guide</th>
      <th>Vehicle</th>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-day>
    <tr>
      <td>Day {{ day.dayNumber }}</td>
      <td>{{ day.date | date }}</td>
      <td>
        <p-dropdown 
          [(ngModel)]="day.hotelId"
          [options]="availableHotels"
          optionLabel="name"
          optionValue="id"
          placeholder="Select Hotel">
        </p-dropdown>
      </td>
      <td>
        <input type="number" [(ngModel)]="day.numberOfRooms" min="1" />
      </td>
      <td>
        <p-dropdown 
          [(ngModel)]="day.guideId"
          [options]="availableGuides"
          optionLabel="name"
          optionValue="id"
          placeholder="Select Guide">
        </p-dropdown>
      </td>
      <td>
        <p-dropdown 
          [(ngModel)]="day.vehicleId"
          [options]="availableVehicles"
          optionLabel="name"
          optionValue="id"
          placeholder="Select Vehicle">
        </p-dropdown>
      </td>
    </tr>
  </ng-template>
</p-table>
```

### Step 6: Add Traveler Information
```typescript
travelerInfo = {
  fullName: 'John Doe',
  contactNumber: '+1-555-0123',
  email: 'john.doe@example.com',
  nationality: 'USA',
  adults: 2,
  children: 1,
  seniors: 0
};
```

### Step 7: Create Booking with Assignments
```typescript
async createBooking() {
  const bookingRequest: CreateBookingWithAssignmentsRequest = {
    templateItineraryId: this.selectedItinerary.id,
    startDate: this.bookingForm.startDate,
    endDate: this.bookingForm.endDate,
    dayAssignments: this.dayAssignments,
    traveler: this.travelerInfo,
    specialRequests: this.bookingForm.specialRequests
  };

  try {
    const response = await this.bookingService.createBookingWithAssignments(bookingRequest);
    
    if (response.status === 'Success') {
      this.messageService.add({
        severity: 'success',
        summary: 'Booking Created',
        detail: `Booking ${response.bookingReference} created successfully`
      });
      
      // Navigate to booking details
      this.router.navigate(['/bookings', response.instanceId]);
    } 
    else if (response.status === 'ConflictsDetected') {
      // Show conflicts to user
      this.displayConflicts(response.conflicts);
    }
  } catch (error) {
    this.messageService.add({
      severity: 'error',
      summary: 'Error',
      detail: error.message
    });
  }
}
```

**Sample Request:**
```json
{
  "templateItineraryId": 123,
  "startDate": "2026-05-01",
  "endDate": "2026-05-12",
  "dayAssignments": [
    {
      "dayNumber": 1,
      "date": "2026-05-01",
      "hotelId": 1,
      "roomType": "Double",
      "numberOfRooms": 3,
      "guideId": 10,
      "vehicleId": 20,
      "notes": "Airport pickup"
    }
  ],
  "traveler": {
    "fullName": "John Doe",
    "contactNumber": "+1-555-0123",
    "email": "john.doe@example.com",
    "nationality": "USA",
    "adults": 2,
    "children": 1,
    "seniors": 0
  },
  "specialRequests": "Vegetarian meals preferred"
}
```

**Sample Success Response:**
```json
{
  "status": "Success",
  "instanceId": 789,
  "bookingReference": "BK-2026-05-15-789",
  "message": "Booking created successfully"
}
```

**Sample Conflict Response:**
```json
{
  "status": "ConflictsDetected",
  "message": "Some resources have conflicts",
  "conflicts": [
    {
      "dayNumber": 5,
      "date": "2026-05-05",
      "resourceType": "Hotel",
      "resourceId": 2,
      "resourceName": "Summit Lodge",
      "conflictingBookingId": 456,
      "conflictingBookingReference": "BK-2026-05-001",
      "conflictReason": "Hotel is already booked for this date"
    }
  ]
}
```

### Step 8: Handle Conflicts
If conflicts are detected, show them to the user and allow them to:
1. Choose different resources
2. Change dates
3. Cancel the booking

```typescript
displayConflicts(conflicts: AssignmentConflict[]) {
  const conflictMessages = conflicts.map(c => 
    `Day ${c.dayNumber}: ${c.resourceType} "${c.resourceName}" is not available (${c.conflictReason})`
  ).join('\n');

  this.confirmationService.confirm({
    message: `The following conflicts were detected:\n\n${conflictMessages}\n\nWould you like to modify your selections?`,
    header: 'Booking Conflicts',
    icon: 'pi pi-exclamation-triangle',
    accept: () => {
      // User can modify selections
      this.highlightConflictingDays(conflicts);
    }
  });
}
```

---

## Angular Service Implementation

```typescript
// booking.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private apiUrl = 'http://localhost:5295/api/inventory/itinerary-enhancements';

  constructor(private http: HttpClient) {}

  checkAvailability(request: CheckAvailabilityRequest): Observable<AvailabilityResponse> {
    return this.http.post<AvailabilityResponse>(`${this.apiUrl}/check-availability`, request);
  }

  createBookingWithAssignments(request: CreateBookingWithAssignmentsRequest): Observable<BookingWithAssignmentsResponse> {
    return this.http.post<BookingWithAssignmentsResponse>(`${this.apiUrl}/bookings/create-with-assignments`, request);
  }

  getAssignments(instanceId: number): Observable<AssignmentResponse[]> {
    return this.http.get<AssignmentResponse[]>(`${this.apiUrl}/instances/${instanceId}/assignments`);
  }

  updateAssignment(assignmentId: number, request: CreateAssignmentRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/assignments/${assignmentId}`, request);
  }

  deleteAssignment(assignmentId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/assignments/${assignmentId}`);
  }
}
```

---

## TypeScript Interfaces

```typescript
export interface CheckAvailabilityRequest {
  startDate: string;
  endDate: string;
  hotelIds?: number[];
  guideIds?: number[];
  vehicleIds?: number[];
}

export interface AvailabilityResponse {
  startDate: string;
  endDate: string;
  hotels: HotelAvailability[];
  guides: GuideAvailability[];
  vehicles: VehicleAvailability[];
}

export interface HotelAvailability {
  id: number;
  name: string;
  isAvailable: boolean;
  availableRooms?: number;
  conflicts: DateConflict[];
}

export interface GuideAvailability {
  id: number;
  name: string;
  isAvailable: boolean;
  conflicts: DateConflict[];
}

export interface VehicleAvailability {
  id: number;
  name: string;
  isAvailable: boolean;
  conflicts: DateConflict[];
}

export interface DateConflict {
  date: string;
  bookingId: number;
  bookingReference: string;
}

export interface CreateBookingWithAssignmentsRequest {
  templateItineraryId: number;
  startDate: string;
  endDate: string;
  dayAssignments: DayAssignmentInput[];
  traveler?: TravelerInfo;
  specialRequests?: string;
}

export interface DayAssignmentInput {
  dayNumber: number;
  date: string;
  hotelId?: number;
  roomType?: string;
  numberOfRooms?: number;
  guideId?: number;
  vehicleId?: number;
  notes?: string;
}

export interface TravelerInfo {
  fullName: string;
  contactNumber?: string;
  email?: string;
  nationality?: string;
  adults: number;
  children: number;
  seniors: number;
}

export interface BookingWithAssignmentsResponse {
  instanceId: number;
  bookingReference: string;
  status: string;
  conflicts: AssignmentConflict[];
}

export interface AssignmentConflict {
  dayNumber: number;
  date: string;
  resourceType: string;
  resourceId: number;
  resourceName: string;
  conflictingBookingId?: number;
  conflictingBookingReference?: string;
  conflictReason: string;
}
```

---

## Database Flow

When you call `createBookingWithAssignments`:

1. **Validate Availability** - Checks all resources for conflicts
2. **Create Booking Instance** - Calls existing `BookingService.CreateBooking()` which:
   - Inserts record into `itinerary_instances` table
   - Copies days from template to `itinerary_instance_days` table
   - Adds travelers to `itinerary_travelers` table
   - Generates booking reference
3. **Create Assignments** - For each day:
   - Inserts into `itinerary_instance_day_assignments` table
   - Links hotel, guide, vehicle to specific day
4. **Return Response** - Returns instance ID and booking reference

---

## Testing Checklist

- [ ] Check availability returns correct conflicts
- [ ] Can create booking with no assignments
- [ ] Can create booking with only hotel assignments
- [ ] Can create booking with hotel + guide + vehicle
- [ ] Conflicts are detected when resources are double-booked
- [ ] Guide can only be assigned to one booking per day
- [ ] Hotel room availability is calculated correctly
- [ ] Vehicle can only be assigned to one booking per day
- [ ] Can add assignments after booking is created
- [ ] Can update existing assignments
- [ ] Can delete assignments
- [ ] Booking reference is generated correctly
- [ ] Traveler information is saved
- [ ] Special requests are saved

---

## Summary

**Complete Flow:**
1. Select itinerary template
2. Choose dates
3. **Check availability** (prevents conflicts)
4. Assign resources per day (hotel, guide, vehicle)
5. Add traveler info
6. **Create booking** (creates instance + assignments)
7. System validates again during creation
8. If conflicts: show errors, let user fix
9. If success: redirect to booking details

**Key Points:**
- Always check availability BEFORE assigning
- System validates availability during booking creation
- Conflicts are shown with details (which booking, which date)
- Each booking creates a NEW instance (copy of template)
- Assignments are linked to instance days, not template days
- One guide per booking per day (no double-booking)
- One vehicle per booking per day (no double-booking)
- Hotels track room availability (multiple bookings allowed if rooms available)
