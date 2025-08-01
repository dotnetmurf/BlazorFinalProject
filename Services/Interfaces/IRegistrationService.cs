/*
    IRegistrationService Interface
    
    Purpose: Defines the contract for registration data management operations in the EventEase application,
    providing a standardized interface for registration CRUD operations, event-specific queries, and attendance tracking.
    
    Core Operations:
    - GetAllAsync: Retrieves all registrations from the data store
    - GetByIdAsync: Fetches a specific registration by its unique identifier
    - AddAsync: Creates a new registration and returns the created entity
    - UpdateAsync: Modifies an existing registration and returns the updated entity
    - DeleteAsync: Removes a registration from the data store by ID
    
    Event-Specific Operations:
    - GetAllForEventAsync: Retrieves all registrations for a specific event
    - GetPagedForEventAsync: Paginated registrations for a specific event with cancellation
    - GetRegistrationCountForEventAsync: Gets total registration count for an event
    - GetAttendedRegistrationsForEventAsync: Retrieves only attended registrations for an event
    - IsUserRegisteredForEventAsync: Checks if a user is already registered for an event
    
    Advanced Operations:
    - GetPagedAsync: Retrieves registrations with pagination support and cancellation
    - TryGetByIdAsync: Safe registration retrieval that returns null if not found
    - ExistsAsync: Checks if a registration exists without retrieving the full entity
    
    Design Patterns:
    - Repository Pattern: Abstracts data access logic from business logic
    - Async/Await: All operations are asynchronous for optimal performance
    - Cancellation Support: Long-running operations support cancellation tokens
    - Null Safety: Provides safe methods that handle missing entities gracefully
    
    Error Handling:
    - GetByIdAsync: May throw exceptions for invalid IDs or missing entities
    - TryGetByIdAsync: Returns null instead of throwing for missing entities
    - Validation: Implementations should validate input parameters and business rules
    
    Usage: Implemented by concrete service classes and injected into components
    for registration management, attendance tracking, and event participation operations
    throughout the EventEase application.
*/

namespace BlazorFinalProject.Services;

public interface IRegistrationService
{
    Task<List<Registration>> GetAllAsync();
    Task<List<Registration>> GetAllForEventAsync(Guid eventId);
    Task<Registration> GetByIdAsync(Guid id);
    Task<Registration> AddAsync(Registration registration);
    Task<Registration> UpdateAsync(Registration registration);
    Task DeleteAsync(Guid id);
    Task<PagedResult<Registration>> GetPagedForEventAsync(Guid eventId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Registration?> TryGetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> GetRegistrationCountForEventAsync(Guid eventId);
    Task<List<Registration>> GetAttendedRegistrationsForEventAsync(Guid eventId);
    Task<bool> IsUserRegisteredForEventAsync(Guid eventId, string emailAddress);
    Task<PagedResult<Registration>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}