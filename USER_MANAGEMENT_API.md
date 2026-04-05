# User Management API Documentation

## Overview
Complete API documentation for the multi-tenant user management system with roles, permissions, and password reset functionality.

---

## User Management Endpoints

### 1. Create Single User
**POST** `/api/usermanagement/create`

Creates a new user with auto-generated password and sends welcome email.

**Request Body:**
```json
{
  "firstName": "John",
  "middleName": "M",
  "lastName": "Doe",
  "gender": "Male",
  "emailAddress": "john.doe@example.com",
  "phoneNumber": "+1234567890",
  "tenantId": 1,
  "organizationId": 1,
  "branchId": 1,
  "roleIds": [2, 3]
}
```

**Response:**
```json
{
  "success": true,
  "message": "User created successfully",
  "data": {
    "userId": 15,
    "username": "john_doe_1734567890",
    "temporaryPassword": "aB3$xY9#pQr4",
    "emailSent": true,
    "resetToken": "abc123...",
    "resetTokenExpiry": "2024-01-08T12:00:00Z"
  }
}
```

---

### 2. Create Multiple Users (Bulk)
**POST** `/api/usermanagement/create-bulk`

Creates multiple users at once (for the accordion form feature).

**Request Body:**
```json
[
  {
    "firstName": "User1",
    "lastName": "One",
    "emailAddress": "user1@example.com",
    "phoneNumber": "+1111111111",
    "tenantId": 1,
    "organizationId": 1,
    "branchId": 1,
    "roleIds": [2]
  },
  {
    "firstName": "User2",
    "lastName": "Two",
    "emailAddress": "user2@example.com",
    "phoneNumber": "+2222222222",
    "tenantId": 1,
    "organizationId": 1,
    "branchId": 1,
    "roleIds": [3]
  }
]
```

**Response:**
```json
{
  "success": true,
  "message": "Created 2 out of 2 users",
  "data": [
    {
      "userId": 16,
      "username": "user1_one_1734567891",
      "temporaryPassword": "cD4%yZ8@qRs5",
      "emailSent": true
    },
    {
      "userId": 17,
      "username": "user2_two_1734567892",
      "temporaryPassword": "eF5&wX7!tUv6",
      "emailSent": true
    }
  ],
  "errors": []
}
```

---

### 3. Get Users by Tenant
**GET** `/api/usermanagement/tenant/{tenantId}`

Retrieves all users for a specific tenant with their roles.

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "userId": 15,
      "firstName": "John",
      "lastName": "Doe",
      "emailAddress": "john.doe@example.com",
      "phoneNumber": "+1234567890",
      "username": "john_doe_1734567890",
      "isActive": true,
      "roles": "manager,user",
      "createdDate": "2024-01-01T10:00:00Z"
    }
  ]
}
```

---

### 4. Get Users by Organization
**GET** `/api/usermanagement/organization/{orgId}`

Retrieves all users for a specific organization.

**Response:** Same format as tenant endpoint.

---

### 5. Deactivate User
**POST** `/api/usermanagement/{userId}/deactivate`

Removes user's login access (soft delete).

**Response:**
```json
{
  "success": true,
  "message": "User deactivated successfully"
}
```

---

### 6. Reactivate User
**POST** `/api/usermanagement/{userId}/reactivate`

Restores user's login access.

**Response:**
```json
{
  "success": true,
  "message": "User reactivated successfully"
}
```

---

### 7. Request Password Reset
**POST** `/api/usermanagement/password-reset/request`

**Authorization:** None required (public endpoint)

Sends password reset email with secure token.

**Request Body:**
```json
{
  "email": "john.doe@example.com"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Password reset email sent",
  "data": {
    "email": "john.doe@example.com",
    "resetToken": "xyz789...",
    "expiresAt": "2024-01-02T10:00:00Z"
  }
}
```

---

### 8. Reset Password
**POST** `/api/usermanagement/password-reset/confirm`

**Authorization:** None required (public endpoint)

Resets password using token from email.

**Request Body:**
```json
{
  "token": "xyz789...",
  "newPassword": "MyNewSecure@Pass123"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Password reset successfully"
}
```

---

## Role Management Endpoints

### 1. Get Roles by Tenant and Product
**GET** `/api/role/tenant/{tenantId}/product/{productId}`

Retrieves all roles for a tenant scoped to their product.

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "tenantId": 1,
      "productId": 1,
      "name": "manager",
      "displayName": "Manager",
      "description": "Can manage users and inventory",
      "isSystemRole": true,
      "isActive": true
    }
  ]
}
```

---

### 2. Get Permissions by Product
**GET** `/api/role/permissions/product/{productId}`

Retrieves all available permissions for a product.

**Example Response (TourAndTravel - productId: 1):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "productId": 1,
      "name": "inventory",
      "displayName": "Inventory Management",
      "description": "Manage hotels, vehicles, guides, activities"
    },
    {
      "id": 2,
      "productId": 1,
      "name": "availability",
      "displayName": "Availability Management",
      "description": "Manage inventory availability and dates"
    }
  ]
}
```

---

### 3. Get User Permissions
**GET** `/api/role/permissions/user/{userId}`

Retrieves effective permissions for a user (aggregated from all roles).

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "inventory",
      "displayName": "Inventory Management",
      "canCreate": true,
      "canRead": true,
      "canUpdate": true,
      "canDelete": false
    }
  ]
}
```

---

### 4. Create Custom Role
**POST** `/api/role/create`

Creates a new tenant-specific custom role.

**Request Body:**
```json
{
  "tenantId": 1,
  "productId": 1,
  "name": "Inventory Clerk",
  "displayName": "Inventory Clerk",
  "description": "Can view and update inventory but not delete"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Role created successfully",
  "data": {
    "roleId": 5
  }
}
```

---

### 5. Update Role
**PUT** `/api/role/{roleId}`

Updates display name and description of a custom role.

**Request Body:**
```json
{
  "displayName": "Senior Inventory Clerk",
  "description": "Experienced inventory manager with extended permissions",
  "isActive": true
}
```

**Response:**
```json
{
  "success": true,
  "message": "Role updated successfully"
}
```

---

### 6. Delete Role
**DELETE** `/api/role/{roleId}`

Deletes a custom role (system roles cannot be deleted).

**Response:**
```json
{
  "success": true,
  "message": "Role deleted successfully"
}
```

---

### 7. Assign Permissions to Role
**POST** `/api/role/{roleId}/permissions`

Assigns or updates permissions for a role with CRUD flags.

**Request Body:**
```json
[
  {
    "permissionId": 1,
    "canCreate": true,
    "canRead": true,
    "canUpdate": true,
    "canDelete": false
  },
  {
    "permissionId": 2,
    "canCreate": false,
    "canRead": true,
    "canUpdate": false,
    "canDelete": false
  }
]
```

**Response:**
```json
{
  "success": true,
  "message": "Permissions assigned successfully"
}
```

---

### 8. Get Role Permissions
**GET** `/api/role/{roleId}/permissions`

Retrieves all permissions assigned to a role.

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "inventory",
      "displayName": "Inventory Management",
      "canCreate": true,
      "canRead": true,
      "canUpdate": true,
      "canDelete": false
    }
  ]
}
```

---

## Email Templates

### Welcome Email
Sent when a new user is created.

**Contains:**
- Username for login
- Temporary password (styled with code block)
- Password reset link (7-day expiration)
- Branded gradient header (purple/violet)

### Password Reset Email
Sent when password reset is requested.

**Contains:**
- Password reset link with token
- 24-hour expiration warning
- Security notice
- Warning-styled gradient header (orange/red)

### Password Changed Notification
Sent after successful password reset.

**Contains:**
- Confirmation of password change
- Security alert if unauthorized
- Support contact information
- Success-styled gradient header (green)

---

## Multi-Tenancy & Security

### Tenant Isolation
- Each tenant has isolated roles (cannot see other tenants' roles)
- Permissions are product-scoped (shared across tenants with same product)
- Users are tenant-scoped (cannot access other tenant data)

### System Roles
Three default roles created for each tenant:
- **admin**: Full access to all permissions
- **manager**: Can create users and manage operations
- **user**: Read-only access to assigned areas

System roles cannot be deleted but can be customized per tenant.

### Password Security
- Auto-generated 12-character passwords with special characters
- BCrypt hashing (cost factor: 10)
- Password reset tokens: cryptographically secure, 24-hour expiration
- Initial setup tokens: 7-day expiration for new users

### Product-Specific Permissions

#### TourAndTravel (productId: 1)
- inventory
- availability
- itinerary
- booking

#### Clinic (productId: 2)
- appointments
- patients
- practitioners
- invoices

#### Remittance (productId: 3)
- agents
- branches
- transactions
- vouchers

---

## Configuration

### SMTP Settings (appsettings.json)
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "SenderEmail": "noreply@yourdomain.com",
    "SenderName": "TNT Platform"
  }
}
```

### Environment Variables
```bash
# SMTP credentials
SMTP_USER=your-email@gmail.com
SMTP_PASS=your-app-password

# Database
DATABASE_URL=postgresql://user:pass@host:5432/dbname
```

---

## Testing with cURL

### Create User
```bash
curl -X POST http://localhost:5000/api/usermanagement/create \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "firstName": "Test",
    "lastName": "User",
    "emailAddress": "test@example.com",
    "phoneNumber": "+1234567890",
    "tenantId": 1,
    "organizationId": 1,
    "branchId": 1,
    "roleIds": [2]
  }'
```

### Request Password Reset
```bash
curl -X POST http://localhost:5000/api/usermanagement/password-reset/request \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com"
  }'
```

### Get Roles
```bash
curl -X GET http://localhost:5000/api/role/tenant/1/product/1 \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Assign Permissions
```bash
curl -X POST http://localhost:5000/api/role/5/permissions \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '[
    {
      "permissionId": 1,
      "canCreate": true,
      "canRead": true,
      "canUpdate": true,
      "canDelete": false
    }
  ]'
```

---

## Error Handling

All endpoints return consistent error format:

```json
{
  "success": false,
  "message": "Error description here"
}
```

Common HTTP status codes:
- **200 OK**: Success
- **400 Bad Request**: Invalid input or business logic error
- **401 Unauthorized**: Missing or invalid JWT token
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server-side error

---

## Next Steps

1. **Configure SMTP**: Update appsettings.json with your email provider credentials
2. **Run Migrations**: Ensure database has latest schema with roles/permissions tables
3. **Test Endpoints**: Use cURL or Postman to test user creation and role management
4. **Build Frontend**: Create Angular components for user list, create form, and role management
5. **Deploy**: Update production environment variables and run migrations on Neon database
