# Interview System Admin Panel - System Design

## Implementation Approach

### Technology Stack
- **Backend Framework**: ASP.NET Core MVC 7.0
- **ORM**: Entity Framework Core 7.0
- **Database**: SQL Server
- **Authentication**: ASP.NET Core Identity with JWT tokens
- **Frontend**: Razor Views with jQuery and Tailwind CSS

### Key Libraries and Frameworks
1. **Authentication & Authorization**
   - Microsoft.AspNetCore.Identity.EntityFrameworkCore
   - Microsoft.AspNetCore.Authentication.JwtBearer

2. **Data Access**
   - Microsoft.EntityFrameworkCore.SqlServer
   - Microsoft.EntityFrameworkCore.Tools

3. **Utilities**
   - UUID7.NET (for UUID v7 generation)
   - FluentValidation (for input validation)
   - AutoMapper (for object mapping)
   - Serilog (for logging)

### System Architecture

#### 1. Core Layers
1. **Presentation Layer (MVC)**
   - Controllers
   - Views
   - ViewModels
   - Filters

2. **Application Layer**
   - Services
   - DTOs
   - Interfaces
   - Validators

3. **Domain Layer**
   - Entities
   - Value Objects
   - Enums
   - Domain Events

4. **Infrastructure Layer**
   - Data Access
   - External Services
   - Logging
   - Security

#### 2. Key Components

1. **Identity Management**
   - Custom UserManager and RoleManager
   - JWT Token Service
   - Claims-based authorization
   - Company-specific authorization handler

2. **Database Context**
   - Audit trail implementation
   - Soft delete filter
   - Company-based data filtering

3. **Interview Management**
   - Interview service
   - Candidate service
   - Analysis service
   - Video processing service

### API Endpoints

#### 1. Authentication
```
POST /api/auth/login
POST /api/auth/logout
POST /api/auth/change-password
```

#### 2. User Management
```
GET    /api/users
POST   /api/users
GET    /api/users/{id}
PUT    /api/users/{id}
DELETE /api/users/{id}
```

#### 3. Company Management
```
GET    /api/companies
POST   /api/companies
GET    /api/companies/{id}
PUT    /api/companies/{id}
DELETE /api/companies/{id}
```

#### 4. Interview Management
```
GET    /api/interviews
POST   /api/interviews
GET    /api/interviews/{id}
PUT    /api/interviews/{id}
DELETE /api/interviews/{id}
GET    /api/interviews/active
GET    /api/interviews/passive
GET    /api/interviews/deleted
```

#### 5. Candidate Management
```
GET    /api/candidates
POST   /api/candidates
POST   /api/candidates/bulk
GET    /api/candidates/{id}
PUT    /api/candidates/{id}
DELETE /api/candidates/{id}
POST   /api/candidates/analyze
POST   /api/candidates/analyze-batch
POST   /api/candidates/refresh-status
POST   /api/candidates/{id}/feedback
```

### Security Considerations

1. **Authentication**
   - JWT tokens with refresh token mechanism
   - Password complexity requirements
   - Account lockout after failed attempts

2. **Authorization**
   - Role-based access control
   - Company-based data isolation
   - Claims-based permissions

3. **Data Protection**
   - Input validation
   - XSS protection
   - CSRF protection
   - SQL injection prevention

### Database Design

1. **Soft Delete Implementation**
```csharp
public abstract class BaseEntity
{
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

2. **Global Query Filters**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Apply soft delete filter
    modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
    modelBuilder.Entity<Company>().HasQueryFilter(c => !c.IsDeleted);
    modelBuilder.Entity<Interview>().HasQueryFilter(i => !i.IsDeleted);
    modelBuilder.Entity<Candidate>().HasQueryFilter(c => !c.IsDeleted);
}
```

3. **Indexes**
```sql
CREATE INDEX IX_Users_Email ON Users(Email) WHERE IsDeleted = 0;
CREATE UNIQUE INDEX IX_Candidates_InterviewId_Email 
    ON Candidates(InterviewId, Email) WHERE IsDeleted = 0;
CREATE INDEX IX_Interviews_FirmId_StartDate_EndDate 
    ON Interviews(FirmId, StartDate, EndDate) WHERE IsDeleted = 0;
```

### Performance Optimizations

1. **Caching Strategy**
   - In-memory caching for company and user data
   - Distributed caching for interview lists
   - Output caching for static content

2. **Database Optimizations**
   - Appropriate indexes
   - Lazy loading disabled
   - Efficient query patterns

3. **API Optimizations**
   - Pagination for list endpoints
   - Compression
   - Async/await patterns

### Monitoring and Logging

1. **Application Logging**
   - Request/Response logging
   - Error logging
   - Audit logging

2. **Performance Monitoring**
   - API response times
   - Database query performance
   - Resource utilization

### Error Handling

1. **Global Exception Handler**
   - Structured error responses
   - Logging
   - Environment-specific error details

2. **Validation**
   - Model validation
   - Business rule validation
   - Custom error messages

### Future Considerations

1. **Scalability**
   - Microservices architecture
   - Container support
   - Cloud deployment

2. **Integration**
   - External video processing services
   - Email notification system
   - Analytics integration