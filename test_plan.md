# Interview System Admin Panel - Test Plan

## 1. User Authentication and Authorization Tests

### 1.1 SuperAdmin Role Tests
- Test login with default credentials (admin/admin)
- Test password change functionality
- Test user creation with all roles
- Test firmId claim assignment
- Test access to all system features

### 1.2 Admin Role Tests
- Test login functionality
- Test user creation (admin and user roles only)
- Test automatic firmId claim assignment
- Test restricted access to company management

### 1.3 User Role Tests
- Test login functionality
- Test restricted access (no user management)
- Test access to allowed features only

### 1.4 General Authentication Tests
- Test invalid login attempts
- Test session management
- Test logout functionality
- Test password reset

## 2. Company Management Tests

### 2.1 CRUD Operations
- Test company creation by SuperAdmin
- Test company details viewing
- Test company information update
- Test soft delete implementation
- Test company list filtering

### 2.2 Access Control Tests
- Verify only SuperAdmin can manage companies
- Test access restrictions for Admin and User roles

## 3. Interview Management Tests

### 3.1 Interview Creation Tests
- Test interview creation with all required fields
- Validate interview duration constraints (max 180s)
- Test date range validation
- Test access for all roles

### 3.2 Interview List Tests
- Test filtering by company
- Test active/passive/deleted status filters
- Test text search functionality
- Test soft delete operation
- Test role-based edit/delete permissions

## 4. Candidate Management Tests

### 4.1 Registration Tests
- Test single candidate registration
- Test bulk email registration
- Verify UUID v7 generation
- Test duplicate email handling
- Test email format validation

### 4.2 Candidate List Tests
- Test company-based filtering
- Test interview status filtering
- Test search functionality
- Test role-based permissions

### 4.3 Analysis Features Tests
- Test bulk analysis functionality
- Test selected candidates analysis
- Test retry analysis for failed cases
- Test interview status refresh
- Test individual candidate review

## 5. Performance and Security Tests

### 5.1 Performance Tests
- Test page load times
- Test bulk operations performance
- Test concurrent user access

### 5.2 Security Tests
- Test authorization tokens
- Test role-based access control
- Test input validation
- Test SQL injection prevention

## Test Environment Requirements

### Development Environment
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server Database
- Test user accounts for each role

### Test Data Requirements
- Sample company data
- Test user accounts
- Sample interview data
- Test candidate emails

## Test Execution Process
1. Setup test environment
2. Execute authentication tests
3. Run company management tests
4. Perform interview management tests
5. Execute candidate management tests
6. Run analysis feature tests
7. Perform security and performance tests

## Test Reporting
- Document test results
- Track issues and bugs
- Maintain test execution logs
- Generate test coverage reports