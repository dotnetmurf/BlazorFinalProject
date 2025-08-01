/*
    IEventService Interface
    
    Purpose: Defines the contract for event data management operations in the EventEase application,
    providing a standardized interface for event CRUD operations and data retrieval.
    
    Core Operations:
    - GetAllAsync: Retrieves all events from the data store
    - GetByIdAsync: Fetches a specific event by its unique identifier
    - AddAsync: Creates a new event and returns the created entity
    - UpdateAsync: Modifies an existing event and returns the updated entity
    - DeleteAsync: Removes an event from the data store by ID
    
    Advanced Operations:
    - GetPagedAsync: Retrieves events with pagination support and cancellation
    - TryGetByIdAsync: Safe event retrieval that returns null if not found
    - ExistsAsync: Checks if an event exists without retrieving the full entity
    
    Design Patterns:
    - Repository Pattern: Abstracts data access logic from business logic
    - Async/Await: All operations are asynchronous for optimal performance
    - Cancellation Support: Long-running operations support cancellation tokens
    - Null Safety: Provides safe methods that handle missing entities gracefully
    
    Error Handling:
    - GetByIdAsync: May throw exceptions for invalid IDs or missing entities
    - TryGetByIdAsync: Returns null instead of throwing for missing entities
    - Validation: Implementations should validate input parameters
    
    Usage: Implemented by concrete service classes and injected into components
    for event management operations throughout the EventEase application.
*/

namespace BlazorFinalProject.Services;

public interface IEventService
{
    Task<List<Event>> GetAllAsync();
    Task<Event> GetByIdAsync(Guid id);
    Task<Event> AddAsync(Event evt);
    Task<Event> UpdateAsync(Event eventToUpdate);
    Task DeleteAsync(Guid id);
    Task<PagedResult<Event>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Event?> TryGetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}