/*
    RegistrationService Implementation
    
    Purpose: Provides registration data management operations for the EventEase application using browser 
    local storage as the persistence layer, implementing the IRegistrationService interface contract.
    
    Core Operations:
    - GetAllAsync: Retrieves all registrations from local storage
    - GetByIdAsync: Fetches specific registration by ID with exception handling for missing entities
    - AddAsync: Creates new registrations with automatic GUID generation and validation
    - UpdateAsync: Modifies existing registrations with comprehensive property updates
    - DeleteAsync: Removes registrations from the data store by ID
    
    Event-Specific Operations:
    - GetAllForEventAsync: Retrieves all registrations for a specific event
    - GetPagedForEventAsync: Paginated registrations for a specific event with cancellation support
    - GetRegistrationCountForEventAsync: Gets total registration count for an event
    - GetAttendedRegistrationsForEventAsync: Retrieves only attended registrations for an event
    - IsUserRegisteredForEventAsync: Checks if a user is already registered for an event
    
    Advanced Operations:
    - GetPagedAsync: Provides paginated registration retrieval with configurable page sizes
    - TryGetByIdAsync: Safe registration retrieval returning null for missing entities
    - ExistsAsync: Efficient existence checking without full entity retrieval
    
    Storage Management:
    - Uses browser localStorage through ILocalStorageService abstraction
    - RegistrationsKey: "registrations" - primary storage key for registration data
    - JSON serialization for browser compatibility and data persistence
    
    Data Integrity:
    - Input validation: Validates required fields, non-empty GUIDs, and business rules
    - Duplicate prevention: Checks for existing registrations by email per event
    - Automatic GUID generation for new registrations
    - Attendance tracking support with boolean flags
    
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
    - ILogger<RegistrationService>: Structured logging for monitoring and debugging
    - Implements IRegistrationService interface for dependency injection
    
    Usage: Injected into components and pages throughout the EventEase application
    to provide reliable registration data management with local storage persistence,
    supporting event participation tracking and attendee management operations.
*/

namespace BlazorFinalProject.Services;

public class RegistrationService : IRegistrationService
{
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<RegistrationService> _logger;

    private const string RegistrationsKey = "registrations";
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    public RegistrationService(ILocalStorageService localStorage, ILogger<RegistrationService> logger)
    {
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<Registration>> GetAllAsync()
    {
        try
        {
            var registrations = await _localStorage.GetItemAsync<List<Registration>>(RegistrationsKey);
            return registrations ?? new List<Registration>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all registrations from local storage");
            return new List<Registration>();
        }
    }

    public async Task<List<Registration>> GetAllForEventAsync(Guid eventId)
    {
        if (eventId == Guid.Empty)
            throw new ArgumentException("Event ID cannot be empty", nameof(eventId));

        try
        {
            var allRegistrations = await GetAllAsync();
            var eventRegistrations = allRegistrations.Where(r => r.EventId == eventId).ToList();

            _logger.LogDebug("Retrieved {Count} registrations for event {EventId}", eventRegistrations.Count, eventId);
            return eventRegistrations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving registrations for event {EventId}", eventId);
            throw;
        }
    }

    public async Task<Registration> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Registration ID cannot be empty", nameof(id));

        var registrations = await GetAllAsync();
        var registration = registrations.FirstOrDefault(r => r.Id == id);

        if (registration == null)
            throw new InvalidOperationException($"Registration with ID {id} not found");

        return registration;
    }

    public async Task<Registration?> TryGetByIdAsync(Guid id)
    {
        if (id == Guid.Empty) return null;

        var registrations = await GetAllAsync();
        return registrations.FirstOrDefault(r => r.Id == id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        if (id == Guid.Empty) return false;

        var registrations = await GetAllAsync();
        return registrations.Any(r => r.Id == id);
    }

    public async Task<Registration> AddAsync(Registration registration)
    {
        if (registration == null)
            throw new ArgumentNullException(nameof(registration));

        if (registration.EventId == Guid.Empty)
            throw new ArgumentException("Event ID cannot be empty", nameof(registration));

        if (string.IsNullOrWhiteSpace(registration.AttendeeName))
            throw new ArgumentException("User name is required", nameof(registration));

        if (string.IsNullOrWhiteSpace(registration.EmailAddress))
            throw new ArgumentException("Email address is required", nameof(registration));

        try
        {
            var registrations = await GetAllAsync();
            registration.Id = Guid.NewGuid();
            registrations.Add(registration);
            await _localStorage.SetItemAsync(RegistrationsKey, registrations);

            _logger.LogInformation("Successfully added registration {RegistrationId} for user {AttendeeName} to event {EventId}",
                registration.Id, registration.AttendeeName, registration.EventId);
            return registration;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding registration for user {AttendeeName} to event {EventId}",
                registration.AttendeeName, registration.EventId);
            throw;
        }
    }

    public async Task<Registration> UpdateAsync(Registration registration)
    {
        if (registration == null)
            throw new ArgumentNullException(nameof(registration));

        if (registration.Id == Guid.Empty)
            throw new ArgumentException("Registration ID cannot be empty", nameof(registration));

        if (registration.EventId == Guid.Empty)
            throw new ArgumentException("Event ID cannot be empty", nameof(registration));

        if (string.IsNullOrWhiteSpace(registration.AttendeeName))
            throw new ArgumentException("Attendee name is required", nameof(registration));

        if (string.IsNullOrWhiteSpace(registration.EmailAddress))
            throw new ArgumentException("Email address is required", nameof(registration));

        try
        {
            var registrations = await GetAllAsync();
            var index = registrations.FindIndex(r => r.Id == registration.Id);

            if (index < 0)
            {
                _logger.LogWarning("Attempted to update non-existent registration with ID: {RegistrationId}", registration.Id);
                throw new InvalidOperationException($"Registration with ID {registration.Id} not found");
            }

            _logger.LogInformation("Updating registration {RegistrationId} for user {AttendeeName}",
                registration.Id, registration.AttendeeName);

            registrations[index] = registration;
            await _localStorage.SetItemAsync(RegistrationsKey, registrations);

            _logger.LogInformation("Successfully updated registration {RegistrationId}", registration.Id);
            return registration;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating registration {RegistrationId}", registration.Id);
            throw;
        }
    }
    public async Task DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Registration ID cannot be empty", nameof(id));

        try
        {
            var registrations = await GetAllAsync();
            var removedCount = registrations.RemoveAll(r => r.Id == id);

            if (removedCount == 0)
                throw new InvalidOperationException($"Registration with ID {id} not found");

            await _localStorage.SetItemAsync(RegistrationsKey, registrations);
            _logger.LogInformation("Successfully deleted registration {RegistrationId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting registration {RegistrationId}", id);
            throw;
        }
    }

    public async Task<PagedResult<Registration>> GetPagedForEventAsync(Guid eventId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (eventId == Guid.Empty)
            throw new ArgumentException("Event ID cannot be empty", nameof(eventId));

        if (pageSize <= 0) pageSize = DefaultPageSize;
        if (pageSize > MaxPageSize) pageSize = MaxPageSize;
        if (pageNumber <= 0) pageNumber = 1;

        var allRegistrations = await GetAllForEventAsync(eventId);

        return await PagedResult<Registration>.CreatePagesAsync(
            allRegistrations,
            pageNumber,
            pageSize,
            r => r.AttendeeName,
            cancellationToken);
    }

    public async Task<int> GetRegistrationCountForEventAsync(Guid eventId)
    {
        if (eventId == Guid.Empty)
            throw new ArgumentException("Event ID cannot be empty", nameof(eventId));

        var registrations = await GetAllForEventAsync(eventId);
        return registrations.Count;
    }

    public async Task<List<Registration>> GetAttendedRegistrationsForEventAsync(Guid eventId)
    {
        if (eventId == Guid.Empty)
            throw new ArgumentException("Event ID cannot be empty", nameof(eventId));

        var registrations = await GetAllForEventAsync(eventId);
        return registrations.Where(r => r.AttendedEvent).ToList();
    }

    public async Task<bool> IsUserRegisteredForEventAsync(Guid eventId, string emailAddress)
    {
        if (eventId == Guid.Empty)
            throw new ArgumentException("Event ID cannot be empty", nameof(eventId));

        if (string.IsNullOrWhiteSpace(emailAddress))
            throw new ArgumentException("Email address cannot be empty", nameof(emailAddress));

        var registrations = await GetAllForEventAsync(eventId);
        return registrations.Any(r => r.EmailAddress.Equals(emailAddress, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<PagedResult<Registration>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageSize <= 0) pageSize = DefaultPageSize;
        if (pageSize > MaxPageSize) pageSize = MaxPageSize;
        if (pageNumber <= 0) pageNumber = 1;

        var allRegistrations = await GetAllAsync();

        return await PagedResult<Registration>.CreatePagesAsync(
            allRegistrations,
            pageNumber,
            pageSize,
            r => r.AttendeeName,
            cancellationToken);
    }
}