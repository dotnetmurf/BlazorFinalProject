/*
    EventService Implementation
    
    Purpose: Provides event data management operations for the EventEase application using browser 
    local storage as the persistence layer, implementing the IEventService interface contract.
    
    Core Operations:
    - GetAllAsync: Retrieves all events from local storage
    - GetByIdAsync: Fetches specific event by ID with exception handling for missing entities
    - AddAsync: Creates new events with automatic GUID generation and validation
    - UpdateAsync: Modifies existing events with comprehensive property updates
    - DeleteAsync: Removes events and their associated registrations (cascade delete)
    
    Advanced Operations:
    - GetPagedAsync: Provides paginated event retrieval with configurable page sizes
    - TryGetByIdAsync: Safe event retrieval returning null for missing entities
    - ExistsAsync: Efficient existence checking without full entity retrieval
    
    Storage Management:
    - Uses browser localStorage through ILocalStorageService abstraction
    - EventsKey: "events" - primary storage key for event data
    - RegistrationsKey: "registrations" - manages associated registration cleanup
    - JSON serialization for browser compatibility and data persistence
    
    Data Integrity:
    - Cascade deletion: Removes associated registrations when deleting events
    - Input validation: Validates required fields and non-empty GUIDs
    - Null safety: Handles missing data gracefully with appropriate defaults
    - Automatic GUID generation for new events
    
    Error Handling:
    - Comprehensive logging for all operations (success, warnings, errors)
    - Exception propagation with context-specific error messages
    - Graceful degradation for storage access failures
    - Validation exceptions for invalid input parameters
    
    Configuration:
    - DefaultPageSize: 10 items per page for pagination
    - MaxPageSize: 100 items maximum page size limit
    - Configurable page size validation and normalization
    
    Dependencies:
    - ILocalStorageService: Browser storage abstraction for data persistence
    - ILogger<EventService>: Structured logging for monitoring and debugging
    - Implements IEventService interface for dependency injection
    
    Usage: Injected into components and pages throughout the EventEase application
    to provide reliable event data management with local storage persistence,
    supporting offline functionality and rapid prototyping scenarios.
*/

namespace BlazorFinalProject.Services;

public class EventService : IEventService
{
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<EventService> _logger;
    private const string EventsKey = "events";
    private const string RegistrationsKey = "registrations";
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    public EventService(ILocalStorageService localStorage, ILogger<EventService> logger)
    {
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<Event>> GetAllAsync()
    {
        try
        {
            var events = await _localStorage.GetItemAsync<List<Event>>(EventsKey);
            return events ?? new List<Event>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all events from local storage");
            return new List<Event>();
        }
    }

    public async Task<Event> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Event ID cannot be empty", nameof(id));

        var events = await GetAllAsync();
        var foundEvent = events.FirstOrDefault(e => e.Id == id);

        if (foundEvent == null)
            throw new InvalidOperationException($"Event with ID {id} not found");

        return foundEvent;
    }

    public async Task<Event?> TryGetByIdAsync(Guid id)
    {
        if (id == Guid.Empty) return null;

        var events = await GetAllAsync();
        return events.FirstOrDefault(e => e.Id == id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        if (id == Guid.Empty) return false;

        var events = await GetAllAsync();
        return events.Any(e => e.Id == id);
    }

    public async Task<Event> AddAsync(Event evt)
    {
        if (evt == null)
            throw new ArgumentNullException(nameof(evt));

        if (string.IsNullOrWhiteSpace(evt.Name))
            throw new ArgumentException("Event name is required", nameof(evt));

        try
        {
            var events = await GetAllAsync();
            evt.Id = Guid.NewGuid();
            events.Add(evt);
            await _localStorage.SetItemAsync(EventsKey, events);

            _logger.LogInformation("Successfully added event {EventId}: {EventName}", evt.Id, evt.Name);
            return evt;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding event: {EventName}", evt.Name);
            throw;
        }
    }

    public async Task<Event> UpdateAsync(Event eventToUpdate)
    {
        if (eventToUpdate == null)
            throw new ArgumentNullException(nameof(eventToUpdate));

        if (eventToUpdate.Id == Guid.Empty)
            throw new ArgumentException("Event ID cannot be empty", nameof(eventToUpdate));

        if (string.IsNullOrWhiteSpace(eventToUpdate.Name))
            throw new ArgumentException("Event name is required", nameof(eventToUpdate));

        try
        {
            var events = await GetAllAsync();
            var existingEvent = events.FirstOrDefault(e => e.Id == eventToUpdate.Id);

            if (existingEvent == null)
            {
                _logger.LogWarning("Attempted to update non-existent event with ID: {EventId}", eventToUpdate.Id);
                throw new InvalidOperationException($"Event with ID {eventToUpdate.Id} not found");
            }

            _logger.LogInformation("Updating event {EventId}: {EventName}", eventToUpdate.Id, eventToUpdate.Name);

            existingEvent.Name = eventToUpdate.Name;
            existingEvent.Date = eventToUpdate.Date;
            existingEvent.Location = eventToUpdate.Location;
            existingEvent.Notes = eventToUpdate.Notes;

            await _localStorage.SetItemAsync(EventsKey, events);
            _logger.LogInformation("Successfully updated event {EventId}", eventToUpdate.Id);

            return existingEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {EventId}", eventToUpdate.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Event ID cannot be empty", nameof(id));

        try
        {
            await DeleteAssociatedRegistrationsAsync(id);

            var events = await GetAllAsync();
            var removedCount = events.RemoveAll(e => e.Id == id);

            if (removedCount == 0)
                throw new InvalidOperationException($"Event with ID {id} not found");

            await _localStorage.SetItemAsync(EventsKey, events);
            _logger.LogInformation("Successfully deleted event {EventId} and its registrations", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {EventId}", id);
            throw;
        }
    }

    public async Task<PagedResult<Event>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageSize <= 0) pageSize = DefaultPageSize;
        if (pageSize > MaxPageSize) pageSize = MaxPageSize;
        if (pageNumber <= 0) pageNumber = 1;

        var allEvents = await GetAllAsync();

        return await PagedResult<Event>.CreatePagesAsync(
            allEvents,
            pageNumber,
            pageSize,
            e => e.Name,
            cancellationToken);
    }

    private async Task DeleteAssociatedRegistrationsAsync(Guid eventId)
    {
        try
        {
            var registrations = await GetRegistrationsAsync();
            var registrationsToDelete = registrations.Where(r => r.EventId == eventId).ToList();

            if (registrationsToDelete.Any())
            {
                var updatedRegistrations = registrations.Except(registrationsToDelete).ToList();
                await _localStorage.SetItemAsync(RegistrationsKey, updatedRegistrations);
                _logger.LogInformation("Deleted {Count} registrations for event {EventId}", registrationsToDelete.Count, eventId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting registrations for event {EventId}", eventId);
            throw;
        }
    }

    private async Task<List<Registration>> GetRegistrationsAsync()
    {
        var registrations = await _localStorage.GetItemAsync<List<Registration>>(RegistrationsKey);
        return registrations ?? new List<Registration>();
    }
}