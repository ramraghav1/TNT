# Multi-Tenant SaaS Implementation Guide

## 🏗️ Architecture Overview

This implementation provides **production-grade multi-tenancy** for your .NET 8 + PostgreSQL + Dapper application.

### Key Features
- ✅ **Tenant Isolation**: Complete data separation between tenants
- ✅ **JWT-Based Security**: TenantId extracted from authenticated user token
- ✅ **Zero Trust Frontend**: Server validates ALL tenant context
- ✅ **Multi-Product Support**: Tenants can subscribe to Tour, Clinic, or Remittance modules
- ✅ **Automatic Filtering**: Base repository handles tenant filtering
- ✅ **Migration-Ready**: FluentMigrator scripts included

---

## 📋 Implementation Checklist

### ✅ Completed Components

1. **Domain Models** (`/Domain/Models/Tenant.cs`)
   - `Tenant`: Main tenant entity
   - `TenantSettings`: Configuration (max users, trial status, timezone, currency)
   - `Product`: Available modules (TourAndTravel, Clinic, Remittance)
   - `TenantProduct`: Tenant-to-product mapping

2. **Database Layer**
   - **Migration 1**: `AddTenantTables.cs` - Creates `tenants`, `products`, `tenant_products`
   - **Migration 2**: `AddTenantIdToExistingTables.cs` - Adds `tenant_id` to business tables
   - **DTOs**: `TenantDTO`, `ProductDTO`, `TenantProductDTO`

3. **Repository Layer**
   - `TenantBaseRepository`: Base class with tenant filtering helpers
   - `TenantRepository`: CRUD operations for tenant management
   - `HotelRepositoryTenantSafe`: Example of converted tenant-safe repository

4. **Service Layer**
   - `ITenantProvider`: Extracts TenantId from JWT claims
   - `JwtTenantProvider`: Implementation using HttpContext

5. **API Layer**
   - `TenantMiddleware`: Validates tenant context on every request
   - `TenantsController`: Tenant management endpoints

6. **Configuration**
   - Updated `Program.cs` with tenant services registration
   - Middleware pipeline configured correctly

---

## 🚀 Getting Started

### Step 1: Run Migrations

```bash
cd DbDeployment
dotnet run
```

This will:
1. Create `tenants`, `products`, `tenant_products` tables
2. Add `tenant_id` column to all business tables
3. Create a default "demo" tenant with TourAndTravel product

### Step 2: Update JWT Token Generation

Modify your login service to include TenantId in JWT claims:

```csharp
// In TokenService.cs or LoginService.cs
public string GenerateToken(User user, long tenantId)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("TenantId", tenantId.ToString()), // ⭐ ADD THIS
        new Claim(ClaimTypes.Name, user.FullName)
    };
    
    // ... rest of token generation
}
```

### Step 3: Update Login to Fetch TenantId

```csharp
// In LoginService.cs
public async Task<LoginResponse> Login(string email, string password)
{
    var user = _loginRepository.GetUserByEmail(email);
    // ... validate password
    
    // Fetch user's tenant
    var tenantId = user.TenantId ?? throw new Exception("User not associated with tenant");
    
    // Generate token with TenantId
    var token = _tokenService.GenerateToken(user, tenantId);
    
    return new LoginResponse { Token = token, ... };
}
```

### Step 4: Convert Existing Repositories

**Before (NOT tenant-safe):**
```csharp
public class HotelRepository : IHotelRepository
{
    private readonly IDbConnection _dbConnection;
    
    public List<HotelResponse> GetAllHotels()
    {
        string query = "SELECT * FROM hotels WHERE is_active = true";
        return _dbConnection.Query<HotelDTO>(query).ToList();
    }
}
```

**After (Tenant-safe):**
```csharp
public class HotelRepository : TenantBaseRepository, IHotelRepository
{
    public HotelRepository(IDbConnection dbConnection, ITenantProvider tenantProvider)
        : base(dbConnection, tenantProvider) { }
    
    public List<HotelResponse> GetAllHotels()
    {
        // ✅ Automatically filters by current user's tenant
        string query = $@"
            SELECT * FROM hotels 
            WHERE {GetTenantFilter()} AND is_active = true";
        
        return QueryWithTenant<HotelDTO>(query).ToList();
    }
}
```

### Step 5: Update DTOs to Include TenantId

```csharp
public class HotelDTO
{
    public long Id { get; set; }
    public long TenantId { get; set; }  // ⭐ ADD THIS
    public string Name { get; set; }
    // ... other properties
}
```

---

## 🔒 Security Best Practices

### ❌ NEVER Trust Frontend for TenantId

```csharp
// ❌ WRONG - Client can manipulate this
public IActionResult GetHotels([FromQuery] long tenantId)
{
    return Ok(_hotelService.GetHotels(tenantId));
}

// ✅ CORRECT - Server extracts from JWT
public IActionResult GetHotels()
{
    var tenantId = _tenantProvider.GetRequiredTenantId(); // From JWT
    return Ok(_hotelService.GetHotels(tenantId));
}
```

### ✅ Always Use Base Repository Methods

```csharp
// ✅ SECURE - Uses tenant filtering
QueryWithTenant<T>(sql, parameters)
QuerySingleOrDefaultWithTenant<T>(sql, parameters)
ExecuteWithTenant(sql, parameters)

// ✅ SECURE - Validates tenant ownership
ValidateTenantOwnership(recordTenantId, "Resource Name")

// ✅ SECURE - Creates parameters with TenantId
CreateTenantParameters(additionalParams)
```

---

## 📊 Database Schema

### Tenants Table
```sql
CREATE TABLE tenants (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    subdomain VARCHAR(100) UNIQUE NOT NULL,
    contact_email VARCHAR(255) NOT NULL,
    status VARCHAR(20) DEFAULT 'Active',
    settings_json JSONB DEFAULT '{}',
    created_at TIMESTAMP DEFAULT NOW()
);
```

### Products Table
```sql
CREATE TABLE products (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) UNIQUE NOT NULL,
    display_name VARCHAR(100) NOT NULL
);

INSERT INTO products (name, display_name) VALUES
    ('TourAndTravel', 'Tour & Travel Management'),
    ('Clinic', 'Clinic Management'),
    ('Remittance', 'Remittance Services');
```

### Tenant-Product Mapping
```sql
CREATE TABLE tenant_products (
    id BIGSERIAL PRIMARY KEY,
    tenant_id BIGINT REFERENCES tenants(id),
    product_id INTEGER REFERENCES products(id),
    is_active BOOLEAN DEFAULT true,
    activated_at TIMESTAMP DEFAULT NOW(),
    UNIQUE(tenant_id, product_id)
);
```

### Business Tables (Example)
```sql
ALTER TABLE hotels ADD COLUMN tenant_id BIGINT NOT NULL REFERENCES tenants(id);
CREATE INDEX idx_hotels_tenant ON hotels(tenant_id);
```

---

## 🧪 Testing Multi-Tenancy

### 1. Create Test Tenants

```sql
-- Tenant 1: Travel Agency
INSERT INTO tenants (name, subdomain, contact_email)
VALUES ('Himalayan Adventures', 'himalayan', 'contact@himalayan.com');

-- Tenant 2: Clinic
INSERT INTO tenants (name, subdomain, contact_email)
VALUES ('City Medical Center', 'citymedical', 'info@citymedical.com');

-- Activate products
INSERT INTO tenant_products (tenant_id, product_id)
SELECT 1, id FROM products WHERE name = 'TourAndTravel';

INSERT INTO tenant_products (tenant_id, product_id)
SELECT 2, id FROM products WHERE name = 'Clinic';
```

### 2. Create Test Users

```sql
-- User for Tenant 1
INSERT INTO users (email, password_hash, full_name, tenant_id)
VALUES ('admin@himalayan.com', '$hashed_password', 'Admin User', 1);

-- User for Tenant 2
INSERT INTO users (email, password_hash, full_name, tenant_id)
VALUES ('admin@citymedical.com', '$hashed_password', 'Clinic Admin', 2);
```

### 3. Test Data Isolation

```bash
# Login as Tenant 1 user
curl -X POST http://localhost:7236/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@himalayan.com", "password": "test123"}'

# Use returned JWT token to fetch hotels
curl http://localhost:7236/api/inventory/hotels \
  -H "Authorization: Bearer <token>"

# Result: Only Tenant 1's hotels are returned
```

---

## 🔧 Conversion Guide for Existing Repositories

For each repository that needs multi-tenancy:

### 1. Update Interface (if needed)
```csharp
// No changes needed to interface usually
public interface IHotelRepository
{
    List<HotelResponse> GetAllHotels(bool includeInactive = false);
    HotelResponse? GetHotelById(long id);
    // ... TenantId is implicit from JWT
}
```

### 2. Inherit from TenantBaseRepository
```csharp
public class HotelRepository : TenantBaseRepository, IHotelRepository
{
    public HotelRepository(IDbConnection dbConnection, ITenantProvider tenantProvider)
        : base(dbConnection, tenantProvider) { }
}
```

### 3. Update SQL Queries
```csharp
// Add tenant filter to WHERE clause
string query = $@"
    SELECT * FROM hotels
    WHERE {GetTenantFilter()} 
        AND is_active = true
    ORDER BY name";

// Use tenant-aware query methods
var hotels = QueryWithTenant<HotelDTO>(query, parameters).ToList();
```

### 4. Update CREATE Operations
```csharp
public HotelResponse CreateHotel(CreateHotelRequest request, long userId)
{
    var tenantId = GetTenantId(); // From JWT
    
    string query = @"
        INSERT INTO hotels (tenant_id, name, location, ...)
        VALUES (@TenantId, @Name, @Location, ...)
        RETURNING id";
    
    var parameters = CreateTenantParameters(new { request.Name, request.Location });
    long id = _dbConnection.QuerySingle<long>(query, parameters);
    
    return GetHotelById(id);
}
```

### 5. Update UPDATE/DELETE Operations
```csharp
public bool DeleteHotel(long id)
{
    // First verify ownership
    var hotel = GetHotelById(id);
    if (hotel == null) return false; // Not found OR wrong tenant
    
    // Update with tenant filter for extra safety
    string query = $@"
        UPDATE hotels SET is_active = false
        WHERE id = @Id AND {GetTenantFilter()}";
    
    return ExecuteWithTenant(query, new { Id = id }) > 0;
}
```

---

## 🎯 Common Patterns

### Pattern 1: List All (Filtered by Tenant)
```csharp
public List<T> GetAll()
{
    string query = $"SELECT * FROM table_name WHERE {GetTenantFilter()}";
    return QueryWithTenant<TDto>(query).ToList();
}
```

### Pattern 2: Get By ID (With Ownership Check)
```csharp
public T? GetById(long id)
{
    string query = $"SELECT * FROM table_name WHERE id = @Id AND {GetTenantFilter()}";
    return QuerySingleOrDefaultWithTenant<TDto>(query, new { Id = id });
}
```

### Pattern 3: Create (Auto-Assign TenantId)
```csharp
public T Create(CreateRequest request)
{
    string query = @"
        INSERT INTO table_name (tenant_id, col1, col2)
        VALUES (@TenantId, @Col1, @Col2)
        RETURNING id";
    
    var parameters = CreateTenantParameters(new { request.Col1, request.Col2 });
    long id = _dbConnection.QuerySingle<long>(query, parameters);
    return GetById(id);
}
```

### Pattern 4: Update (Verify Ownership First)
```csharp
public bool Update(long id, UpdateRequest request)
{
    // Verify exists and belongs to tenant
    var existing = GetById(id);
    if (existing == null) return false;
    
    string query = $@"
        UPDATE table_name SET col1 = @Col1
        WHERE id = @Id AND {GetTenantFilter()}";
    
    return ExecuteWithTenant(query, new { Id = id, request.Col1 }) > 0;
}
```

---

## 📝 Migration Order

Run migrations in this order:

1. ✅ `AddTenantTables.cs` (20260402000001)
2. ✅ `AddTenantIdToExistingTables.cs` (20260402000002)
3. ⏳ Your future migrations (with tenant_id in CREATE TABLE statements)

---

## 🌐 Frontend Integration

Your Angular app should:

1. **Store JWT token** in localStorage/sessionStorage
2. **Send token** in Authorization header: `Bearer <token>`
3. **NEVER send TenantId** manually - it's in the JWT
4. **Handle 403 errors** when tenant context is missing

```typescript
// Angular HTTP Interceptor
intercept(req: HttpRequest<any>, next: HttpHandler) {
    const token = localStorage.getItem('authToken');
    if (token) {
        req = req.clone({
            setHeaders: { Authorization: `Bearer ${token}` }
        });
    }
    return next.handle(req);
}
```

---

## 🔍 Debugging

### Check Current Tenant
```csharp
// In any controller
var tenantId = _tenantProvider.GetTenantId();
Console.WriteLine($"Current Tenant: {tenantId}");
```

### Verify JWT Claims
```bash
# Decode JWT at https://jwt.io
# Look for "TenantId" claim
```

### Test Tenant Middleware
```bash
# Without auth - should allow public endpoints
curl http://localhost:7236/api/auth/login

# With auth but no TenantId - should return 403
curl http://localhost:7236/api/hotels \
  -H "Authorization: Bearer <token_without_tenantid>"
```

---

## ⚠️ Important Notes

1. **Super Admins**: Users with `tenant_id = NULL` can manage all tenants (for support staff)
2. **Cascade Deletes**: Deleting a tenant removes all associated data
3. **Trial Accounts**: Check `TenantSettings.IsTrialAccount` and `TrialExpiresAt`
4. **Subdomain Routing**: Future enhancement to route by subdomain (e.g., `tenant1.yourdomain.com`)

---

## 📚 Next Steps

1. ✅ Run both migrations
2. ✅ Update JWT token generation to include TenantId
3. ✅ Convert all existing repositories to inherit from `TenantBaseRepository`
4. ✅ Update all DTOs to include `TenantId` field
5. ✅ Test with multiple tenants
6. ✅ Update frontend to handle 403 tenant errors
7. ✅ Implement tenant onboarding UI (admin only)

---

## 🆘 Support

For questions or issues:
- Check middleware in `TenantMiddleware.cs`
- Verify JWT claims include "TenantId"
- Ensure all queries use `GetTenantFilter()`
- Validate FK constraints are in place

**Security Rule**: If in doubt, reject. Better to block legitimate requests than leak data.
